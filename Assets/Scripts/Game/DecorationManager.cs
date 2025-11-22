using System.Collections.Generic;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    public static DecorationManager Instance;

    static readonly List<Decoration> decorations = new List<Decoration>();

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void Register(Decoration deco)
    {
        if (!deco) return;
        if (!decorations.Contains(deco))
            decorations.Add(deco);
    }

    public static void Unregister(Decoration deco)
    {
        if (!deco) return;
        decorations.Remove(deco);
    }

    float GetTotalMultiplier(DecorationBonusType type)
    {
        float mult = 1f;
        for (int i = 0; i < decorations.Count; i++)
        {
            var d = decorations[i];
            if (!d || d.bonusType != type) continue;
            mult *= d.bonusValue;
        }
        return mult;
    }

    public float HungerDrainMultiplier => GetTotalMultiplier(DecorationBonusType.HungerDrainMultiplier);
    public float HealthRegenMultiplier => GetTotalMultiplier(DecorationBonusType.HealthRegenMultiplier);
    public float BreedChanceMultiplier => GetTotalMultiplier(DecorationBonusType.BreedChanceMultiplier);
    public float SellPriceMultiplier => GetTotalMultiplier(DecorationBonusType.SellPriceMultiplier);
    public float WaterRegenMultiplier => GetTotalMultiplier(DecorationBonusType.WaterHealthRegenMultiplier);
}
