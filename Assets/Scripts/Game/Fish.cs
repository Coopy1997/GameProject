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
    public float hunger = 1f;                  // 0–1
    public float health = 1f;                  // 0–1
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
    public string[] traits;

    [Header("Breeding")]
    public bool canBreed = true;
    public float baseBreedChance = 0.5f;
    public float breedCooldown = 30f;
    public float breedTimer = 0f;

    public bool CanBreed()
    {
        return canBreed && breedTimer <= 0f;
    }

    [Header("References")]
    public SpriteRenderer sr;
    TankBounds tank;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        tank = FindObjectOfType<TankBounds>();
        PickNewDirection();
    }

    void Update()
    {
        ageSeconds += Time.deltaTime;

        UpdateHunger();
        UpdateHealth();
        UpdateBreedingTimer();
        UpdateMovement();
    }

    void UpdateBreedingTimer()
    {
        if (breedTimer > 0f)
            breedTimer -= Time.deltaTime;
    }

    void UpdateHunger()
    {
        hunger -= hungerDrainPerSecond * Time.deltaTime;
        hunger = Mathf.Clamp01(hunger);
    }

    void UpdateHealth()
    {
        if (hunger < 0.25f)
            health -= (0.25f - hunger) * 0.1f * Time.deltaTime;

        if (hunger > 0.75f)
            health += (hunger - 0.75f) * 0.05f * Time.deltaTime;

        health = Mathf.Clamp01(health);

        if (health <= 0f)
            Die();
    }

    void UpdateMovement()
    {
        if (!tank) return;

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
            PickNewDirection();

        transform.position += (Vector3)(targetDir * speed * Time.deltaTime);

        if (sr)
            sr.flipX = targetDir.x < 0;

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

    public void Eat(float amount)
    {
        hunger += amount;
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
}
