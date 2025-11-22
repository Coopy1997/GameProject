using System.Collections.Generic;
using UnityEngine;

public class TraitsDatabase : MonoBehaviour
{
    public static TraitsDatabase Instance { get; private set; }

    [Header("All Traits")]
    public List<Trait> allTraits = new List<Trait>();

    [Header("Rarity Weights")]
    public float commonWeight = 60f;
    public float uncommonWeight = 25f;
    public float rareWeight = 10f;
    public float epicWeight = 4f;
    public float legendaryWeight = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Trait GetRandomTraitWeighted()
    {
        if (allTraits == null || allTraits.Count == 0) return null;

        float total = 0f;
        for (int i = 0; i < allTraits.Count; i++)
        {
            Trait t = allTraits[i];
            if (t == null) continue;
            total += GetWeightForRarity(t.rarity);
        }

        if (total <= 0f) return null;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < allTraits.Count; i++)
        {
            Trait t = allTraits[i];
            if (t == null) continue;

            float w = GetWeightForRarity(t.rarity);
            cumulative += w;

            if (roll <= cumulative)
                return t;
        }

        return allTraits[allTraits.Count - 1];
    }

    float GetWeightForRarity(TraitRarity rarity)
    {
        switch (rarity)
        {
            case TraitRarity.Common: return commonWeight;
            case TraitRarity.Uncommon: return uncommonWeight;
            case TraitRarity.Rare: return rareWeight;
            case TraitRarity.Epic: return epicWeight;
            case TraitRarity.Legendary: return legendaryWeight;
            default: return 0f;
        }
    }
}
