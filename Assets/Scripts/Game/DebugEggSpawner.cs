using UnityEngine;

public class DebugEggSpawner : MonoBehaviour
{
    [Header("References")]
    public TankBounds tankBounds;
    public EggRarity rarityToSpawn = EggRarity.Common;

    [Tooltip("Optional fixed spawn point if tankBounds is not set.")]
    public Transform fallbackSpawnPoint;

    public void SpawnDebugEgg()
    {
        if (GameController.Instance == null)
        {
            Debug.LogError("[DEBUG] GameController.Instance is null!");
            return;
        }

        // Decide where to spawn
        Vector3 pos = Vector3.zero;

        if (tankBounds != null)
        {
            // Random point in tank
            pos = tankBounds.GetRandomPointInside(0.8f);
        }
        else if (fallbackSpawnPoint != null)
        {
            pos = fallbackSpawnPoint.position;
        }

        // Pick the correct egg prefab from GameController
        FishEgg prefab = null;
        switch (rarityToSpawn)
        {
            case EggRarity.Common:
                prefab = GameController.Instance.commonEggPrefab;
                break;
            case EggRarity.Rare:
                prefab = GameController.Instance.rareEggPrefab;
                break;
            case EggRarity.Legendary:
                prefab = GameController.Instance.legendaryEggPrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("[DEBUG] No egg prefab set for rarity " + rarityToSpawn);
            return;
        }

        Instantiate(prefab, pos, Quaternion.identity);
        Debug.Log("[DEBUG] Spawned " + rarityToSpawn + " egg at " + pos);
    }
}
