using System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : PersistentMonoSingleton<MouseManager>
{
    public bool IsInitialized => enabled;
    public Texture2D point, doorway, attack, target, arrow;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    private RaycastHit m_HitInfo;
    private readonly Vector2 m_Hotspot = new Vector2(16, 16);

    private void Update()
    {
        SetCursorTexture();
        if (InteractWithUI()) return;
        MouseControl();
    }

    private void SetCursorTexture()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (InteractWithUI())
        {
            Cursor.SetCursor(point, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (Physics.Raycast(ray, out m_HitInfo))
        {
            switch (m_HitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, m_Hotspot, CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, m_Hotspot, CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, m_Hotspot, CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, m_Hotspot, CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, m_Hotspot, CursorMode.Auto);
                    break;
            }
        }
    }

    private void MouseControl()
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

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }
}