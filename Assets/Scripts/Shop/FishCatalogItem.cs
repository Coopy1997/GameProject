using UnityEngine;

[System.Serializable]
public class FishCatalogItem
{
    public string id;                  // e.g. "SUNNY_GUPPY"
    public string displayName;         // "Sunny Guppy"
    public Fish prefab;                // prefab with Fish component
    public Sprite icon;
    public int buyCost = 20;

    [Header("Value")]
    public int baseSellValue = 50;     // base price for this breed
    public float breedValueMultiplier = 1f; // e.g. 1 = normal, 2 = rare, 4 = legendary
}
