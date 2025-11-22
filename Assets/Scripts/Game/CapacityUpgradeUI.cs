using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapacityUpgradeUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text capacityLabel;   // e.g. "Tank Capacity: 3"
    public TMP_Text costLabel;       // e.g. "Cost: 50 G"
    public Button buyButton;

    [Header("Upgrade Data")]
    public int[] upgradeCosts = { 50, 100, 200 };
    public int capacityPerLevel = 2;

    int currentLevel = 0;

    void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuyUpgrade);

        Refresh();
    }

    void Refresh()
    {
        if (GameController.Instance != null && capacityLabel != null)
            capacityLabel.text = "Tank Capacity: " + GameController.Instance.maxFish;

        if (currentLevel >= upgradeCosts.Length)
        {
            if (costLabel) costLabel.text = "MAX";
            if (buyButton) buyButton.interactable = false;
        }
        else
        {
            int cost = upgradeCosts[currentLevel];
            if (costLabel) costLabel.text = "Cost: " + cost + " G";
            if (buyButton) buyButton.interactable = true;
        }
    }

    void BuyUpgrade()
    {
        if (GameController.Instance == null) return;
        if (currentLevel >= upgradeCosts.Length) return;

        int cost = upgradeCosts[currentLevel];
        if (!GameController.Instance.TrySpendGold(cost))
            return;

        currentLevel++;
        GameController.Instance.maxFish += capacityPerLevel;
        if (GameController.Instance.maxFish < 1)
            GameController.Instance.maxFish = 1;

        Refresh();
    }
}
