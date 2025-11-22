using UnityEngine;

public enum EggRarity
{
    Common,
    Rare,
    Legendary
}

public class FishEgg : MonoBehaviour
{
    [Header("Setup")]
    public EggRarity rarity = EggRarity.Common;
    public SpriteRenderer sr;
    public Sprite commonSprite;
    public Sprite rareSprite;
    public Sprite legendarySprite;

    [Header("Hatching")]
    public float commonHatchTime = 20f;
    public float rareHatchTime = 45f;
    public float legendaryHatchTime = 90f;

    [Header("Floating")]
    [Tooltip("How fast the egg drifts downwards.")]
    public float floatSpeed = 0.4f;

    [Tooltip("World-space Y position of the tank bottom. Adjust in Inspector to match your tank.")]
    public float bottomY = -3.5f;

    float hatchTimer;
    bool isHatching = true;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();

        ApplySprite();
        hatchTimer = GetHatchTimeForRarity();
    }

    void Update()
    {
        if (!isHatching) return;

        // Drift down towards the bottom
        FloatDown();

        // Hatching timer
        hatchTimer -= Time.deltaTime;
        if (hatchTimer <= 0f)
        {
            Hatch();
        }
    }

    void FloatDown()
    {
        Vector3 pos = transform.position;

        // Only move down while above the bottom
        if (pos.y > bottomY)
        {
            pos.y -= floatSpeed * Time.deltaTime;
            transform.position = pos;
        }
    }

    float GetHatchTimeForRarity()
    {
        switch (rarity)
        {
            case EggRarity.Common: return commonHatchTime;
            case EggRarity.Rare: return rareHatchTime;
            case EggRarity.Legendary: return legendaryHatchTime;
            default: return commonHatchTime;
        }
    }

    void ApplySprite()
    {
        if (!sr) return;

        switch (rarity)
        {
            case EggRarity.Common:
                sr.sprite = commonSprite;
                break;
            case EggRarity.Rare:
                sr.sprite = rareSprite;
                break;
            case EggRarity.Legendary:
                sr.sprite = legendarySprite;
                break;
        }
    }

    void Hatch()
    {
        isHatching = false;

        if (GameController.Instance != null)
        {
            GameController.Instance.SpawnFishFromEgg(rarity, transform.position);
        }

        Destroy(gameObject);
    }
}
