using UnityEngine;

public class MysteryEgg : MonoBehaviour
{
    [Tooltip("breedIds that this egg can hatch into")]
    public string[] possibleBreeds;

    [Tooltip("Weights matching each breedId. e.g. 60, 30, 10")]
    public float[] breedWeights;

    public void Hatch()
    {
        if (GameController.Instance == null)
        {
            Destroy(gameObject);
            return;
        }

        string breedId = PickBreed();
        Vector3 pos = transform.position;
        Fish fish = GameController.Instance.SpawnFishByBreedId(breedId, pos);

        if (fish != null)
        {
            fish.MakeBaby();
        }

        Destroy(gameObject);
    }

    string PickBreed()
    {
        if (possibleBreeds == null || possibleBreeds.Length == 0)
            return "";

        if (breedWeights == null || breedWeights.Length != possibleBreeds.Length)
        {
            int idx = Random.Range(0, possibleBreeds.Length);
            return possibleBreeds[idx];
        }

        float total = 0f;
        for (int i = 0; i < breedWeights.Length; i++)
            total += Mathf.Max(0f, breedWeights[i]);

        float r = Random.value * total;
        for (int i = 0; i < breedWeights.Length; i++)
        {
            float w = Mathf.Max(0f, breedWeights[i]);
            if (r <= w)
                return possibleBreeds[i];
            r -= w;
        }

        return possibleBreeds[0];
    }

    void OnMouseDown()
    {
        Hatch();
    }
}
