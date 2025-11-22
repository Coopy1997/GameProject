using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    public GameObject root;
    public TMP_Text text;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    // Show tooltip with text + position
    public void Show(string tooltipText, Vector2 screenPos)
    {
        if (root == null || text == null) return;

        text.text = tooltipText;
        root.SetActive(true);

        // convert screenPos → world/UI position
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.transform.parent as RectTransform,
            screenPos,
            null,
            out anchoredPos
        );

        root.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }

    // Hide tooltip
    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}
