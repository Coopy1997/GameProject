using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Economy")]
    public int gold = 50;
    public TMP_Text goldText;
    public int goldPerFishFeed = 3;
    public int goldPerFishSell = 50;
    public int goldFromCleanBonus = 10;

    [Header("Spawning")]
    public Transform fishParent;
    public Fish fallbackFishPrefab;
    public List<FishCatalogItem> fishCatalog = new List<FishCatalogItem>();
    public int maxFish = 50;

    [Header("References")]
    public WaterHealthController waterHealth;
    public ShopController shop;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buySound;
    public AudioClip sellSound;
    public AudioClip errorSound;
    public AudioClip feedSound;
    public AudioClip cleanSound;

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateGoldUI();
    }

    void Start()
    {
        UpdateFishCount();
    }

    public void UpdateGoldUI()
    {
        if (goldText) goldText.text = gold + " G";
    }

    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;
        UpdateGoldUI();
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount)
        {
            PlaySound(errorSound);
            return false;
        }
        gold -= amount;
        UpdateGoldUI();
        PlaySound(buySound);
        return true;
    }

    public Fish SpawnFish()
    {
        if (GetFishCount() >= maxFish)
        {
            PlaySound(errorSound);
            return null;
        }

        FishCatalogItem item = null;
        if (fishCatalog != null && fishCatalog.Count > 0)
        {
            item = fishCatalog[0];
        }

        Vector3 pos = GetRandomTankPosition();
        Fish f = SpawnFishFromItem(item, pos);
        return f;
    }

    public Fish SpawnFishByBreedId(string breedId)
    {
        Vector3 pos = GetRandomTankPosition();
        return SpawnFishByBreedId(breedId, pos);
    }

    public Fish SpawnFishByBreedId(string breedId, Vector3 worldPos)
    {
        if (GetFishCount() >= maxFish)
        {
            PlaySound(errorSound);
            return null;
        }

        FishCatalogItem item = GetCatalogItemByBreedId(breedId);
        if (item != null)
        {
            return SpawnFishFromItem(item, worldPos);
        }

        return SpawnFishFromPrefab(fallbackFishPrefab, worldPos, breedId);
    }

    Fish SpawnFishFromItem(FishCatalogItem item, Vector3 pos)
    {
        if (item == null || item.prefab == null)
        {
            return SpawnFishFromPrefab(fallbackFishPrefab, pos, null);
        }

        Fish prefab = item.prefab;
        Transform parent = fishParent ? fishParent : null;
        Fish f = Instantiate(prefab, pos, Quaternion.identity, parent);

        f.breedId = prefab.breedId;
        f.breedDisplayName = item.displayName;

        UpdateFishCount();
        return f;
    }

    Fish SpawnFishFromPrefab(Fish prefab, Vector3 pos, string overrideBreedId)
    {
        if (prefab == null) return null;

        Transform parent = fishParent ? fishParent : null;
        Fish f = Instantiate(prefab, pos, Quaternion.identity, parent);

        if (!string.IsNullOrEmpty(overrideBreedId))
        {
            f.breedId = overrideBreedId;
        }

        if (string.IsNullOrEmpty(f.breedDisplayName))
        {
            f.breedDisplayName = prefab.breedDisplayName;
        }

        UpdateFishCount();
        return f;
    }

    FishCatalogItem GetCatalogItemByBreedId(string id)
    {
        if (fishCatalog == null) return null;
        for (int i = 0; i < fishCatalog.Count; i++)
        {
            var item = fishCatalog[i];
            if (item == null || item.prefab == null) continue;

            Fish pf = item.prefab;
            if (!string.IsNullOrEmpty(pf.breedId) && pf.breedId == id) return item;
            if (!string.IsNullOrEmpty(pf.breedDisplayName) && pf.breedDisplayName == id) return item;
            if (!string.IsNullOrEmpty(item.displayName) && item.displayName == id) return item;
        }
        return null;
    }

    Vector3 GetRandomTankPosition()
    {
        TankBounds tb = FindObjectOfType<TankBounds>();
        if (!tb) return Vector3.zero;

        float x = Random.Range(tb.min.x, tb.max.x);
        float y = Random.Range(tb.min.y, tb.max.y);
        return new Vector3(x, y, 0f);
    }

    public void OnFishFed(Fish fish)
    {
        AddGold(goldPerFishFeed);
        PlaySound(feedSound);
    }

    public void OnTankCleaned()
    {
        AddGold(goldFromCleanBonus);
        PlaySound(cleanSound);
    }

    public void SellFish(Fish fish)
    {
        if (fish == null) return;

        float mult = fish.GetSellPriceMultiplier();
        int amount = Mathf.RoundToInt(goldPerFishSell * mult);
        if (amount < 0) amount = 0;

        AddGold(amount);
        Destroy(fish.gameObject);
        UpdateFishCount();
        PlaySound(sellSound);
    }

    public int GetFishCount()
    {
        Fish[] fish = FindObjectsOfType<Fish>();
        return fish == null ? 0 : fish.Length;
    }

    public void UpdateFishCount()
    {
        if (waterHealth != null)
        {
            int c = GetFishCount();
            waterHealth.RegisterFishCount(c);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
