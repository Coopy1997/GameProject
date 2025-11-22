using System.Collections.Generic;
using UnityEngine;

public static class TraitDatabase
{
    public static List<Trait> allTraits = new List<Trait>()
    {
        new Trait(TraitType.Hungry, "Hungry", "Eats more food, hunger drains faster.", TraitRarity.Common),
        new Trait(TraitType.Peckish, "Peckish", "Eats less food, hunger drains slower.", TraitRarity.Common),
        new Trait(TraitType.Glutton, "Glutton", "Hunger drains much faster, but food fills more.", TraitRarity.Uncommon),
        new Trait(TraitType.SlowMetabolism, "Slow Metabolism", "Hunger drains much slower.", TraitRarity.Uncommon),

        new Trait(TraitType.Hardy, "Hardy", "Health decreases slower when hungry or in bad water.", TraitRarity.Uncommon),
        new Trait(TraitType.Fragile, "Fragile", "Health decreases faster in bad conditions.", TraitRarity.Common),

        new Trait(TraitType.RapidGrowth, "Rapid Growth", "Ages and matures faster.", TraitRarity.Rare),
        new Trait(TraitType.SlowGrowth, "Slow Growth", "Ages slower.", TraitRarity.Common),

        new Trait(TraitType.Social, "Social", "Health drains slower with other fish around.", TraitRarity.Uncommon),
        new Trait(TraitType.Territorial, "Territorial", "More stressed in crowded tanks.", TraitRarity.Common),

        new Trait(TraitType.Breeder, "Breeder", "Higher chance to breed.", TraitRarity.Rare),
        new Trait(TraitType.Shy, "Shy", "Lower chance to breed unless fewer fish.", TraitRarity.Common),

        new Trait(TraitType.Calm, "Calm", "Swims slower, saves hunger.", TraitRarity.Uncommon),
        new Trait(TraitType.Energetic, "Energetic", "Swims faster, uses more hunger.", TraitRarity.Common),

        new Trait(TraitType.CleanFreak, "Clean Freak", "Heals faster in clean water.", TraitRarity.Rare),
        new Trait(TraitType.DirtyScavenger, "Dirty Scavenger", "Ignores dirty water penalties.", TraitRarity.Rare),

        new Trait(TraitType.Lucky, "Lucky", "Better chance for rare offspring and events.", TraitRarity.Epic),
        new Trait(TraitType.Mutated, "Mutated", "Weird genetics, higher breed chance but drains health faster.", TraitRarity.Epic),

        new Trait(TraitType.JewelScales, "Jewel Scales", "Sell value is higher.", TraitRarity.Rare),
        new Trait(TraitType.Drab, "Drab", "Sell value is lower but drains less health.", TraitRarity.Common),

        new Trait(TraitType.AncientSpirit, "Ancient Spirit", "A mythical shimmering aura surrounds this fish.", TraitRarity.Legendary)
    };

    public static List<Trait> GetRandomTraits(int minCount, int maxCount)
    {
        if (minCount < 0) minCount = 0;
        if (maxCount > allTraits.Count) maxCount = allTraits.Count;
        if (maxCount < minCount) maxCount = minCount;

        int count = Random.Range(minCount, maxCount + 1);

        List<Trait> result = new List<Trait>();
        List<Trait> pool = new List<Trait>(allTraits);

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public static Trait GetLegendaryTrait()
    {
        for (int i = 0; i < allTraits.Count; i++)
        {
            if (allTraits[i].rarity == TraitRarity.Legendary)
                return allTraits[i];
        }
        return null;
    }
}

public static class TraitColors
{
    public static string GetColor(TraitRarity rarity)
    {
        switch (rarity)
        {
            default:
            case TraitRarity.Common: return "#FFFFFF";  // white
            case TraitRarity.Uncommon: return "#6CFF78";  // green
            case TraitRarity.Rare: return "#45C6FF";  // blue
            case TraitRarity.Epic: return "#C55BFF";  // purple
            case TraitRarity.Legendary: return "#FFA500";  // orange
        }
    }
}

