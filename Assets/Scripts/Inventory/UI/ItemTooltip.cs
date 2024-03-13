using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;
    public Text itemInfoText;
    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.description;
    }


    public void UpdatePosition()
    {
        var mousePos = Input.mousePosition;
        var corners = new Vector3[4];

        m_RectTransform.GetWorldCorners(corners);

        var width = corners[3].x - corners[0].x;
        var height = corners[1].y - corners[0].y;

        if (mousePos.y < height)
            m_RectTransform.position = mousePos + Vector3.up * height * 0.6f;
        else if (Screen.width - mousePos.x > width)
            m_RectTransform.position = mousePos + Vector3.right * width * 0.6f;
        else
            m_RectTransform.position = mousePos + Vector3.left * width * 0.6f;
    }
}