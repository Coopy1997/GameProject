using UnityEngine;

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    VeryRare
}

[System.Serializable]
public class FishCatalogItem
{
    public string id;                 // "SunnyGuppy"
    public string displayName;        // "Sunny Guppy"
    public Fish prefab;
    public Sprite icon;

    public FishRarity rarity = FishRarity.Common;

    public int buyPrice = 50;
    public int sellPrice = 40;

    public float timeToAdult = 60f;
    public float baseBreedChance = 0.4f;
}
