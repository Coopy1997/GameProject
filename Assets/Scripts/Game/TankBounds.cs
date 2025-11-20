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
}
