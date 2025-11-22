using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class FishInfoUI : MonoBehaviour
{
    public static FishInfoUI Instance;

    [Header("Root")]
    public GameObject root;

    [Header("Header")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text breedAgeText;

    [Header("Bars")]
    public TMP_Text hungerLabel;
    public Image hungerBar;
    public TMP_Text healthLabel;
    public Image healthBar;

    [Header("Traits")]
    public TMP_Text traitsText;

    [Header("Value / Actions")]
    public TMP_Text valueText;   // "Value: 120 G"
    public Button sellButton;

    [Header("Tooltip")]
    public ToolTipUI traitsTooltip;

    Fish currentFish;

    private void Awake()
    {
        Instance = this;

        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellButtonClicked);

        Hide();
    }

    public void Show(Fish fish)
    {
        if (!fish)
        {
            Hide();
            return;
        }

        currentFish = fish;
        root.SetActive(true);

        RefreshUI();
    }

    public void ForceRefresh()
    {
        if (currentFish != null)
            RefreshUI();
    }

    void RefreshUI()
    {
        if (currentFish == null) return;

        // Icon
        if (iconImage && currentFish.sr)
            iconImage.sprite = currentFish.sr.sprite;

        // Name
        if (string.IsNullOrEmpty(currentFish.fishName))
            nameText.text = "Unnamed Fish";
        else
            nameText.text = currentFish.fishName;

        // Breed + sex + age
        string breed = currentFish.isBaby ? "???" : currentFish.breedDisplayName;
        string sex = currentFish.sex == FishSex.Male ? "♂" : "♀";

        int ageS = Mathf.FloorToInt(currentFish.ageSeconds);
        int m = ageS / 60;
        int s = ageS % 60;

        breedAgeText.text = $"{breed}  {sex}   Age: {m}m {s}s";

        // Bars
        if (hungerBar)
            hungerBar.fillAmount = currentFish.Hunger01;
        if (hungerLabel)
            hungerLabel.text = "Hunger: " + Mathf.RoundToInt(currentFish.Hunger01 * 100f) + "%";

        if (healthBar)
            healthBar.fillAmount = currentFish.health01;
        if (healthLabel)
            healthLabel.text = "Health: " + Mathf.RoundToInt(currentFish.health01 * 100f) + "%";

        // Traits
        traitsText.text = BuildTraitList();

        // Value
        if (valueText != null)
        {
            int value = currentFish.GetSellPrice();
            string color = GetValueColor(currentFish, value);
            valueText.text = $"Value: <color={color}>{value} G</color>";
        }
    }

    string BuildTraitList()
    {
        if (currentFish.traits == null || currentFish.traits.Count == 0)
            return "Traits:\n\n• None";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Traits:\n");

        foreach (var t in currentFish.traits)
        {
            if (t == null) continue;
            string color = TraitColors.GetColor(t.rarity);
            sb.AppendLine($"• <color={color}>{t.name}</color>");
        }

        return sb.ToString();
    }

    // Decide the color of the value text based on rarity & price
    string GetValueColor(Fish fish, int value)
    {
        // 1) Check highest rarity trait
        int rarityRank = 0; // 0=common,1=uncommon,2=rare,3=epic,4=legendary

        if (fish.traits != null)
        {
            foreach (var t in fish.traits)
            {
                if (t == null) continue;
                int r = 0;
                switch (t.rarity)
                {
                    case TraitRarity.Common: r = 0; break;
                    case TraitRarity.Uncommon: r = 1; break;
                    case TraitRarity.Rare: r = 2; break;
                    case TraitRarity.Epic: r = 3; break;
                    case TraitRarity.Legendary: r = 4; break;
                }
                if (r > rarityRank) rarityRank = r;
            }
        }

        // 2) Use price as secondary factor for low-rarity fish
        if (rarityRank < 2) // common/uncommon
        {
            if (value >= 200) rarityRank = 2;  // treat as rare color if high value
            if (value >= 400) rarityRank = 3;  // epic color
            if (value >= 800) rarityRank = 4;  // legendary color
        }

        // 3) Map final rarityRank to actual color
        switch (rarityRank)
        {
            default:
            case 0: return "#CCCCCC"; // dull grey
            case 1: return "#6CFF78"; // green
            case 2: return "#45C6FF"; // blue
            case 3: return "#C55BFF"; // purple
            case 4: return "#FFA500"; // orange / legendary
        }
    }

    public void Hide()
    {
        currentFish = null;
        if (root != null)
            root.SetActive(false);
        if (traitsTooltip != null)
            traitsTooltip.Hide();
    }

    public Fish GetCurrentFish()
    {
        return currentFish;
    }

    public void OnSellButtonClicked()
    {
        if (currentFish == null) return;
        if (GameController.Instance != null)
            GameController.Instance.SellFish(currentFish);

        Hide();
    }
}
