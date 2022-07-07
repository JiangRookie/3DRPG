using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open in Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }

        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    private DialogueData_SO m_CurrentData;

    private ReorderableList m_PiecesList = null;

    private Vector2 m_ScrollPos = Vector2.zero;

    private Dictionary<string, ReorderableList> m_OptionListDict = new Dictionary<string, ReorderableList>();

    [MenuItem("JStudio/Dialogue Editor")]
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.autoRepaintOnSceneChange = true;
    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.m_CurrentData = data;
    }

    [OnOpenAsset]
    public static bool OpenAsset(int instanceID, int line)
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;

        if (data != null)
        {
            InitWindow(data);
            return true;
        }

        return false;
    }

    private void OnSelectionChange()
    {
        // 选择改变时调用一次
        var newData = Selection.activeObject as DialogueData_SO;

        if (newData != null)
        {
            m_CurrentData = newData;
            SetupReorderableList();
        }
        else
        {
            m_CurrentData = null;
            m_PiecesList = null;
        }

        Repaint();
    }

    private void OnGUI()
    {
        if (m_CurrentData != null)
        {
            EditorGUILayout.LabelField(m_CurrentData.name, EditorStyles.boldLabel);
            GUILayout.Space(10);

            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandHeight
                (true));
            if (m_PiecesList == null) SetupReorderableList();

            m_PiecesList.DoLayoutList();
            GUILayout.EndScrollView();
        }
        else
        {
            // if (GUILayout.Button("Create New Dialogue"))
            // {
            //     string dataPath = "Assets/Game Data/Dialogue Data/";
            //     if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            //
            //     DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();
            //     AssetDatabase.CreateAsset(newData, dataPath + "/" + "New Dialogue.asset");
            //     m_CurrentData = newData;
            // }

            GUILayout.Label("No Data Selected!", EditorStyles.boldLabel);
        }
    }

    private void OnDisable()
    {
        m_OptionListDict.Clear();
    }

    private void SetupReorderableList()
    {
        m_PiecesList = new ReorderableList(m_CurrentData.dialoguePieceList,
            typeof(DialoguePiece), true, true, true, true);

        m_PiecesList.drawHeaderCallback += OnDrawPieceHeader;
        m_PiecesList.drawElementCallback += OnDrawPieceListElement;
        m_PiecesList.elementHeightCallback += OnHeightChanged;
    }

    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(m_CurrentData.dialoguePieceList[index]);
    }

    private float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;

        var isExpand = piece.canExpand;

        if (isExpand)
        {
            height += EditorGUIUtility.singleLineHeight * 9;

            var option = piece.optionList;
            if (option.Count > 1)
            {
                height += EditorGUIUtility.singleLineHeight * option.Count;
            }
        }

        return height;
    }

    private void OnDrawPieceListElement(Rect rect, int index, bool isactive, bool isfocused)
    {
        EditorUtility.SetDirty(m_CurrentData);
    
        GUIStyle textStyle = new GUIStyle("TextField");

        if (index < m_CurrentData.dialoguePieceList.Count)
        {
            var currentPiece = m_CurrentData.dialoguePieceList[index];
            var tempRect = rect;

            tempRect.height = EditorGUIUtility.singleLineHeight;

            currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, currentPiece.ID);

            if (currentPiece.canExpand)
            {
                tempRect.width = 30;
                tempRect.y += tempRect.height;
                EditorGUI.LabelField(tempRect, "ID");

                tempRect.x += tempRect.width;
                tempRect.width = 100;
                currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

                tempRect.x += tempRect.width + 10;
                EditorGUI.LabelField(tempRect, "Quest");

                tempRect.x += 45;
                currentPiece.quest = EditorGUI.ObjectField(tempRect, currentPiece.quest,
                    typeof(QuestData_SO), false) as QuestData_SO;

                tempRect.y += EditorGUIUtility.singleLineHeight + 5;
                tempRect.x = rect.x;
                tempRect.height = 60;
                tempRect.width = tempRect.height;
                currentPiece.image =
                    EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false) as Sprite;

                //文本框作业
                tempRect.x += tempRect.width + 5;
                tempRect.width = rect.width - tempRect.x;
                textStyle.wordWrap = true;
                currentPiece.text = (string)EditorGUI.TextField(tempRect, currentPiece.text);

                // 画选项
                tempRect.y += tempRect.height + 5;
                tempRect.x = rect.x;
                tempRect.width = rect.width;

                string optionListKey = currentPiece.ID + currentPiece.text;
                if (optionListKey != string.Empty)
                {
                    if (!m_OptionListDict.ContainsKey(optionListKey))
                    {
                        var optionList = new ReorderableList(currentPiece.optionList, typeof(DialogueOption), true,
                            true, true, true);

                        optionList.drawHeaderCallback = OnDrawOptionHeader;

                        optionList.drawElementCallback = (optionRect, optionIndex, optionActive, optionFocused) =>
                        {
                            OnDrawOptionElement(currentPiece, optionRect, optionIndex, optionActive, optionFocused);
                        };

                        m_OptionListDict[optionListKey] = optionList;
                    }

                    m_OptionListDict[optionListKey].DoList(tempRect);
                }
            }
        }
    }

    private void OnDrawOptionHeader(Rect rect)
    {
        GUI.Label(rect, "Option Text");
        rect.x += rect.width * 0.5f + 10;
        GUI.Label(rect, "Target ID");
        rect.x += rect.width * 0.3f;
        GUI.Label(rect, "Apply");
    }

    private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive,
        bool optionFocused)
    {
        var currentOption = currentPiece.optionList[optionIndex];
        var tempRect = optionRect;

        tempRect.width = optionRect.width * 0.5f;
        currentOption.text = EditorGUI.TextField(tempRect, currentOption.text);

        tempRect.x += tempRect.width + 5f;
        tempRect.width = optionRect.width * 0.3f;
        currentOption.targetID = EditorGUI.TextField(tempRect, currentOption.targetID);

        tempRect.x += tempRect.width + 5;
        tempRect.width = optionRect.width * 0.2f;
        currentOption.takeQuest = EditorGUI.Toggle(tempRect, currentOption.takeQuest);
    }

    private void OnDrawPieceHeader(Rect rect)
    {
        GUI.Label(rect, "Dialogue Pieces");
    }
}