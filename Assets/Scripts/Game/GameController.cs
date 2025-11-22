using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Economy")]
    public int gold = 50;
    public TMP_Text goldText;
    public int goldPerFishFeed = 3;
    public int fallbackBaseSellValue = 50;
    public int goldFromCleanBonus = 10;

    [Header("Capacity")]
    public int maxFish = 3;          // tank capacity, used by other systems

    [Header("Spawning")]
    public Transform fishParent;
    public GameObject fishPrefab;    // generic fallback prefab
    public Transform[] spawnPoints;

    [Header("Egg Prefabs")]
    public FishEgg commonEggPrefab;
    public FishEgg rareEggPrefab;
    public FishEgg legendaryEggPrefab;

    [Header("Fish Breeds by Rarity")]
    public FishCatalogItem[] commonBreeds;
    public FishCatalogItem[] rareBreeds;
    public FishCatalogItem[] legendaryBreeds;

    [Header("UI")]
    public TMP_Text statusText;
    public GameObject shopPanel;
    public Button shopButton;
    public Button closeShopButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip shopSound;
    public AudioClip successSound;
    public AudioClip errorSound;
    public AudioClip sellSound;

    [Header("FX")]
    public FloatingGoldText goldPopupPrefab;

    [Header("State")]
    public List<Fish> allFish = new List<Fish>();

    private WaterHealthController waterHealth;
    private static GameController _instance;
    public static GameController Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        waterHealth = FindObjectOfType<WaterHealthController>();
        UpdateGoldUI();

        if (shopButton) shopButton.onClick.AddListener(OpenShop);
        if (closeShopButton) closeShopButton.onClick.AddListener(CloseShop);

        // collect any fish already in scene
        var fishes = FindObjectsOfType<Fish>();
        for (int i = 0; i < fishes.Length; i++)
        {
            if (!allFish.Contains(fishes[i]))
                allFish.Add(fishes[i]);
        }

        UpdateFishCount();
    }

    // =========================================================
    // Economy
    // =========================================================
    public bool TrySpendGold(int amount)
    {
        if (gold < amount)
        {
            PlaySound(errorSound);
            return false;
        }

        gold -= amount;
        UpdateGoldUI();
        PlaySound(successSound);
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
        PlaySound(successSound);
    }

    void UpdateGoldUI()
    {
        if (goldText) goldText.text = $"Gold: {gold}";
    }

    // =========================================================
    // Shop Controls
    // =========================================================
    public void OpenShop()
    {
        if (shopPanel) shopPanel.SetActive(true);
        PlaySound(shopSound ? shopSound : clickSound);
    }

    public void CloseShop()
    {
        if (shopPanel) shopPanel.SetActive(false);
        PlaySound(clickSound);
    }

    public void ToggleShopPanel()
    {
        if (!shopPanel) return;
        bool newState = !shopPanel.activeSelf;
        shopPanel.SetActive(newState);
        PlaySound(clickSound);
    }

    // =========================================================
    // Fish Spawning (generic)
    // =========================================================
    public void SpawnFish()
    {
        SpawnFish(fishPrefab);
    }

    public void SpawnFish(GameObject prefab)
    {
        if (!prefab) return;

        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            spawnPoint = spawnPoints[idx];
        }

        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        GameObject f = Instantiate(prefab, pos, Quaternion.identity, fishParent);
        Fish fish = f.GetComponent<Fish>();
        if (fish != null)
        {
            if (!allFish.Contains(fish))
                allFish.Add(fish);

            // every spawned fish starts as a baby
            fish.MakeBaby();
        }

        UpdateFishCount();
    }

    // =========================================================
    // Spawning from catalog / ids (used by eggs, breeding, debug)
    // =========================================================

    /// <summary>
    /// Spawn a fish based on a catalog item, returns the Fish component.
    /// </summary>
    public Fish SpawnFishFromCatalog(FishCatalogItem catalog, Vector3 position)
    {
        if (catalog == null || catalog.prefab == null)
        {
            Debug.LogWarning("SpawnFishFromCatalog: missing prefab on catalog item");
            return null;
        }

        // optional capacity check – comment out if you don't want this here
        if (allFish.Count >= maxFish)
        {
            Debug.Log("[GameController] Tank at capacity, cannot spawn more fish.");
            return null;
        }

        Fish fish = Instantiate(catalog.prefab, position, Quaternion.identity, fishParent);

        if (fish != null)
        {
            fish.breedId = catalog.id;
            fish.breedDisplayName = catalog.displayName;
            fish.baseSellValue = catalog.baseSellValue;
            fish.breedValueMultiplier = catalog.breedValueMultiplier;

            fish.MakeBaby();

            if (!allFish.Contains(fish))
                allFish.Add(fish);
        }

        UpdateFishCount();
        return fish;
    }

    /// <summary>
    /// New helper to keep old scripts happy.
    /// Spawns a fish by its breedId string and returns the Fish.
    /// </summary>
    public Fish SpawnFishByBreedId(string breedId, Vector3 position)
    {
        FishCatalogItem item = FindCatalogItemById(breedId);
        if (item == null)
        {
            Debug.LogWarning("SpawnFishByBreedId: no catalog item with id " + breedId);
            return null;
        }

        return SpawnFishFromCatalog(item, position);
    }

    /// <summary>
    /// Searches all rarity arrays for a matching id.
    /// </summary>
    private FishCatalogItem FindCatalogItemById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        FishCatalogItem result = FindInArray(commonBreeds, id);
        if (result != null) return result;

        result = FindInArray(rareBreeds, id);
        if (result != null) return result;

        result = FindInArray(legendaryBreeds, id);
        if (result != null) return result;

        return null;
    }

    private FishCatalogItem FindInArray(FishCatalogItem[] arr, string id)
    {
        if (arr == null) return null;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null && arr[i].id == id)
                return arr[i];
        }
        return null;
    }

    // =========================================================
    // Spawning from Eggs
    // =========================================================
    // Called by FishEgg / MysteryEgg when they hatch
    public void SpawnFishFromEgg(EggRarity rarity, Vector3 position)
    {
        FishCatalogItem[] source = commonBreeds;

        switch (rarity)
        {
            case EggRarity.Common: source = commonBreeds; break;
            case EggRarity.Rare: source = rareBreeds; break;
            case EggRarity.Legendary: source = legendaryBreeds; break;
        }

        if (source == null || source.Length == 0)
        {
            Debug.LogWarning("SpawnFishFromEgg: no breeds set for rarity " + rarity);
            return;
        }

        FishCatalogItem catalog = source[Random.Range(0, source.Length)];
        SpawnFishFromCatalog(catalog, position);
    }

    // =========================================================
    // Fish Management / Callbacks
    // =========================================================
    public void OnFishFed(Fish fish)
    {
        AddGold(goldPerFishFeed);
    }

    public void OnFishDied(Fish fish)
    {
        if (fish && allFish.Contains(fish))
            allFish.Remove(fish);

        UpdateFishCount();
        PlaySound(errorSound);
    }

    public void SellFish(Fish fish)
    {
        if (fish == null) return;

        int amount = fish.GetSellPrice();
        if (amount <= 0) amount = fallbackBaseSellValue;

        AddGold(amount);

        // floating +gold popup
        if (goldPopupPrefab != null)
        {
            Vector3 pos = fish.transform.position;
            FloatingGoldText popup = Instantiate(goldPopupPrefab, pos, Quaternion.identity);
            popup.SetText("+" + amount + " G");
        }

        if (allFish.Contains(fish))
            allFish.Remove(fish);

        Destroy(fish.gameObject);
        UpdateFishCount();

        PlaySound(sellSound ? sellSound : successSound);
        Debug.Log($"Sold {fish.breedDisplayName} for {amount} G");
    }

    public void UpdateFishCount()
    {
        // clean up nulls
        for (int i = allFish.Count - 1; i >= 0; i--)
        {
            if (allFish[i] == null)
                allFish.RemoveAt(i);
        }

        if (waterHealth)
            waterHealth.RegisterFishCount(allFish.Count);
    }

    /// <summary>
    /// Old helper for scripts: current number of fish in tank.
    /// </summary>
    public int GetFishCount()
    {
        // in case nulls got left in the list
        int count = 0;
        for (int i = 0; i < allFish.Count; i++)
        {
            if (allFish[i] != null) count++;
        }
        return count;
    }

    // =========================================================
    // Helpers
    // =========================================================
    public void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}
