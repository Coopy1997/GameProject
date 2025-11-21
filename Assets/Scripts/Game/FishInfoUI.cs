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

    Fish current;

    [HideInInspector] public string traitsTooltip = "";

    public Fish CurrentFish
    {
        get { return current; }
    }

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

    public void ForceRefresh()
    {
        UpdateInstant();
    }

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
        {
            UpdateInstant();
        }
    }

    public void UpdateInstant()
    {
        if (current == null) return;

        if (nameText)
        {
            if (string.IsNullOrEmpty(current.fishName))
                nameText.text = "Unnamed Fish";
            else
                nameText.text = current.fishName;
        }

        if (breedAgeText)
        {
            int m = Mathf.FloorToInt(current.ageSeconds / 60f);
            int s = Mathf.FloorToInt(current.ageSeconds % 60f);
            string age = m + "m " + s + "s";

            string sexSymbol = "?";
            if (current.sex == FishSex.Male) sexSymbol = "♂";
            else if (current.sex == FishSex.Female) sexSymbol = "♀";

            breedAgeText.text = current.breedDisplayName + "  •  " + sexSymbol + "  •  Age: " + age;
        }

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

        if (hungerLabel) hungerLabel.text = "Hunger";
        if (healthLabel) healthLabel.text = "Health";

        if (hungerBar) hungerBar.fillAmount = current.Hunger01;
        if (healthBar) healthBar.fillAmount = current.health01;

        if (traitsText)
        {
            traitsTooltip = "";

            if (current.traits != null && current.traits.Count > 0)
            {
                traitsText.text = "Traits:\n";

                for (int i = 0; i < current.traits.Count; i++)
                {
                    var t = current.traits[i];
                    if (t == null) continue;

                    traitsText.text += "• " + t.name + "\n";

                    traitsTooltip += t.name + " - " + t.description + "\n";
                }

                if (string.IsNullOrEmpty(traitsTooltip))
                {
                    traitsTooltip = traitsText.text;
                }
            }
            else
            {
                traitsText.text = "Traits:\n• None";
                traitsTooltip = "Traits:\n• None";
            }
        }
    }
}
