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
    public GameObject fishPrefab;
    public Transform[] spawnPoints;

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

    // ------------------------------
    // Economy
    // ------------------------------
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

    // ------------------------------
    // Shop Controls (for Buttons)
    // ------------------------------
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

    // ------------------------------
    // Fish Management
    // ------------------------------
    public void SpawnFish()
    {
        SpawnFish(fishPrefab);
    }

    public void SpawnFish(GameObject prefab)
    {
        if (!prefab) return;

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

    // ------------------------------
    // Helpers
    // ------------------------------
    public void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}
