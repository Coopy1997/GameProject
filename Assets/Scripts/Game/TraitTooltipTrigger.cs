using UnityEngine;
using UnityEngine.EventSystems;

public class TraitTooltipTrigger : MonoBehaviour, IPointerClickHandler
{
    public FishInfoUI infoUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!infoUI || !infoUI.traitsTooltip) return;

        if (infoUI.traitsTooltip.gameObject.activeSelf)
            infoUI.traitsTooltip.Hide();
        else
            infoUI.traitsTooltip.Show(infoUI.traitsText.text, eventData.position);
    }
}
