using UnityEngine;

public class ShopOpener : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject backdrop; // optional

    public void OpenShop()
    {
        if (backdrop) backdrop.SetActive(true);
        if (shopPanel) shopPanel.SetActive(true);

        // Optional: if you use ModalAnimator on the panel:
        var anim = shopPanel ? shopPanel.GetComponent<ModalAnimator>() : null;
        if (anim) anim.PlayOpen();
    }

    public void CloseShop()
    {
        // Optional: play close animation then disable via animation event
        var anim = shopPanel ? shopPanel.GetComponent<ModalAnimator>() : null;
        if (anim) anim.PlayClose();
        else
        {
            if (shopPanel) shopPanel.SetActive(false);
            if (backdrop) backdrop.SetActive(false);
        }
    }
}
