using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jiang.Games
{
    public partial class MouseManager : ViewController
    {
        public static EasyEvent<Vector3> OnMouseClickedEvent = new EasyEvent<Vector3>();
        public static EasyEvent<GameObject> OnEnemyClickedEvent = new EasyEvent<GameObject>();

        private readonly Vector2 _Hotspot = new Vector2(16, 16);
        private RaycastHit _HitInfo;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

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
                Cursor.SetCursor(PointSprite, Vector2.zero, CursorMode.Auto);
                return;
            }

            if (Physics.Raycast(ray, out _HitInfo))
            {
                switch (_HitInfo.collider.gameObject.tag)
                {
                    case "Ground":
                        Cursor.SetCursor(TargetSprite, _Hotspot, CursorMode.Auto);
                        break;
                    case "Enemy":
                        Cursor.SetCursor(AttackSprite, _Hotspot, CursorMode.Auto);
                        break;
                    case "Portal":
                        Cursor.SetCursor(DoorwaySprite, _Hotspot, CursorMode.Auto);
                        break;
                    case "Item":
                        Cursor.SetCursor(PointSprite, _Hotspot, CursorMode.Auto);
                        break;

                    default:
                        Cursor.SetCursor(ArrowSprite, _Hotspot, CursorMode.Auto);
                        break;
                }
            }
        }

        private void MouseControl()
        {
            if (!Input.GetMouseButtonDown(0) || _HitInfo.collider == null) return;

            if (_HitInfo.collider.gameObject.CompareTag("Ground") ||
                _HitInfo.collider.gameObject.CompareTag("Portal") ||
                _HitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClickedEvent.Trigger(_HitInfo.point);
            }

            if (_HitInfo.collider.gameObject.CompareTag("Enemy") ||
                _HitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClickedEvent.Trigger(_HitInfo.collider.gameObject);
            }
        }

        private bool InteractWithUI()
        {
            // If the mouse is over a UI element, return true，otherwise return false
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}