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
    public int goldPerFishSell = 50;
    public int goldFromCleanBonus = 10;

    [Header("Spawning")]
    public Transform fishParent;
    public GameObject fishPrefab;             // fallback / simple spawn
    public Transform[] spawnPoints;
    public int maxFish = 30;                  // hard cap

    [Header("Fish Catalog")]
    public FishCatalogItem[] fishCatalog;     // edit in Inspector

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
    }

    // --------- Economy ---------
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

    // --------- Shop Controls ---------
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

    // --------- Fish Management ---------
    // Basic spawn used by ShopUI
    public void SpawnFish()
    {
        // If catalog has at least one entry, use that prefab; otherwise fall back
        if (fishCatalog != null && fishCatalog.Length > 0 && fishCatalog[0] != null && fishCatalog[0].prefab)
        {
            SpawnFish(fishCatalog[0].prefab.gameObject);
        }
        else
        {
            SpawnFish(fishPrefab);
        }
    }

    public void SpawnFish(GameObject prefab)
    {
        if (!prefab) return;
        if (allFish.Count >= maxFish) return;

        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        GameObject f = Instantiate(prefab, pos, Quaternion.identity, fishParent);
        Fish fish = f.GetComponent<Fish>();
        if (fish != null && !allFish.Contains(fish))
            allFish.Add(fish);

        if (waterHealth)
            waterHealth.RegisterFishCount(allFish.Count);
    }

    // helper for breeding / specific buys
    public FishCatalogItem GetCatalogItemById(string id)
    {
        if (fishCatalog == null) return null;
        for (int i = 0; i < fishCatalog.Length; i++)
        {
            var item = fishCatalog[i];
            if (item != null && item.displayName == id) // or use a separate id string if you add one
                return item;
        }
        return null;
    }

    public Fish SpawnFishByBreedId(string id)
    {
        // find catalog item by id
        FishCatalogItem item = null;

        // prefer matching "id" field if you added it
        for (int i = 0; i < fishCatalog.Length; i++)
        {
            if (fishCatalog[i] != null)
            {
                // If you use "id" field inside FishCatalogItem, replace "displayName" with "id"
                if (fishCatalog[i].id == id)
                {
                    item = fishCatalog[i];
                    break;
                }
            }
        }

        // fallback: match by displayName
        if (item == null)
        {
            for (int i = 0; i < fishCatalog.Length; i++)
            {
                if (fishCatalog[i] != null && fishCatalog[i].displayName == id)
                {
                    item = fishCatalog[i];
                    break;
                }
            }
        }

        if (item == null || item.prefab == null) return null;
        if (allFish.Count >= maxFish) return null;

        // pick a spawn point
        Transform sp = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
            sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 pos = sp ? sp.position : Vector3.zero;

        // spawn the fish
        GameObject f = Instantiate(item.prefab.gameObject, pos, Quaternion.identity, fishParent);
        Fish fish = f.GetComponent<Fish>();

        if (fish != null)
        {
            if (!allFish.Contains(fish)) allFish.Add(fish);
            if (waterHealth) waterHealth.RegisterFishCount(allFish.Count);
        }

        return fish;
    }

    public Fish SpawnFishFromItem(FishCatalogItem item)
    {
        if (item == null || !item.prefab) return null;
        if (allFish.Count >= maxFish) return null;

        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        GameObject f = Instantiate(item.prefab.gameObject, pos, Quaternion.identity, fishParent);
        Fish fish = f.GetComponent<Fish>();
        if (fish != null && !allFish.Contains(fish))
            allFish.Add(fish);

        if (waterHealth)
            waterHealth.RegisterFishCount(allFish.Count);

        return fish;
    }

    public void OnFishFed(Fish fish)
    {
        AddGold(goldPerFishFeed);
    }

    public void OnFishDied(Fish fish)
    {
        if (fish && allFish.Contains(fish))
            allFish.Remove(fish);

        if (waterHealth)
            waterHealth.RegisterFishCount(allFish.Count);

        PlaySound(errorSound);
    }

    // --------- Helpers ---------
    public void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}
