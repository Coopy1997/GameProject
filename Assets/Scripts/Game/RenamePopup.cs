using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RenamePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;          // the whole popup, usually the RenamePopup object
    public TMP_InputField inputField;
    public Button okButton;
    public Button cancelButton;

    private Fish targetFish;
    private FishInfoUI infoUI;

    void Awake()
    {
        if (root == null)
            root = gameObject;

        root.SetActive(false);

        if (okButton != null)
            okButton.onClick.AddListener(Apply);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Close);
    }

    public void Open(Fish fish, FishInfoUI ui)
    {
        targetFish = fish;
        infoUI = ui;

        if (root != null)
            root.SetActive(true);

        if (inputField != null)
        {
            inputField.text = string.IsNullOrEmpty(fish.fishName) ? "" : fish.fishName;
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    void Apply()
    {
        if (targetFish != null && inputField != null)
        {
            string newName = inputField.text.Trim();
            if (string.IsNullOrEmpty(newName))
                newName = "Unnamed Fish";

            targetFish.fishName = newName;
        }

        // refresh info panel
        if (infoUI != null)
            infoUI.ForceRefresh();

        Close();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        targetFish = null;
        infoUI = null;
    }
}
