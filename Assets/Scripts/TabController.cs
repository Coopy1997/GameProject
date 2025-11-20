using UnityEngine;
using UnityEngine.UI;

public class TabbedShop : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button tabFish;
    public Button tabUpgrades;
    public Button tabDeco;

    [Header("Sections (match order)")]
    public GameObject sectionFish;
    public GameObject sectionUpgrades;
    public GameObject sectionDeco;

    [Header("Tab Colors")]
    public Color active = new Color32(45, 108, 223, 255);
    public Color inactive = new Color32(60, 66, 80, 255);
    public Color activeText = Color.black;      // ← your request: black text
    public Color inactiveText = Color.black;

    void Awake()
    {
        tabFish.onClick.AddListener(() => Show(0));
        tabUpgrades.onClick.AddListener(() => Show(1));
        tabDeco.onClick.AddListener(() => Show(2));
        Show(0);
    }

    void Show(int i)
    {
        sectionFish.SetActive(i == 0);
        sectionUpgrades.SetActive(i == 1);
        sectionDeco.SetActive(i == 2);

        SetTab(tabFish, i == 0);
        SetTab(tabUpgrades, i == 1);
        SetTab(tabDeco, i == 2);
    }

    void SetTab(Button b, bool on)
    {
        if (!b) return;
        var img = b.GetComponent<Image>();
        if (img) img.color = on ? active : inactive;

        var txt = b.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (txt) txt.color = on ? activeText : inactiveText;
    }
}
