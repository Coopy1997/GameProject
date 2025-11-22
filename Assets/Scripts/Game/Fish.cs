using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Identity")]
    public string breedId;
    public string breedDisplayName = "Fish";
    public Sprite icon;

    [Header("Naming")]
    [SerializeField] private string customName = "";

    [Header("Growth")]
    public bool isBaby = true;
    public float babyDuration = 10f;  // seconds to mature
    private float ageSeconds = 0f;
    public float babyScale = 0.35f;
    public float adultScale = 1.0f;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float health = 100f;
    public float maxHunger = 100f;
    public float hunger = 100f;

    [Header("Death Settings")]
    public bool isDead = false;
    public float floatSpeed = 0.4f;
    private SpriteRenderer sr;

    [Header("Traits")]
    public List<Trait> traits = new List<Trait>();
    public FishRarity rarity = FishRarity.Common;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // start as baby
        if (isBaby)
            transform.localScale = Vector3.one * babyScale;
        else
            transform.localScale = Vector3.one * adultScale;
    }

    void Update()
    {
        if (isDead)
        {
            FloatDeadFish();
            return;
        }

        AgeUpdate();
        HungerUpdate();
        HealthUpdate();
    }

    // ============================================================
    // AGE & GROWTH
    // ============================================================
    void AgeUpdate()
    {
        ageSeconds += Time.deltaTime;

        if (isBaby && ageSeconds >= babyDuration)
        {
            Mature();
        }
    }

    public void MakeBaby()
    {
        isBaby = true;
        ageSeconds = 0f;
        transform.localScale = Vector3.one * babyScale;
    }

    void Mature()
    {
        isBaby = false;
        transform.localScale = Vector3.one * adultScale;
    }

    public string GetAgeString()
    {
        int seconds = Mathf.FloorToInt(ageSeconds);
        if (isBaby) return $"Baby ({seconds}s)";
        return $"{seconds}s";
    }

    // ============================================================
    // HUNGER & HEALTH
    // ============================================================
    void HungerUpdate()
    {
        // base decay
        hunger -= Time.deltaTime * 2f;

        // trait modifiers
        if (traits != null)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                Trait t = traits[i];
                if (t == null) continue;
                hunger -= Time.deltaTime * t.hungerDrainModifier;
            }
        }

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);

        // starve damage
        if (hunger <= 0f)
        {
            health -= Time.deltaTime * 3f;
        }
    }

    void HealthUpdate()
    {
        if (health <= 0f && !isDead)
        {
            Die();
        }
    }

    public float GetHunger01()
    {
        if (maxHunger <= 0f) return 0f;
        return Mathf.Clamp01(hunger / maxHunger);
    }

    public float GetHealth01()
    {
        if (maxHealth <= 0f) return 0f;
        return Mathf.Clamp01(health / maxHealth);
    }

    public void Feed(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        health -= dmg;
        if (health <= 0f) Die();
    }

    // ============================================================
    // DEATH BEHAVIOR
    // ============================================================
    void Die()
    {
        isDead = true;

        if (sr != null)
            sr.color = new Color(0.5f, 1f, 0.5f, 1f); // greenish dead tint

        transform.rotation = Quaternion.Euler(0, 0, 180); // upside down
    }

    void FloatDeadFish()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    // ============================================================
    // NAME / DISPLAY
    // ============================================================
    public string GetDisplayName()
    {
        if (string.IsNullOrWhiteSpace(customName))
            return breedDisplayName;
        return customName;
    }

    public void SetCustomName(string name)
    {
        customName = name;
    }

    // ============================================================
    // VALUE / RARITY
    // ============================================================
    public int GetSellValue()
    {
        int baseValue = 10;

        // size multiplier (adult bigger = more expensive)
        float sizeMult = Mathf.Lerp(0.5f, 2f, adultScale);

        // rarity multiplier
        float rarityMult = 1f;
        switch (rarity)
        {
            case FishRarity.Uncommon: rarityMult = 1.3f; break;
            case FishRarity.Rare: rarityMult = 1.7f; break;
            case FishRarity.Epic: rarityMult = 2.5f; break;
            case FishRarity.Legendary: rarityMult = 20f; break;
            default: rarityMult = 1f; break;
        }

        // trait multiplier
        float traitMult = 1f;
        if (traits != null)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                Trait t = traits[i];
                if (t == null) continue;
                traitMult += t.sellPriceMultiplier;
            }
        }

        int value = Mathf.RoundToInt(baseValue * sizeMult * rarityMult * traitMult);
        return Mathf.Max(value, 0);
    }

    public string GetRarityColor()
    {
        switch (rarity)
        {
            case FishRarity.Uncommon: return "#6BFF83";
            case FishRarity.Rare: return "#7D7BFF";
            case FishRarity.Epic: return "#FF4DF2";
            case FishRarity.Legendary: return "#FFB300";
            default: return "#FFFFFF";
        }
    }

    public string GetFormattedTraitList()
    {
        if (traits == null || traits.Count == 0)
            return "<i>No Traits</i>";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < traits.Count; i++)
        {
            Trait t = traits[i];
            if (t == null) continue;
            string color = t.GetColorHex();
            sb.AppendLine($"• <color={color}>{t.traitName}</color>");
        }
        return sb.ToString().TrimEnd();
    }
}
