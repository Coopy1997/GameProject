using UnityEngine;

public enum ToolType { Hand, Food, Medicine }

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    [Header("Current Tool")]
    public ToolType currentTool = ToolType.Hand;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Methods to call from UI Button OnClick()
    public void SelectHand() => SetTool(ToolType.Hand);
    public void SelectFood() => SetTool(ToolType.Food);
    public void SelectMedicine() => SetTool(ToolType.Medicine);

    public void SetTool(ToolType tool)
    {
        currentTool = tool;
        Debug.Log($"Tool changed to: {tool}");
    }
}
