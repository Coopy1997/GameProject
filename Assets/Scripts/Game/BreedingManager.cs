using System.Collections.Generic;
using UnityEngine;

public class BreedingManager : MonoBehaviour
{
    public float checkInterval = 5f;
    public float minWaterRatio = 0.6f;

    float t;

    void Update()
    {
        t -= Time.deltaTime;
        if (t <= 0f)
        {
            t = checkInterval;
            TryBreedOnce();
        }
    }

    void TryBreedOnce()
    {
        if (!GameController.Instance) return;
        var gc = GameController.Instance;

        if (gc.allFish.Count >= gc.maxFish) return;

        var wh = WaterHealthController.Instance;
        if (wh)
        {
            float ratio = wh.maxHealth > 0 ? (float)wh.currentHealth / wh.maxHealth : 0f;
            if (ratio < minWaterRatio) return;
        }

        List<Fish> candidates = new List<Fish>();
        for (int i = 0; i < gc.allFish.Count; i++)
        {
            var f = gc.allFish[i];
            if (f == null) continue;
            if (!f.CanBreed()) continue;
            candidates.Add(f);
        }

        if (candidates.Count < 2) return;

        var parentA = candidates[Random.Range(0, candidates.Count)];
        var parentB = candidates[Random.Range(0, candidates.Count)];
        if (parentA == parentB) return;

        string childId = GetChildBreedId(parentA.breedId, parentB.breedId);

        float baseChance = (parentA.baseBreedChance + parentB.baseBreedChance) * 0.5f;
        float extraChance = GetExtraComboChance(parentA.breedId, parentB.breedId);
        float chance = Mathf.Clamp01(baseChance + extraChance);

        float roll = Random.value;
        if (roll > chance) return;

        var baby = gc.SpawnFishByBreedId(childId);
        if (baby != null)
        {
            parentA.breedTimer = parentA.breedCooldown;
            parentB.breedTimer = parentB.breedCooldown;
        }
    }

    string GetChildBreedId(string a, string b)
    {
        if ((a == "SunnyGuppy" && b == "SunnyGuppy") ||
            (a == "SunnyGuppy" && b == "StripefinMolly") ||
            (a == "StripefinMolly" && b == "SunnyGuppy"))
        {
            if (a == "SunnyGuppy" && b == "SunnyGuppy") return "StripefinMolly";
            if ((a == "SunnyGuppy" && b == "StripefinMolly") ||
                (a == "StripefinMolly" && b == "SunnyGuppy")) return "MoonTetra";
        }

        if ((a == "StripefinMolly" && b == "MoonTetra") ||
            (a == "MoonTetra" && b == "StripefinMolly"))
            return "RoyalAngel";

        if ((a == "MoonTetra" && b == "RoyalAngel") ||
            (a == "RoyalAngel" && b == "MoonTetra"))
            return "CelestialKoi";

        return a;
    }

    float GetExtraComboChance(string a, string b)
    {
        if (a == "SunnyGuppy" && b == "SunnyGuppy") return 0.4f;

        if ((a == "SunnyGuppy" && b == "StripefinMolly") ||
            (a == "StripefinMolly" && b == "SunnyGuppy"))
            return 0.5f;

        if ((a == "StripefinMolly" && b == "MoonTetra") ||
            (a == "MoonTetra" && b == "StripefinMolly"))
            return 0.6f;

        if ((a == "MoonTetra" && b == "RoyalAngel") ||
            (a == "RoyalAngel" && b == "MoonTetra"))
            return 0.75f;

        return 0f;
    }
}
