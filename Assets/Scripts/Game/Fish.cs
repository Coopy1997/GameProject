using System.Collections.Generic;
using UnityEngine;

public enum FishSex
{
    Male,
    Female
}

public class Fish : MonoBehaviour
{
    [Header("Identity")]
    public string fishName = "Fish";
    public string breedId = "SunnyGuppy";
    public string breedDisplayName = "Sunny Guppy";
    public FishSex sex = FishSex.Male;

    [Header("Age")]
    public float ageSeconds = 0f;

    [Header("Hunger & Health")]
    public float hunger = 1f;
    public float health = 1f;
    public float hungerDrainPerSecond = 0.05f;

    public float Hunger01
    {
        get { return Mathf.Clamp01(hunger); }
    }

    public float health01
    {
        get { return Mathf.Clamp01(health); }
    }

    [Header("Movement")]
    public float speed = 1.5f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 4f;

    Vector2 targetDir;
    float idleTimer;

    [Header("Traits")]
    public List<Trait> traits = new List<Trait>();

    [Header("Breeding")]
    public bool canBreed = true;
    public float baseBreedChance = 0.5f;
    public float breedCooldown = 30f;
    public float breedTimer = 0f;

    [Header("References")]
    public SpriteRenderer sr;
    TankBounds tank;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        tank = FindObjectOfType<TankBounds>();
        PickNewDirection();

        if (traits == null || traits.Count == 0)
        {
            traits = TraitDatabase.GetRandomTraits(1, 3);
        }

        if (UnityEngine.Random.value < 0.5f) sex = FishSex.Male;
        else sex = FishSex.Female;
    }

    void Update()
    {
        float ageMul = GetAgeSpeedMultiplier();
        ageSeconds += Time.deltaTime * ageMul;

        UpdateHunger();
        UpdateHealth();
        UpdateBreedingTimer();
        UpdateMovement();
    }

    void UpdateBreedingTimer()
    {
        if (breedTimer > 0f) breedTimer -= Time.deltaTime;
    }

    void UpdateHunger()
    {
        float drain = hungerDrainPerSecond;
        drain *= GetHungerDrainMultiplier();
        hunger -= drain * Time.deltaTime;
        hunger = Mathf.Clamp01(hunger);
    }

    void UpdateHealth()
    {
        float badLossMul = GetBadHealthLossMultiplier();
        float goodGainMul = GetGoodHealthGainMultiplier();

        if (hunger < 0.25f)
        {
            float loss = (0.25f - hunger) * 0.1f * badLossMul;
            health -= loss * Time.deltaTime;
        }

        if (hunger > 0.75f)
        {
            float gain = (hunger - 0.75f) * 0.05f * goodGainMul;
            health += gain * Time.deltaTime;
        }

        float water = 1f;
        if (WaterHealthController.Instance != null)
        {
            water = WaterHealthController.Instance.currentHealth;
        }

        if (HasTrait(TraitType.CleanFreak) && water > 0.8f)
        {
            health += 0.03f * Time.deltaTime;
        }

        if (!HasTrait(TraitType.DirtyScavenger) && water < 0.3f)
        {
            health -= 0.03f * badLossMul * Time.deltaTime;
        }

        if (HasTrait(TraitType.Drab))
        {
            health -= 0.01f * Time.deltaTime;
        }

        if (HasTrait(TraitType.Mutated))
        {
            health -= 0.01f * Time.deltaTime;
        }

        health = Mathf.Clamp01(health);

        if (health <= 0f) Die();
    }

    void UpdateMovement()
    {
        if (!tank) return;

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f) PickNewDirection();

        float moveSpeed = speed * GetSpeedMultiplier();
        transform.position += (Vector3)(targetDir * moveSpeed * Time.deltaTime);

        if (sr) sr.flipX = targetDir.x < 0;

        Vector2 min = tank.min;
        Vector2 max = tank.max;
        Vector3 pos = transform.position;

        if (pos.x < min.x || pos.x > max.x)
        {
            targetDir.x *= -1;
        }

        if (pos.y < min.y || pos.y > max.y)
        {
            targetDir.y *= -1;
        }
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        targetDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
    }

    public void Eat(float amount)
    {
        float mul = GetEatAmountMultiplier();
        hunger += amount * mul;
        hunger = Mathf.Clamp01(hunger);
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp01(health);
    }

    public void ApplyDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp01(health);
        if (health <= 0f) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public bool CanBreed()
    {
        return canBreed && breedTimer <= 0f;
    }

    public void TriggerBreedCooldown()
    {
        breedTimer = breedCooldown;
    }

    public bool HasTrait(TraitType t)
    {
        if (traits == null) return false;
        for (int i = 0; i < traits.Count; i++)
        {
            if (traits[i] != null && traits[i].type == t) return true;
        }
        return false;
    }

    public float GetHungerDrainMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Hungry)) m *= 1.25f;
        if (HasTrait(TraitType.Peckish)) m *= 0.8f;
        if (HasTrait(TraitType.Glutton)) m *= 1.35f;
        if (HasTrait(TraitType.SlowMetabolism)) m *= 0.6f;
        if (HasTrait(TraitType.Calm)) m *= 0.85f;
        if (HasTrait(TraitType.Energetic)) m *= 1.2f;
        return m;
    }

    public float GetEatAmountMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Peckish)) m *= 0.8f;
        if (HasTrait(TraitType.Glutton)) m *= 1.2f;
        return m;
    }

    public float GetSpeedMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Calm)) m *= 0.8f;
        if (HasTrait(TraitType.Energetic)) m *= 1.25f;
        return m;
    }

    public float GetAgeSpeedMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.RapidGrowth)) m *= 1.5f;
        if (HasTrait(TraitType.SlowGrowth)) m *= 0.6f;
        return m;
    }

    public float GetBadHealthLossMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Hardy)) m *= 0.5f;
        if (HasTrait(TraitType.Fragile)) m *= 1.3f;
        return m;
    }

    public float GetGoodHealthGainMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Drab)) m *= 0.8f;
        return m;
    }

    public float GetBreedChanceMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Breeder)) m *= 1.25f;
        if (HasTrait(TraitType.Shy)) m *= 0.8f;
        if (HasTrait(TraitType.Mutated)) m *= 1.1f;
        if (HasTrait(TraitType.Lucky)) m *= 1.1f;
        return m;
    }

    public float GetSellPriceMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.JewelScales)) m *= 1.25f;
        if (HasTrait(TraitType.Drab)) m *= 0.8f;
        return m;
    }
}
