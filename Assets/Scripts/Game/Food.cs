using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Food : MonoBehaviour
{
    public int nutrition = 25;
    public float lifeTime = 12f;
    public float sinkSpeed = 0.7f;
    public float floorOffset = 0.05f;

    Rigidbody2D rb;
    TankBounds tank;
    float t;
    bool atBottom;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tank = FindObjectOfType<TankBounds>();
        rb.gravityScale = 0f;
        rb.drag = 2f;
        var col = GetComponent<Collider2D>();
        col.isTrigger = false;
    }

    void FixedUpdate()
    {
        if (!atBottom)
        {
            Vector2 p = rb.position;
            p.y += -sinkSpeed * Time.fixedDeltaTime;
            if (tank != null && p.y <= tank.min.y + floorOffset)
            {
                p.y = tank.min.y + floorOffset;
                atBottom = true;
                rb.velocity = Vector2.zero;
            }
            rb.MovePosition(p);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= lifeTime) Destroy(gameObject);
    }
}
