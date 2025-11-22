using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishInfoUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Basic Info")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text breedAgeText;

    [Header("Traits")]
    public TMP_Text traitsTitleText;  // "Traits:"
    public TMP_Text traitsBodyText;   // bullet list

    [Header("Bars")]
    public Image hungerBar;
    public Image healthBar;

    [Header("Value")]
    public TMP_Text valueText;

    [Header("Buttons")]
    public Button renameButton;
    public Button sellButton;

    [Header("External")]
    public RenamePopup renamePopup;
    public GameController gameController;

    private Fish currentFish;

    void Start()
    {
        Hide();
    }

    public void Show(Fish fish)
    {
        currentFish = fish;
        if (root != null) root.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
        currentFish = null;
    }

    public void Refresh()
    {
        if (currentFish == null) return;

        // Icon
        if (iconImage != null)
        {
            iconImage.enabled = currentFish.icon != null;
            iconImage.sprite = currentFish.icon;
        }

        // Name
        if (nameText != null)
            nameText.text = currentFish.GetDisplayName();

        // Breed + Age
        if (breedAgeText != null)
        {
            string growth = currentFish.isBaby ? "Baby" : "Adult";
            breedAgeText.text =
                $"{currentFish.breedDisplayName}\n" +
                $"Age: {currentFish.GetAgeString()}\n" +
                $"{growth}";
        }

        // Bars
        if (hungerBar != null)
            hungerBar.fillAmount = currentFish.GetHunger01();
        if (healthBar != null)
            healthBar.fillAmount = currentFish.GetHealth01();

        // Traits
        UpdateTraitsUI();

        // Value
        UpdateValueUI();
    }

    void UpdateTraitsUI()
    {
        if (traitsTitleText != null)
            traitsTitleText.text = "Traits:";

        if (traitsBodyText == null) return;

        traitsBodyText.text = currentFish.GetFormattedTraitList();
    }

    void UpdateValueUI()
    {
        if (valueText == null) return;

        int value = currentFish.GetSellValue();
        string color = currentFish.GetRarityColor();

        valueText.text = $"Value: <color={color}>{value} G</color>";
    }

    // ============================================================
    // BUTTON HANDLERS
    // ============================================================

    public void OnRenameButtonClicked()
    {
        if (renamePopup == null || currentFish == null) return;
        renamePopup.OpenForFish(currentFish, this);
    }

    public void OnSellButtonClicked()
    {
        if (currentFish == null || gameController == null) return;

        gameController.SellFish(currentFish);
        Hide();
    }
}
