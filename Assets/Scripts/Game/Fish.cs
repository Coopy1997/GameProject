using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Fish : MonoBehaviour
{
    public float minSpeed = 1.0f;
    public float maxSpeed = 2.2f;

    public float foodSenseRadius = 3.5f;
    public float eatDistance = 0.65f;
    public LayerMask foodMask;

    public float verticalWanderStrength = 0.4f;
    public float verticalSeekStrength = 1.2f;
    public float maxVerticalSpeed = 0.8f;

    public float flipDeadzoneX = 0.35f;
    public float flipCooldown = 0.35f;

    public int maxHunger = 100;
    public float hungerPerSecond = 4f;
    public float fullTime = 50f;
    public HungerBar hungerBar;

    public float wallXPadding = 0.2f;
    public float wallYPadding = 0.3f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    TankBounds tank;

    float speed;
    int hunger;
    bool isFull;
    float fullTimer;
    int direction = 1; // +1 right, -1 left
    float wanderOffset;
    float nextFlipTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        tank = FindObjectOfType<TankBounds>();

        rb.gravityScale = 0f;
        rb.drag = 0.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        speed = Random.Range(minSpeed, maxSpeed);
        hunger = maxHunger / 2;
        wanderOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (isFull)
        {
            fullTimer -= Time.deltaTime;
            if (fullTimer <= 0f) isFull = false;
        }
        else
        {
            hunger -= Mathf.CeilToInt(hungerPerSecond * Time.deltaTime);
            if (hunger < 0) hunger = 0;
        }
        if (hungerBar) hungerBar.Set(hunger, maxHunger);
    }

    void FixedUpdate()
    {
        if (!tank) return;

        Vector2 pos = rb.position;

        Collider2D nearest = null;
        float best = float.MaxValue;
        int mask = (foodMask.value == 0) ? Physics2D.AllLayers : foodMask.value;

        if (!isFull)
        {
            var hits = Physics2D.OverlapCircleAll(pos, foodSenseRadius, mask);
            for (int i = 0; i < hits.Length; i++)
            {
                var h = hits[i];
                if (!h || !h.TryGetComponent<Food>(out _)) continue;
                float d = (h.transform.position - (Vector3)pos).sqrMagnitude;
                if (d < best) { best = d; nearest = h; }
            }
        }

        float vx = direction * speed;

        float vy = Mathf.Sin((Time.time + wanderOffset) * 0.8f) * verticalWanderStrength;
        if (nearest)
        {
            Vector2 toFood = (Vector2)nearest.transform.position - pos;
            float dx = toFood.x;
            float dy = toFood.y;

            if (Time.time >= nextFlipTime && Mathf.Abs(dx) > flipDeadzoneX)
            {
                int want = dx >= 0f ? 1 : -1;
                if (want != direction)
                {
                    direction = want;
                    nextFlipTime = Time.time + flipCooldown;
                }
            }

            float targetVy = Mathf.Clamp(dy * verticalSeekStrength, -maxVerticalSpeed, maxVerticalSpeed);
            vy = Mathf.MoveTowards(vy, targetVy, 2f * Time.fixedDeltaTime);

            float dist = toFood.magnitude;
            if (dist <= eatDistance)
            {
                if (nearest.TryGetComponent<Food>(out var food))
                {
                    hunger = Mathf.Min(maxHunger, hunger + food.nutrition);
                    Object.Destroy(food.gameObject);
                    if (hunger >= maxHunger)
                    {
                        isFull = true;
                        fullTimer = fullTime;
                    }
                    if (hungerBar) hungerBar.Set(hunger, maxHunger);
                }
            }
        }

        Vector2 v = new Vector2(vx, vy);
        v.y = Mathf.Clamp(v.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb.velocity = v;

        if (pos.x > tank.max.x - wallXPadding) { rb.position = new Vector2(tank.max.x - wallXPadding, pos.y); direction = -1; nextFlipTime = Time.time + flipCooldown; }
        if (pos.x < tank.min.x + wallXPadding) { rb.position = new Vector2(tank.min.x + wallXPadding, pos.y); direction = 1; nextFlipTime = Time.time + flipCooldown; }
        if (pos.y > tank.max.y - wallYPadding) rb.position = new Vector2(pos.x, tank.max.y - wallYPadding);
        if (pos.y < tank.min.y + wallYPadding) rb.position = new Vector2(pos.x, tank.min.y + wallYPadding);

        sr.flipX = direction < 0;
        transform.rotation = Quaternion.identity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, foodSenseRadius);
    }
}
