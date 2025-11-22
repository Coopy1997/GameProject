using System.Collections.Generic;
using UnityEngine;

public enum FishSex
{
    Male,
    Female
}

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
    Drab,
    AncientSpirit
}

public enum TraitRarity
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
    public TraitType type;
    public string name;
    public string description;
    public TraitRarity rarity;

    public Trait(TraitType type, string name, string description, TraitRarity rarity)
    {
        this.type = type;
        this.name = name;
        this.description = description;
        this.rarity = rarity;
    }
}

public class Fish : MonoBehaviour
{
    [Header("Identity")]
    public string fishName = "Fish";
    public string breedId = "DefaultFish";
    public string breedDisplayName = "Default Fish";
    public FishSex sex = FishSex.Male;

    [Header("Age")]
    public float ageSeconds = 0f;

    [Header("Hunger & Health")]
    public float hunger = 1f;          // 0..1
    public float health = 1f;          // 0..1
    public float hungerDrainPerSecond = 0.05f;

    public float Hunger01 => Mathf.Clamp01(hunger);
    public float health01 => Mathf.Clamp01(health);

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

    [Header("Growth (Baby → Adult)")]
    public bool isBaby = true;
    public float babyDuration = 20f;
    public float babyScale = 0.15f;      // was 0.4
    public float adultScale = 1f;        // overridden at runtime
    public float adultScaleMin = 0.6f;   // was 0.8
    public float adultScaleMax = 1.0f;   // was 1.2
    float babyAge = 0f;
    public Sprite babySprite;
    public Sprite adultSprite;

    [Header("Visuals")]
    public SpriteRenderer sr;
    public GameObject sparklePrefab;

    [Header("Death")]
    public bool isDead = false;
    public float floatSpeed = 0.5f;
    public Color deadColor = new Color(0.5f, 1f, 0.5f, 1f);

    [Header("Value")]
    public int baseSellValue = 50;       // default base value if catalog not used
    public float breedValueMultiplier = 1f;  // set from FishCatalogItem
    float sizeValueMultiplier = 1f;      // calculated from adultScale

    TankBounds tank;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        tank = FindObjectOfType<TankBounds>();

        if (traits == null || traits.Count == 0)
        {
            traits = TraitDatabase.GetRandomTraits(1, 2);
        }

        sex = (Random.value < 0.5f) ? FishSex.Male : FishSex.Female;

        if (adultSprite == null && sr != null)
            adultSprite = sr.sprite;

        // randomize adult size for this fish
        adultScale = Random.Range(adultScaleMin, adultScaleMax);
        sizeValueMultiplier = GetSizeValueMultiplierInternal();

        MakeBaby();
        CheckLegendarySparkle();
        PickNewDirection();
    }

    void Update()
    {
        if (isDead)
        {
            UpdateDeadFloat();
            return;
        }

        float ageMul = GetAgeSpeedMultiplier();
        ageSeconds += Time.deltaTime * ageMul;

        UpdateBabyGrowth();
        UpdateHunger();
        UpdateHealth();
        UpdateBreedingTimer();
        UpdateMovement();
    }

    // ---------------- BABY / GROWTH ----------------

    public void MakeBaby()
    {
        isBaby = true;
        babyAge = 0f;
        ageSeconds = 0f;

        if (sr == null) sr = GetComponent<SpriteRenderer>();

        transform.localScale = Vector3.one * babyScale;

        if (sr != null && babySprite != null)
            sr.sprite = babySprite;
    }

    void Mature()
    {
        isBaby = false;

        if (sr != null && adultSprite != null)
            sr.sprite = adultSprite;

        transform.localScale = Vector3.one * adultScale;
    }

    void UpdateBabyGrowth()
    {
        if (!isBaby) return;

        babyAge += Time.deltaTime;
        float t = 0f;
        if (babyDuration > 0f) t = Mathf.Clamp01(babyAge / babyDuration);

        float scale = Mathf.Lerp(babyScale, adultScale, t);
        transform.localScale = Vector3.one * scale;

        if (babyAge >= babyDuration)
            Mature();
    }

    // ---------------- BREEDING ----------------

    void UpdateBreedingTimer()
    {
        if (breedTimer > 0f) breedTimer -= Time.deltaTime;
    }

    public bool CanBreed()
    {
        if (isBaby) return false;
        return canBreed && breedTimer <= 0f;
    }

    public void TriggerBreedCooldown()
    {
        breedTimer = breedCooldown;
    }

    // ---------------- HUNGER & HEALTH ----------------

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
            water = WaterHealthController.Instance.currentHealth;

        if (HasTrait(TraitType.CleanFreak) && water > 0.8f)
            health += 0.03f * Time.deltaTime;

        if (!HasTrait(TraitType.DirtyScavenger) && water < 0.3f)
            health -= 0.03f * badLossMul * Time.deltaTime;

        if (HasTrait(TraitType.Drab))
            health -= 0.01f * Time.deltaTime;

        if (HasTrait(TraitType.Mutated))
            health -= 0.01f * Time.deltaTime;

        health = Mathf.Clamp01(health);
        if (health <= 0f) Die();
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
        if (isDead) return;
        isDead = true;

        if (sr != null)
        {
            sr.color = deadColor;
            sr.flipY = true;
        }

        hungerDrainPerSecond = 0f;
        speed = 0f;
    }

    // ---------------- MOVEMENT ----------------

    void UpdateMovement()
    {
        if (!tank) tank = FindObjectOfType<TankBounds>();
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
            targetDir.x *= -1;

        if (pos.y < min.y || pos.y > max.y)
            targetDir.y *= -1;
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        targetDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
    }

    // ---------------- DEAD FLOAT ----------------

    void UpdateDeadFloat()
    {
        if (tank == null)
            tank = FindObjectOfType<TankBounds>();
        if (tank == null) return;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (transform.position.y > tank.max.y - 0.1f)
        {
            Vector3 p = transform.position;
            p.y = tank.max.y - 0.1f;
            transform.position = p;
        }
    }

    void OnMouseDown()
    {
        if (!isDead) return;

        if (GameController.Instance != null)
            GameController.Instance.UpdateFishCount();

        Destroy(gameObject);
    }

    // ---------------- TRAIT HELPERS ----------------

    public bool HasTrait(TraitType t)
    {
        if (traits == null) return false;
        for (int i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];
            if (trait != null && trait.type == t) return true;
        }
        return false;
    }

    Dictionary<string, int> GetTraitCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        if (traits == null) return counts;

        for (int i = 0; i < traits.Count; i++)
        {
            Trait t = traits[i];
            if (t == null) continue;
            string n = t.name;
            if (!counts.ContainsKey(n)) counts[n] = 0;
            counts[n]++;
        }
        return counts;
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

        var counts = GetTraitCounts();
        foreach (var kv in counts)
        {
            if (kv.Value >= 2)
            {
                if (kv.Key == "Hungry" || kv.Key == "Glutton" || kv.Key == "Energetic")
                    m *= 1.2f;
                if (kv.Key == "Peckish" || kv.Key == "Slow Metabolism" || kv.Key == "Calm")
                    m *= 0.8f;
            }
        }

        if (DecorationManager.Instance != null)
            m *= DecorationManager.Instance.HungerDrainMultiplier;

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

        if (DecorationManager.Instance != null)
            m *= DecorationManager.Instance.HealthRegenMultiplier;

        return m;
    }

    public float GetBreedChanceMultiplier()
    {
        float m = 1f;
        if (HasTrait(TraitType.Breeder)) m *= 1.25f;
        if (HasTrait(TraitType.Shy)) m *= 0.8f;
        if (HasTrait(TraitType.Mutated)) m *= 1.1f;
        if (HasTrait(TraitType.Lucky)) m *= 1.1f;

        var counts = GetTraitCounts();
        foreach (var kv in counts)
        {
            if (kv.Value >= 2)
            {
                if (kv.Key == "Breeder" || kv.Key == "Lucky")
                    m *= 1.5f;
            }
        }

        if (DecorationManager.Instance != null)
            m *= DecorationManager.Instance.BreedChanceMultiplier;

        return m;
    }

    public float GetSellPriceMultiplier()
    {
        float mult = 1f;

        for (int i = 0; i < traits.Count; i++)
        {
            Trait t = traits[i];
            if (t == null) continue;

            switch (t.rarity)
            {
                case TraitRarity.Common: mult += 0f; break;
                case TraitRarity.Uncommon: mult += 0.1f; break;
                case TraitRarity.Rare: mult += 0.25f; break;
                case TraitRarity.Epic: mult += 0.5f; break;
                case TraitRarity.Legendary: mult *= 20f; break;
            }

            if (t.type == TraitType.JewelScales)
                mult *= 1.25f;
            if (t.type == TraitType.Drab)
                mult *= 0.8f;
        }

        var counts = GetTraitCounts();
        foreach (var kv in counts)
        {
            if (kv.Value >= 2)
                mult *= 1.5f;
        }

        if (DecorationManager.Instance != null)
            mult *= DecorationManager.Instance.SellPriceMultiplier;

        return mult;
    }

    float GetSizeValueMultiplierInternal()
    {
        // Map adultScale 0.8→1.2 to value 0.8x→1.3x
        float t = Mathf.InverseLerp(adultScaleMin, adultScaleMax, adultScale);
        return Mathf.Lerp(0.8f, 1.3f, t);
    }

    public float GetSizeValueMultiplier()
    {
        return sizeValueMultiplier;
    }

    // FINAL SELL PRICE
    public int GetSellPrice()
    {
        float baseVal = baseSellValue * breedValueMultiplier;
        float result = baseVal;
        result *= GetSellPriceMultiplier();
        result *= GetSizeValueMultiplier();

        if (health01 < 0.5f) result *= 0.7f;   // sick fish sell cheaper
        if (isBaby) result *= 0.5f;           // babies are cheaper than adults

        if (result < 0f) result = 0f;
        return Mathf.RoundToInt(result);
    }

    void CheckLegendarySparkle()
    {
        if (sparklePrefab == null || traits == null) return;

        for (int i = 0; i < traits.Count; i++)
        {
            Trait t = traits[i];
            if (t == null) continue;
            if (t.rarity == TraitRarity.Legendary)
            {
                GameObject s = Instantiate(sparklePrefab, transform);
                s.transform.localPosition = Vector3.zero;
                break;
            }
        }
    }
}
