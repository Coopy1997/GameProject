using UnityEngine;

public class HungerBar : MonoBehaviour
{
    public Transform fill;

    public void Set(int value, int max)
    {
        float k = max <= 0 ? 0f : Mathf.Clamp01((float)value / max);
        var s = fill.localScale;
        s.x = Mathf.Max(0.0001f, k);
        fill.localScale = s;
    }
}
