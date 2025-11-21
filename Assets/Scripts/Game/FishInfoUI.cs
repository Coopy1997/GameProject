using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Rename")]
    public RenamePopup renamePopup;

    private Fish current;

    public void Show(Fish fish)
    {
        current = fish;
        if (root) root.SetActive(true);
        UpdateInstant();
    }

    public void Hide()
    {
        current = null;
        if (root) root.SetActive(false);
    }

    // called by RenamePopup when name changes
    public void ForceRefresh()
    {
        UpdateInstant();
    }

    // called by your rename button
    public void OnRenameButtonClicked()
    {
        if (current != null && renamePopup != null)
        {
            renamePopup.Open(current, this);
        }
    }

    void Update()
    {
        if (current != null)
            UpdateInstant();
    }

    public void UpdateInstant()
    {
        if (current == null) return;

        // NAME
        if (nameText)
        {
            nameText.text = string.IsNullOrEmpty(current.fishName)
                ? "Unnamed Fish"
                : current.fishName;
        }

        // BREED + SEX + AGE
        if (breedAgeText)
        {
            int m = Mathf.FloorToInt(current.ageSeconds / 60f);
            int s = Mathf.FloorToInt(current.ageSeconds % 60f);
            string age = $"{m}m {s}s";

            string sexSymbol = "?";
            switch (current.sex)
            {
                case FishSex.Male: sexSymbol = "♂"; break;
                case FishSex.Female: sexSymbol = "♀"; break;
            }

            // e.g. "Sunny Guppy  •  ♀  •  Age: 0m 42s"
            breedAgeText.text = $"{current.breedDisplayName}  •  {sexSymbol}  •  Age: {age}";
        }

        // ICON (uses fish's current sprite)
        if (iconImage)
        {
            var sr = current.GetComponent<SpriteRenderer>();
            if (sr && sr.sprite)
            {
                iconImage.enabled = true;
                iconImage.sprite = sr.sprite;
                iconImage.preserveAspect = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }

        // HUNGER
        if (hungerLabel)
            hungerLabel.text = "Hunger";

        if (hungerBar)
            hungerBar.fillAmount = current.Hunger01;

        // HEALTH
        if (healthLabel)
            healthLabel.text = "Health";

        if (healthBar)
            healthBar.fillAmount = current.health01;

        // TRAITS
        if (traitsText)
        {
            if (current.traits != null && current.traits.Length > 0)
            {
                traitsText.text = "Traits:\n";
                for (int i = 0; i < current.traits.Length; i++)
                {
                    var t = current.traits[i];
                    if (!string.IsNullOrWhiteSpace(t))
                        traitsText.text += "• " + t + "\n";
                }
            }
            else
            {
                traitsText.text = "Traits:\n• None";
            }
        }
    }
}
