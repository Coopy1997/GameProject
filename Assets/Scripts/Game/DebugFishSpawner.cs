using UnityEngine;

public class DebugFishSpawner : MonoBehaviour
{
    [Header("Debug Spawn Settings")]
    public string breedIdToSpawn = "DefaultFish";   // e.g. "SUNNY_GUPPY"
    public Transform spawnPoint;                    // where to spawn

    public void SpawnDebugFish()
    {
        if (GameController.Instance == null)
        {
            Debug.LogError("[DEBUG] GameController.Instance is null!");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        // This matches: public Fish SpawnFishByBreedId(string breedId, Vector3 position)
        Fish spawned = GameController.Instance.SpawnFishByBreedId(breedIdToSpawn, pos);

        if (spawned == null)
        {
            Debug.LogError("[DEBUG] FAILED to spawn fish!");
        }
        else
        {
            Debug.Log("[DEBUG] Spawned debug fish: " + spawned.breedDisplayName);
        }
    }
}
