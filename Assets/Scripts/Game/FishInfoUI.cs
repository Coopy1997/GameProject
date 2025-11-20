using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishInfoUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Header")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text breedAgeText;

    [Header("Bars")]
    public TMP_Text hungerLabel;
    public Image hungerBar;
    public TMP_Text healthLabel;
    public Image healthBar;

    [Header("Traits")]
    public TMP_Text traitsText;

    private Fish current;

    public void Show(Fish fish)
    {
        SetFish(fish);
    }

    public void Hide()
    {
        ClearFish();
    }

    public void SetFish(Fish fish)
    {
        current = fish;
        if (root) root.SetActive(true);
        UpdateInstant();
    }

    public void ClearFish()
    {
        current = null;
        if (root) root.SetActive(false);
    }

    void Update()
    {
        if (current != null) UpdateInstant();
    }

    void UpdateInstant()
    {
        if (current == null) return;

        // Header
        if (nameText)
            nameText.text = string.IsNullOrEmpty(current.fishName)
                ? "Unnamed Fish"
                : current.fishName;

        if (breedAgeText)
        {
            int m = Mathf.FloorToInt(current.ageSeconds / 60f);
            int s = Mathf.FloorToInt(current.ageSeconds % 60f);
            string age = $"{m}m {s}s";
            breedAgeText.text = $"{current.breedDisplayName}  •  Age: {age}";
        }

        // You can set the icon sprite from the fish's SpriteRenderer
        if (iconImage)
        {
            var sr = current.GetComponent<SpriteRenderer>();
            if (sr) iconImage.sprite = sr.sprite;
        }

        // Hunger
        if (hungerLabel) hungerLabel.text = "Hunger";
        if (hungerBar) hungerBar.fillAmount = current.Hunger01;

        // Health
        if (healthLabel) healthLabel.text = "Health";
        if (healthBar) healthBar.fillAmount = current.health01;

        // Traits
        if (traitsText)
        {
            if (current.traits != null && current.traits.Length > 0)
            {
                traitsText.text = "Traits:\n";
                for (int i = 0; i < current.traits.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(current.traits[i]))
                        traitsText.text += "• " + current.traits[i] + "\n";
                }
            }
            else
            {
                traitsText.text = "Traits:\n• None";
            }

        }
    }
}
