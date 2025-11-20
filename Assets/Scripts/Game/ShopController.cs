using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text filterLevelText;   // Section_Upgrades/Row_Filter/Level
    public TMP_Text filterCostText;    // Section_Upgrades/Row_Filter/Label (shows "Cost: XYZ G")

    [Header("Costs")]
    public int baseFilterCost = 150;
    public float costMultiplier = 1.6f;

    [Header("State")]
    [SerializeField] private int currentFilterLevel = 1;
    public int CurrentFilterLevel => currentFilterLevel;

    void Start()
    {
        // Apply starting level to the water system
        ApplyFilterLevelToSystem();
        RefreshUI();
    }

    // --- Button hook from the Upgrades row ---
    public void BuyFilterUpgrade()
    {
        int cost = GetCurrentFilterCost();

        if (TrySpend(cost))
        {
            currentFilterLevel++;
            ApplyFilterLevelToSystem();
            RefreshUI();
            Debug.Log($"Filter upgraded -> Lv. {currentFilterLevel}");
        }
        else
        {
            Debug.Log("Not enough gold to upgrade filter.");
        }
    }

    // --- UI helpers ---
    public void RefreshUI()
    {
        if (filterLevelText) filterLevelText.text = $"Filter Lv. {Mathf.Max(1, currentFilterLevel)}";
        if (filterCostText) filterCostText.text = $"Cost: {GetCurrentFilterCost()} G";
    }

    public int GetCurrentFilterCost()
    {
        // cost grows with level (Lv.1 uses base cost)
        int levelIndex = Mathf.Max(0, currentFilterLevel - 1);
        return Mathf.CeilToInt(baseFilterCost * Mathf.Pow(costMultiplier, levelIndex));
    }

    // --- Integration with other systems ---
    void ApplyFilterLevelToSystem()
    {
        // Use singleton if you have it; otherwise try to find the component.
        var wh = WaterHealthController.Instance ?
                 WaterHealthController.Instance :
                 FindObjectOfType<WaterHealthController>();

        if (wh)
        {
            // Expecting a method like: void SetFilterLevel(int level)
            wh.SetFilterLevel(currentFilterLevel);
        }
    }

    bool TrySpend(int amount)
    {
        // Prefer GameController.Instance.TrySpend if your class has it.
        // We avoid a hard compile-time call so this also works when that
        // method doesn’t exist.
        var gc = FindObjectOfType<GameController>();
        if (!gc) return false;

        // If your GameController already has a TrySpend(int) method,
        // you can uncomment this line and remove the fallback below.
        // return gc.TrySpend(amount);

        // Fallback spend: directly reduce gold if enough and ask GC to refresh UI.
        if (gc.gold < amount) return false;
        gc.gold -= amount;

        // Let GameController refresh its UI if it has such a method
        // (SendMessage doesn’t care about access modifiers; it’s optional).
        gc.SendMessage("UpdateGoldUI", SendMessageOptions.DontRequireReceiver);
        return true;
    }
}
