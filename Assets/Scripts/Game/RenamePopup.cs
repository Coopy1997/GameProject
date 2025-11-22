using UnityEngine;
using TMPro;

public class RenamePopup : MonoBehaviour
{
    public GameObject window;
    public TMP_InputField nameInput;

    Fish targetFish;
    FishInfoUI infoUI;

    // called by FishInfoUI
    public void OpenForFish(Fish fish, FishInfoUI sourceUi)
    {
        targetFish = fish;
        infoUI = sourceUi;

        if (nameInput != null)
            nameInput.text = fish != null ? fish.GetDisplayName() : "";

        if (window != null) window.SetActive(true);
        else gameObject.SetActive(true);
    }

    // 🔹 THIS is the method you hook to the OK button
    public void ConfirmRename()
    {
        if (targetFish == null) { Close(); return; }

        string newName = nameInput != null ? nameInput.text : "";
        targetFish.SetCustomName(newName);

        if (infoUI != null)
            infoUI.Refresh();

        Close();
    }

    // 🔹 Hook this to the Cancel / X button if you like
    public void Close()
    {
        if (window != null) window.SetActive(false);
        else gameObject.SetActive(false);
    }
}
