using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterHealthController : MonoBehaviour
{
    public static WaterHealthController Instance { get; private set; }

    [Header("UI")]
    public TMP_Text waterHealthText;   // assign: UIRoot/HealthGroup/waterHealthText
    public Image healthFillImage;      // assign: .../waterHealthBar/Fill Area/Fill

    [Header("Tint (assign ONE of these)")]
    public SpriteRenderer waterTint;   // world-space option
    public Image waterTintUI;          // UI option (Canvas)

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;    // shown in UI (rounded)

    // precise float we actually mutate each frame
    float _healthF;

    [Header("Decay (per second)")]
    [Tooltip("Base passive decay per second (e.g., 0.02 means ~2 HP per 100s).")]
    public float baseDecayPerSecond = 0.02f;

    [Tooltip("Extra decay per second for each fish in the tank (e.g., 0.01).")]
    public float decayPerFish = 0.01f;

    [Header("On Fish Death")]
    [Tooltip("Instant HP penalty applied when a fish dies.")]
    public int deathPenalty = 15;

    [Header("Medicine")]
    [Tooltip("How much HP is restored when using medicine.")]
    public int medicineHeal = 35;

    [Tooltip("Gold cost to use medicine once.")]
    public int medicineCost = 50;

    [Header("Filter")]
    [Tooltip("Starting filter level (>=1). Level 1 = no reduction.")]
    public int filterLevel = 1;

    [Tooltip("Each extra level reduces total decay by this fraction (e.g., 0.25 = 25%).")]
    public float filterReductionPerLevel = 0.25f;

    int fishCount;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _healthF = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentHealth = Mathf.RoundToInt(_healthF);
        RefreshUI();
    }

    void Update()
    {
        // compute decay per second
        float decay = Mathf.Max(0f, baseDecayPerSecond + fishCount * decayPerFish);

        // multiplicative reduction from filter (level 1 = no reduction)
        float reduction = 1f;
        if (filterLevel > 1)
        {
            // example: level 2 => (1 - r), level 3 => (1 - r)^2 ...
            float perLevel = Mathf.Clamp01(filterReductionPerLevel);
            reduction = Mathf.Pow(1f - perLevel, filterLevel - 1);
        }
        // never reduce below 20% so water still gets dirty eventually
        decay *= Mathf.Clamp(reduction, 0.2f, 1f);

        // smooth decay over time
        _healthF -= decay * Time.deltaTime;
        _healthF = Mathf.Clamp(_healthF, 0f, maxHealth);
        currentHealth = Mathf.RoundToInt(_healthF);

        RefreshUI();
    }

    // Call this whenever fish count changes (e.g., on buy/sell/spawn/despawn).
    public void RegisterFishCount(int count)
    {
        fishCount = Mathf.Max(0, count);
    }

    // Call when a fish dies.
    public void NotifyFishDied()
    {
        _healthF = Mathf.Clamp(_healthF - deathPenalty, 0f, maxHealth);
        currentHealth = Mathf.RoundToInt(_healthF);
        RefreshUI();
    }

    public void SetFilterLevel(int level)
    {
        filterLevel = Mathf.Max(1, level);
    }

    // Invoke from PlayerToolController when Medicine tool is used.
    public void UseMedicine()
    {
        if (GameController.Instance && GameController.Instance.TrySpendGold(medicineCost))
        {
            _healthF = Mathf.Clamp(_healthF + medicineHeal, 0f, maxHealth);
            currentHealth = Mathf.RoundToInt(_healthF);
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        // text
        if (waterHealthText)
            waterHealthText.text = $"Water: {currentHealth}/{maxHealth}";

        // bar
        if (healthFillImage)
        {
            float ratio = maxHealth > 0 ? Mathf.Clamp01(_healthF / maxHealth) : 0f;
            healthFillImage.fillAmount = ratio;
        }

        // tint color (more vivid green; goes full toxic >90% dirty)
        float ratioClean = maxHealth > 0 ? Mathf.Clamp01(_healthF / maxHealth) : 0f;
        float dirt = 1f - ratioClean; // 0=clean, 1=dirty
        Color tintColor;

        if (dirt < 0.9f)
        {
            float t = dirt / 0.9f;
            // vivid green with alpha ramp
            tintColor = Color.Lerp(
                new Color(0f, 0f, 0f, 0f),
                new Color(0.0f, 0.95f, 0.0f, Mathf.Lerp(0.15f, 0.6f, t)),
                t
            );
        }
        else
        {
            // fully dirty: strong, slightly thicker green (add a touch of yellow if you like)
            tintColor = new Color(0.0f, 1.0f, 0.0f, 0.65f);
        }

        if (waterTint) waterTint.color = tintColor;
        if (waterTintUI) waterTintUI.color = tintColor;
    }
}
