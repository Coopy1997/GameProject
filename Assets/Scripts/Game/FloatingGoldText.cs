using UnityEngine;
using TMPro;

public class FloatingGoldText : MonoBehaviour
{
    public TMP_Text text;
    public float lifetime = 1.2f;
    public float moveUpDistance = 1.0f;
    public Vector3 worldOffset = new Vector3(0f, 0.5f, 0f);

    Vector3 startPos;
    Color startColor;
    float timer;

    void Start()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        startPos = transform.position + worldOffset;

        if (text != null)
            startColor = text.color;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;
        if (t > 1f) t = 1f;

        // move up
        transform.position = startPos + Vector3.up * moveUpDistance * t;

        // fade out
        if (text != null)
        {
            Color c = startColor;
            c.a = 1f - t;
            text.color = c;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string s)
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        if (text != null)
            text.text = s;
    }
}
