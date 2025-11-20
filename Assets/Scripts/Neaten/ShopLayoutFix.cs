using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabbedShopController : MonoBehaviour
{
    [Header("Tabs")]
    public Button tabFish;
    public Button tabUpgrades;
    public Button tabDeco;

    [Header("Sections (containers under Section_Container)")]
    public GameObject sectionFish;      // Section_Fish
    public GameObject sectionUpgrades;  // Section_Upgrades
    public GameObject sectionDeco;      // Section_Deco

    [Header("Row Buttons (inside each section)")]
    public Button buyFishButton;        // Section_Fish/Row_BuyFish/Buy
    public Button upgradeFilterButton;  // Section_Upgrades/Row_Filter/Upgrade
    public Button buyDecoButton;        // Section_Deco/Row_Plant/Buy (placeholder)

    [Header("Row Labels (optional, if you use them)")]
    public TextMeshProUGUI fishCostText;      // Section_Fish/Row_BuyFish/Cost
    public TextMeshProUGUI filterCostText;    // Section_Upgrades/Row_Filter/Label
    public TextMeshProUGUI filterLevelText;   // Section_Upgrades/Row_Filter/Level
    public TextMeshProUGUI decoCostText;      // Section_Deco/Row_Plant/Cost

    [Header("Optional external logic")]
    public ShopUI shopUI;                // so Buy Fish calls ShopUI.BuyFish
    public ShopController shopController;// so Upgrade calls ShopController.BuyFilterUpgrade

    void Awake()
    {
        // Tab clicks
        if (tabFish) tabFish.onClick.AddListener(() => ShowTab(0));
        if (tabUpgrades) tabUpgrades.onClick.AddListener(() => ShowTab(1));
        if (tabDeco) tabDeco.onClick.AddListener(() => ShowTab(2));

        // Button labels (safe even if TMP child already exists)
        SetButtonLabel(buyFishButton, "Buy Fish");
        SetButtonLabel(upgradeFilterButton, "Upgrade Filter");
        SetButtonLabel(buyDecoButton, "Buy Decoration");

        // Button actions (wire to your game logic if present)
        if (buyFishButton)
        {
            buyFishButton.onClick.RemoveAllListeners();
            if (shopUI) buyFishButton.onClick.AddListener(shopUI.BuyFish);
        }
        if (upgradeFilterButton)
        {
            upgradeFilterButton.onClick.RemoveAllListeners();
            if (shopController) upgradeFilterButton.onClick.AddListener(shopController.BuyFilterUpgrade);
        }
        if (buyDecoButton)
        {
            buyDecoButton.onClick.RemoveAllListeners();
            // placeholder: no action yet
        }

        // Default tab
        ShowTab(1); // open Upgrades first (change to 0 if you want Fish first)
        RefreshTexts();
    }

    public void RefreshTexts()
    {
        // If you’re driving costs/levels from code, update here.
        if (shopUI && fishCostText) fishCostText.text = $"Cost: {shopUI.baseFishCost} G";
        if (shopController)
        {
            if (filterCostText) filterCostText.text = $"Cost: {shopController.baseFilterCost} G";
            if (filterLevelText) filterLevelText.text = $"Filter Lv. {Mathf.Max(1, shopController.CurrentFilterLevel)}";
        }
        if (decoCostText) decoCostText.text = "Cost: 50 G";
    }

    // --- helpers ---
    void ShowTab(int index)
    {
        if (sectionFish) sectionFish.SetActive(index == 0);
        if (sectionUpgrades) sectionUpgrades.SetActive(index == 1);
        if (sectionDeco) sectionDeco.SetActive(index == 2);

        // simple tab state color
        SetTabState(tabFish, index == 0);
        SetTabState(tabUpgrades, index == 1);
        SetTabState(tabDeco, index == 2);
    }

    void SetTabState(Button b, bool active)
    {
        if (!b) return;
        var img = b.GetComponent<Image>();
        var txt = b.GetComponentInChildren<TextMeshProUGUI>(true);
        if (img) img.color = active ? new Color32(64, 128, 255, 255) : new Color32(230, 237, 243, 255);
        if (txt) txt.color = active ? Color.white : Color.black;
    }

    void SetButtonLabel(Button b, string text)
    {
        if (!b) return;
        var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
        if (!tmp)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(b.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
            tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
        }
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.fontSize = 28;
        tmp.color = Color.black;
        tmp.text = text;
    }
}
