using UnityEngine;

public enum DecorationBonusType
{
    HungerDrainMultiplier,      // < 1 = slower hunger drain
    HealthRegenMultiplier,      // > 1 = faster regen
    BreedChanceMultiplier,      // > 1 = easier breeding
    SellPriceMultiplier,        // > 1 = more gold when selling
    WaterHealthRegenMultiplier  // > 1 = faster water recovery
}

public class Decoration : MonoBehaviour
{
    public DecorationBonusType bonusType = DecorationBonusType.HungerDrainMultiplier;
    public float bonusValue = 1.1f; // 1.1 = +10%, 0.9 = -10%

    void OnEnable()
    {
        DecorationManager.Register(this);
    }

    void OnDisable()
    {
        DecorationManager.Unregister(this);
    }
}
