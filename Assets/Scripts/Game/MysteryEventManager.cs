using UnityEngine;

public class MysteryEventManager : MonoBehaviour
{
    public MysteryEgg eggPrefab;
    public float minInterval = 60f;
    public float maxInterval = 180f;

    float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (!eggPrefab) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TrySpawnEgg();
            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    void TrySpawnEgg()
    {
        if (GameController.Instance == null) return;

        int count = GameController.Instance.GetFishCount();
        if (count >= GameController.Instance.maxFish - 1)
            return;

        TankBounds tb = Object.FindObjectOfType<TankBounds>();
        if (!tb) return;

        float x = Random.Range(tb.min.x, tb.max.x);
        float y = Random.Range(tb.min.y + 0.5f, tb.max.y - 0.5f);

        Object.Instantiate(eggPrefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}
