using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TraitTooltipTrigger : MonoBehaviour, IPointerClickHandler
{
    public FishInfoUI infoUI;
    public TMP_Text traitsLabel;
    public TooltipUI tooltip;

    void Awake()
    {
        if (traitsLabel == null) traitsLabel = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tooltip == null)
        {
            Debug.Log("TraitTooltipTrigger: no tooltip reference");
            return;
        }

        // if it's already visible, clicking again just closes it
        if (tooltip.IsVisible)
        {
            tooltip.Hide();
            Debug.Log("TraitTooltipTrigger: CLICK close tooltip");
            return;
        }

        string msg = "";

        if (infoUI != null && !string.IsNullOrEmpty(infoUI.traitsTooltip))
        {
            msg = infoUI.traitsTooltip;
        }
        else if (traitsLabel != null)
        {
            msg = traitsLabel.text;
        }

        if (string.IsNullOrEmpty(msg))
        {
            Debug.Log("TraitTooltipTrigger: empty message, nothing to show");
            return;
        }

        tooltip.Show(msg);
        Debug.Log("TraitTooltipTrigger: CLICK open tooltip");
    }
}
