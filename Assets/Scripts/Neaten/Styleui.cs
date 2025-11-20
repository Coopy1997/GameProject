// Assets/Editor/ShopHardReset.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
#endif

public static class ShopHardReset
{
#if UNITY_EDITOR
    const float RowH = 56f;
    const float CellW = 220f;

    [MenuItem("Tools/Shop/Hard Reset All Rows (anchors, sizes, TMP, HLG)")]
    static void HardReset()
    {
        var root = Selection.activeTransform;
        if (!root)
        {
            Debug.LogWarning("Select ShopPanel in the Hierarchy first.");
            return;
        }

        int rows = 0, cells = 0, fixedTmps = 0, removed = 0;

        // Find ALL rows under ShopPanel, including inactive sections
        foreach (var row in root.GetComponentsInChildren<RectTransform>(true))
        {
            if (!row.name.StartsWith("Row_")) continue;
            rows++;

            // --- Fix row RectTransform ---
            row.anchorMin = new Vector2(0, 1);
            row.anchorMax = new Vector2(1, 1);
            row.pivot = new Vector2(0.5f, 1f);
            row.sizeDelta = new Vector2(0, RowH);
            row.anchoredPosition = Vector2.zero;
            row.localScale = Vector3.one;

            // --- Remove components that fight layout on the row itself ---
            var img = row.GetComponent<Image>(); if (img) Object.DestroyImmediate(img, true);
            var fitter = row.GetComponent<ContentSizeFitter>(); if (fitter) Object.DestroyImmediate(fitter, true);

            // --- Ensure HorizontalLayoutGroup on row ---
            var hlg = row.GetComponent<HorizontalLayoutGroup>() ?? row.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(0, 0, 0, 0);
            hlg.spacing = 16;
            hlg.childAlignment = TextAnchor.MiddleCenter;  // IMPORTANT: vertical centering
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            // Row height via LayoutElement
            var leRow = row.GetComponent<LayoutElement>() ?? row.gameObject.AddComponent<LayoutElement>();
            leRow.preferredHeight = RowH;
            leRow.flexibleHeight = 0;

            // --- Fix children (button + label cells) ---
            for (int i = 0; i < row.childCount; i++)
            {
                var c = row.GetChild(i) as RectTransform;
                if (!c) continue;

                // Remove rogue components that break layout sizing
                c.GetComponent<ContentSizeFitter>()?.let(x => { Object.DestroyImmediate(x, true); removed++; });

                // Child RectTransform & LayoutElement
                c.anchorMin = c.anchorMax = new Vector2(0.5f, 0.5f);
                c.pivot = new Vector2(0.5f, 0.5f);
                c.sizeDelta = new Vector2(CellW, RowH);
                c.localScale = Vector3.one;

                var le = c.GetComponent<LayoutElement>() ?? c.gameObject.AddComponent<LayoutElement>();
                le.ignoreLayout = false;
                le.preferredWidth = CellW;
                le.preferredHeight = RowH;
                le.flexibleWidth = 0;
                le.flexibleHeight = 0;
                cells++;

                var isButton = c.GetComponent<Button>() != null;

                if (isButton)
                {
                    // Ensure exactly one TMP child that fills the button
                    TextMeshProUGUI keep = null;
                    foreach (var tmp in c.GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        if (tmp.transform.parent == c)
                        {
                            if (!keep) keep = tmp;
                            else { Object.DestroyImmediate(tmp.gameObject, true); removed++; }
                        }
                        else { Object.DestroyImmediate(tmp.gameObject, true); removed++; }
                    }
                    if (!keep)
                    {
                        var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                        go.transform.SetParent(c, false);
                        keep = go.GetComponent<TextMeshProUGUI>();
                    }
                    var tr = keep.GetComponent<RectTransform>();
                    tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;

                    keep.enableWordWrapping = false;
                    keep.overflowMode = TextOverflowModes.Ellipsis;
                    keep.fontSize = 28;
                    keep.color = Color.black;
                    keep.alignment = TextAlignmentOptions.Center;
                    keep.raycastTarget = false;
                    fixedTmps++;
                }
                else
                {
                    // Label/Level cells: one TMP on THIS object only
                    var tmps = c.GetComponents<TextMeshProUGUI>();
                    if (tmps.Length == 0) c.gameObject.AddComponent<TextMeshProUGUI>();
                    else if (tmps.Length > 1)
                    {
                        for (int t = 1; t < tmps.Length; t++) { Object.DestroyImmediate(tmps[t], true); removed++; }
                    }
                    // Kill any child TMPs
                    for (int t = c.childCount - 1; t >= 0; t--)
                        if (c.GetChild(t).GetComponent<TextMeshProUGUI>())
                        { Object.DestroyImmediate(c.GetChild(t).gameObject, true); removed++; }

                    var tmp = c.GetComponent<TextMeshProUGUI>();
                    tmp.enableWordWrapping = false;
                    tmp.overflowMode = TextOverflowModes.Ellipsis;
                    tmp.fontSize = 26;
                    tmp.color = Color.black;
                    tmp.alignment = c.name.ToLower().Contains("level")
                        ? TextAlignmentOptions.MidlineRight
                        : TextAlignmentOptions.MidlineLeft;
                    tmp.raycastTarget = false;
                    fixedTmps++;
                }
            }
        }

        // Force rebuild across entire panel tree
        var canvases = root.GetComponentsInParent<Canvas>(true);
        foreach (var c in canvases) { Canvas.ForceUpdateCanvases(); }

        Debug.Log($"Shop hard reset done. Rows: {rows}, Cells sized: {cells}, TMP fixed: {fixedTmps}, Removed stray comps: {removed} ✅");
    }

    static void let<T>(this T obj, System.Action<T> act) where T : Object { if (obj) act(obj); }
#endif
}
