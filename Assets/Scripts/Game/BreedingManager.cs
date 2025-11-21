using System.Collections.Generic;
using UnityEngine;

public class BreedingManager : MonoBehaviour
{
    public GameController gameController;
    public float checkInterval = 5f;
    public float maxDistance = 1.5f;
    public float minAgeSeconds = 20f;
    public float minHealth = 0.6f;
    public float minHunger = 0.5f;
    public float globalBreedMultiplier = 0.5f;
    public int maxFishInTank = 30;

    float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = checkInterval;
            TryBreed();
        }
    }

    void TryBreed()
    {
        Fish[] fishArray = FindObjectsOfType<Fish>();
        if (fishArray == null || fishArray.Length == 0) return;

        if (fishArray.Length >= maxFishInTank) return;

        List<Fish> list = new List<Fish>();
        for (int i = 0; i < fishArray.Length; i++)
        {
            if (fishArray[i] != null) list.Add(fishArray[i]);
        }

        int count = list.Count;
        if (count < 2) return;

        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                Fish a = list[i];
                Fish b = list[j];

                if (a == null || b == null) continue;
                if (a == b) continue;

                if (!a.CanBreed() || !b.CanBreed()) continue;

                if (a.ageSeconds < minAgeSeconds || b.ageSeconds < minAgeSeconds) continue;
                if (a.health01 < minHealth || b.health01 < minHealth) continue;
                if (a.Hunger01 < minHunger || b.Hunger01 < minHunger) continue;

                float dist = Vector2.Distance(a.transform.position, b.transform.position);
                if (dist > maxDistance) continue;

                if (a.sex == b.sex) continue;

                float baseChance = (a.baseBreedChance * a.GetBreedChanceMultiplier() +
                                    b.baseBreedChance * b.GetBreedChanceMultiplier()) * 0.5f;

                float chance = baseChance * globalBreedMultiplier;
                if (chance <= 0f) continue;
                if (chance > 1f) chance = 1f;

                float r = Random.value;
                if (r <= chance)
                {
                    Vector3 pos = (a.transform.position + b.transform.position) * 0.5f;
                    SpawnChild(a, b, pos);
                    return;
                }
            }
        }
    }

    void SpawnChild(Fish a, Fish b, Vector3 pos)
    {
        if (a == null || b == null) return;

        string breedId = a.breedId;
        string displayName = a.breedDisplayName;

        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
        }

        Fish child = null;

        if (gameController != null)
        {
            child = gameController.SpawnFishByBreedId(breedId, pos);
        }

        if (child != null)
        {
            child.ageSeconds = 0f;
            child.hunger = 1f;
            child.health = 1f;
            child.fishName = "";
            child.breedId = breedId;
            child.breedDisplayName = displayName;

            child.isBaby = true;
            child.transform.localScale = Vector3.one * child.babyScale;

            if (child.sr != null && child.babySprite != null)
            {
                child.sr.sprite = child.babySprite;
            }

            child.traits = TraitInheritanceSystem.InheritTraits(a, b);
        }

        a.TriggerBreedCooldown();
        b.TriggerBreedCooldown();
    }
}
