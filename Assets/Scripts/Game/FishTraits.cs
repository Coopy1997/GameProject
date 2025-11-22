using UnityEngine;

public enum TraitRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class Trait
{
    public string id;
    public string traitName;
    [TextArea] public string description;
    public TraitRarity rarity;

    // Modifiers (optional – used in Fish & value)
    public float hungerDrainModifier;   // positive = drains faster, negative = drains slower
    public float sellPriceMultiplier;   // e.g. 0.2 = +20% value

    public string GetColorHex()
    {
        switch (rarity)
        {
            case TraitRarity.Uncommon: return "#6BFF83";  // light green
            case TraitRarity.Rare: return "#7D7BFF";  // blue
            case TraitRarity.Epic: return "#FF4DF2";  // pink
            case TraitRarity.Legendary: return "#FFB300";  // orange
            default: return "#FFFFFF";  // white
        }
    }
}
