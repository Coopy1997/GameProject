using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public GameObject root;
    public TMP_Text text;

    RectTransform rt;
    int lastOpenFrame;

    void Awake()
    {
        if (root == null) root = gameObject;
        if (text == null) text = GetComponentInChildren<TMP_Text>();

        rt = root.GetComponent<RectTransform>();
        root.SetActive(false);
    }

    public bool IsVisible
    {
        get { return root != null && root.activeSelf; }
    }

    public void Show(string msg)
    {
        if (root == null || text == null) return;

        root.SetActive(true);
        text.text = msg;

        // we DO NOT move the panel here – you can position it by hand in the editor
        lastOpenFrame = Time.frameCount;
    }

    public void Hide()
    {
        if (root == null) return;
        root.SetActive(false);
    }

    void Update()
    {
        if (!IsVisible) return;

        // click anywhere to close, but ignore the same frame as the opening click
        if (Input.GetMouseButtonDown(0) && Time.frameCount > lastOpenFrame)
        {
            Hide();
        }
    }
}
