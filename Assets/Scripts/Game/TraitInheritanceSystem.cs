using System.Collections.Generic;
using UnityEngine;

public class TraitInheritanceSystem : MonoBehaviour
{
    public static TraitInheritanceSystem Instance { get; private set; }

    [Header("Trait Count")]
    public int minTraitsPerFish = 1;
    public int maxTraitsPerFish = 2;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Call this when a new fish is spawned (no parents)
    public void AssignRandomTraits(Fish fish)
    {
        if (fish == null || TraitsDatabase.Instance == null) return;

        fish.traits.Clear();
        int count = Random.Range(minTraitsPerFish, maxTraitsPerFish + 1);

        for (int i = 0; i < count; i++)
        {
            Trait t = TraitsDatabase.Instance.GetRandomTraitWeighted();
            if (t != null && !fish.traits.Contains(t))
                fish.traits.Add(t);
        }

        // set fish rarity based on best trait
        UpdateFishRarityFromTraits(fish);
    }

    // Call this when breeding
    public List<Trait> InheritTraits(Fish parentA, Fish parentB)
    {
        List<Trait> result = new List<Trait>();

        if (parentA != null && parentA.traits != null)
        {
            for (int i = 0; i < parentA.traits.Count; i++)
            {
                if (parentA.traits[i] != null && !result.Contains(parentA.traits[i]))
                    result.Add(parentA.traits[i]);
            }
        }

        if (parentB != null && parentB.traits != null)
        {
            for (int i = 0; i < parentB.traits.Count; i++)
            {
                if (parentB.traits[i] != null && !result.Contains(parentB.traits[i]))
                    result.Add(parentB.traits[i]);
            }
        }

        // Ensure 1–2 traits only
        while (result.Count > maxTraitsPerFish)
        {
            int idx = Random.Range(0, result.Count);
            result.RemoveAt(idx);
        }

        return result;
    }

    public void UpdateFishRarityFromTraits(Fish fish)
    {
        if (fish == null) return;
        if (fish.traits == null || fish.traits.Count == 0)
        {
            fish.rarity = FishRarity.Common;
            return;
        }

        TraitRarity best = TraitRarity.Common;
        for (int i = 0; i < fish.traits.Count; i++)
        {
            Trait t = fish.traits[i];
            if (t == null) continue;
            if ((int)t.rarity > (int)best)
                best = t.rarity;
        }

        // map TraitRarity → FishRarity
        fish.rarity = (FishRarity)best;
    }
}
