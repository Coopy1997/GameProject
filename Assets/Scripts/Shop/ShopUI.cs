using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public Button buyFishButton;
    public TMP_Text fishCostText;
    public int baseFishCost = 100;
    public float costMultiplier = 1.15f;

    int fishBought = 0;

    void Awake()
    {
        RefreshUI();
        if (buyFishButton) buyFishButton.onClick.AddListener(BuyFish);
    }

    int GetCurrentFishCost()
    {
        return Mathf.CeilToInt(baseFishCost * Mathf.Pow(costMultiplier, fishBought));
    }

    void RefreshUI()
    {
        if (fishCostText) fishCostText.text = GetCurrentFishCost() + " G";
    }

    public void BuyFish()
    {
        if (!GameController.Instance) return;
        int cost = GetCurrentFishCost();
        if (!GameController.Instance.TrySpendGold(cost)) return;
        GameController.Instance.SpawnFish();
        fishBought++;
        RefreshUI();
    }
}
