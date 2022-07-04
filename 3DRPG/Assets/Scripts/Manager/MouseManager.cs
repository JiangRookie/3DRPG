using System;
using UnityEngine;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    private RaycastHit m_HitInfo;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        MouseControl();
    }

    private void FixedUpdate()
    {
        SetCursorTexture();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out m_HitInfo))
        {
            //切换鼠标贴图
            switch (m_HitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && m_HitInfo.collider != null)
        {
            if (m_HitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(m_HitInfo.point);
            if (m_HitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(m_HitInfo.collider.gameObject);
            if (m_HitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(m_HitInfo.collider.gameObject);
            if (m_HitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(m_HitInfo.point);
            if (m_HitInfo.collider.gameObject.CompareTag("Item"))
                OnMouseClicked?.Invoke(m_HitInfo.point);
        }
    }
}