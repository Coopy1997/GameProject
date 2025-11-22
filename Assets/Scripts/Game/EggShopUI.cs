using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class EggShopEntry
{
    public string id;
    public EggRarity rarity;

    [Header("Visuals")]
    public Sprite icon;
    public string displayName;
    [TextArea] public string description;
    public int price;

    [Header("Runtime")]
    public FishEgg eggPrefab;
}

public class EggShopUI : MonoBehaviour
{
    [Header("Egg Definitions")]
    public EggShopEntry commonEgg;
    public EggShopEntry rareEgg;
    public EggShopEntry legendaryEgg;

    [Header("Row - Common")]
    public Image commonIcon;
    public TMP_Text commonNameText;
    public TMP_Text commonDescText;
    public TMP_Text commonPriceText;

    [Header("Row - Rare")]
    public Image rareIcon;
    public TMP_Text rareNameText;
    public TMP_Text rareDescText;
    public TMP_Text rarePriceText;

    [Header("Row - Legendary")]
    public Image legendaryIcon;
    public TMP_Text legendaryNameText;
    public TMP_Text legendaryDescText;
    public TMP_Text legendaryPriceText;

    [Header("Spawning")]
    public TankBounds tankBounds;
    public Transform fallbackSpawnPoint;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        SetupRow(commonEgg, commonIcon, commonNameText, commonDescText, commonPriceText);
        SetupRow(rareEgg, rareIcon, rareNameText, rareDescText, rarePriceText);
        SetupRow(legendaryEgg, legendaryIcon, legendaryNameText, legendaryDescText, legendaryPriceText);
    }

    void SetupRow(
        EggShopEntry entry,
        Image iconImg,
        TMP_Text nameTxt,
        TMP_Text descTxt,
        TMP_Text priceTxt)
    {
        if (entry == null) return;

        if (iconImg) iconImg.sprite = entry.icon;
        if (nameTxt) nameTxt.text = entry.displayName;
        if (descTxt) descTxt.text = entry.description;
        if (priceTxt) priceTxt.text = entry.price + " G";
    }

    // hooked up to the Common Buy button
    public void BuyCommonEgg() => BuyEgg(commonEgg);
    // hooked up to the Rare Buy button
    public void BuyRareEgg() => BuyEgg(rareEgg);
    // hooked up to the Legendary Buy button
    public void BuyLegendaryEgg() => BuyEgg(legendaryEgg);

    void BuyEgg(EggShopEntry entry)
    {
        if (entry == null || entry.eggPrefab == null) return;
        if (GameController.Instance == null) return;

        // Try to pay
        if (!GameController.Instance.TrySpendGold(entry.price))
            return;

        // Decide where to spawn the egg
        Vector3 pos = Vector3.zero;
        if (tankBounds != null)
        {
            pos = tankBounds.GetRandomPointInside(0.8f);
        }
        else if (fallbackSpawnPoint != null)
        {
            pos = fallbackSpawnPoint.position;
        }

        Instantiate(entry.eggPrefab, pos, Quaternion.identity);
    }
}
