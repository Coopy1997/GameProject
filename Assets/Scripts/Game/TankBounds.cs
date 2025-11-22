using UnityEngine;

public class TankBounds : MonoBehaviour
{
    public Vector2 min = new Vector2(-8f, -4f);
    public Vector2 max = new Vector2(8f, 4f);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((min + max) / 2f, max - min);
    }

    /// <summary>
    /// Returns a random point inside the tank rectangle.
    /// margin keeps the point away from the edges a bit.
    /// </summary>
    public Vector3 GetRandomPointInside(float margin = 0.5f)
    {
        float x = Random.Range(min.x + margin, max.x - margin);
        float y = Random.Range(min.y + margin, max.y - margin);
        return new Vector3(x, y, 0f);
    }
}
