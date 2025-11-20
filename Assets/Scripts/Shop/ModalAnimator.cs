using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModalAnimator : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup panelGroup;
    public Image backdrop;
    public float openTime = 0.12f;
    public float closeTime = 0.10f;
    public float startScale = 0.95f;
    public float endScale = 1.0f;
    public float backdropMaxAlpha = 0.62f;

    Coroutine anim;
    bool isOpen;

    public void InstantHide()
    {
        if (panel) panel.localScale = Vector3.one * startScale;
        if (panelGroup) { panelGroup.alpha = 0f; panelGroup.interactable = false; panelGroup.blocksRaycasts = false; }
        if (backdrop) { var c = backdrop.color; c.a = 0f; backdrop.color = c; backdrop.raycastTarget = false; }
        if (panel) panel.gameObject.SetActive(false);
        if (backdrop) backdrop.gameObject.SetActive(false);
        isOpen = false;
    }
    public void PlayOpen()
    {
        // Simple open: enable the panel and optionally play an Animator trigger
        gameObject.SetActive(true);

        var anim = GetComponent<Animator>();
        if (anim) anim.SetTrigger("Open");
    }

    public void PlayClose()
    {
        // Simple close: optionally play an Animator trigger, then disable after short delay
        var anim = GetComponent<Animator>();
        if (anim) anim.SetTrigger("Close");
        else gameObject.SetActive(false);
    }

    public void Open()
    {
        if (anim != null) StopCoroutine(anim);
        if (backdrop) backdrop.gameObject.SetActive(true);
        if (panel) panel.gameObject.SetActive(true);
        anim = StartCoroutine(Play(true));
    }

    public void Close()
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Play(false));
    }

    IEnumerator Play(bool opening)
    {
        float t = 0f;
        float dur = opening ? openTime : closeTime;
        float s0 = opening ? startScale : endScale;
        float s1 = opening ? endScale : startScale;
        float a0 = opening ? 0f : 1f;
        float a1 = opening ? 1f : 0f;
        float b0 = opening ? 0f : backdropMaxAlpha;
        float b1 = opening ? backdropMaxAlpha : 0f;

        if (panel) panel.localScale = Vector3.one * s0;
        if (panelGroup) { panelGroup.alpha = a0; panelGroup.interactable = false; panelGroup.blocksRaycasts = false; }
        if (backdrop) { var c = backdrop.color; c.a = b0; backdrop.color = c; backdrop.raycastTarget = true; }

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            k = 1f - Mathf.Pow(1f - k, 3f);
            if (panel) panel.localScale = Vector3.one * Mathf.Lerp(s0, s1, k);
            if (panelGroup) panelGroup.alpha = Mathf.Lerp(a0, a1, k);
            if (backdrop)
            {
                var c = backdrop.color; c.a = Mathf.Lerp(b0, b1, k); backdrop.color = c;
            }
            yield return null;
        }

        if (panel) panel.localScale = Vector3.one * s1;
        if (panelGroup)
        {
            panelGroup.alpha = a1;
            panelGroup.interactable = opening;
            panelGroup.blocksRaycasts = opening;
        }
        if (backdrop)
        {
            var c = backdrop.color; c.a = b1; backdrop.color = c;
            backdrop.raycastTarget = opening;
            if (!opening) backdrop.gameObject.SetActive(false);
        }
        if (!opening && panel) panel.gameObject.SetActive(false);
        isOpen = opening;
        anim = null;
    }
}
