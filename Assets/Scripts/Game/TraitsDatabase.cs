using System.Collections.Generic;

public enum TraitType
{
    Hungry,
    Peckish,
    Glutton,
    SlowMetabolism,
    Hardy,
    Fragile,
    RapidGrowth,
    SlowGrowth,
    Social,
    Territorial,
    Breeder,
    Shy,
    Calm,
    Energetic,
    CleanFreak,
    DirtyScavenger,
    Lucky,
    Mutated,
    JewelScales,
    Drab
}

[System.Serializable]
public class Trait
{
    public TraitType type;
    public string name;
    public string description;

    public Trait(TraitType t, string n, string d)
    {
        type = t;
        name = n;
        description = d;
    }
}

public static class TraitDatabase
{
    public static List<Trait> allTraits = new List<Trait>
    {
        new Trait(TraitType.Hungry, "Hungry", "Eats more food, hunger drains faster."),
        new Trait(TraitType.Peckish, "Peckish", "Eats less food, hunger drains slower."),
        new Trait(TraitType.Glutton, "Glutton", "Hunger drains much faster, but food fills more."),
        new Trait(TraitType.SlowMetabolism, "Slow Metabolism", "Hunger drains much slower."),
        new Trait(TraitType.Hardy, "Hardy", "Loses less health when starving."),
        new Trait(TraitType.Fragile, "Fragile", "Loses more health when conditions are bad."),
        new Trait(TraitType.RapidGrowth, "Rapid Growth", "Ages and matures faster."),
        new Trait(TraitType.SlowGrowth, "Slow Growth", "Ages slower."),
        new Trait(TraitType.Social, "Social", "Happier with more fish."),
        new Trait(TraitType.Territorial, "Territorial", "More stressed with lots of fish."),
        new Trait(TraitType.Breeder, "Breeder", "Higher chance to breed."),
        new Trait(TraitType.Shy, "Shy", "Lower chance to breed."),
        new Trait(TraitType.Calm, "Calm", "Swims slower, saves energy."),
        new Trait(TraitType.Energetic, "Energetic", "Swims faster, uses more energy."),
        new Trait(TraitType.CleanFreak, "Clean Freak", "Heals faster in clean water."),
        new Trait(TraitType.DirtyScavenger, "Dirty Scavenger", "Ignores dirty water penalties."),
        new Trait(TraitType.Lucky, "Lucky", "Better chance for rare stuff."),
        new Trait(TraitType.Mutated, "Mutated", "Weird genetics, breed chance a bit higher, health drains faster."),
        new Trait(TraitType.JewelScales, "Jewel Scales", "Sells for more."),
        new Trait(TraitType.Drab, "Drab", "Sells for less but drains less health.")
    };

    public static List<Trait> GetRandomTraits(int minCount, int maxCount)
    {
        if (minCount < 0) minCount = 0;
        if (maxCount > allTraits.Count) maxCount = allTraits.Count;
        if (maxCount < minCount) maxCount = minCount;

        int count = UnityEngine.Random.Range(minCount, maxCount + 1);

        List<Trait> result = new List<Trait>();
        List<Trait> pool = new List<Trait>(allTraits);

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;
            int index = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}
