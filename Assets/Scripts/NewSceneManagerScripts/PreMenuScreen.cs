using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PreMenuScreen : MonoBehaviour
{
    [SerializeField] private Image image; // Assign in Inspector if needed
    private bool waitTimeFin;
    public float fadeDuration = 1f;
    private bool isFading = false;
    public float blackScreenStayTime;

    private List<Image> imagesToFade;
    private List<TextMeshProUGUI> textsToFade;

    private void Awake()
    {
        // Collect all Image components in this object and its children
        imagesToFade = new List<Image>(GetComponentsInChildren<Image>(true));
        // Collect all TextMeshProUGUI components in this object and its children
        textsToFade = new List<TextMeshProUGUI>(GetComponentsInChildren<TextMeshProUGUI>(true));
    }

    private void FixedUpdate()
    {
        if (waitTimeFin && !isFading)
        {
            StartCoroutine(FadeOutUI());
            isFading = true;
        }
        else if (!waitTimeFin)
        {
            StartCoroutine(CurtainScroll(blackScreenStayTime));
        }
    }

    IEnumerator CurtainScroll(float delay)
    {
        yield return new WaitForSeconds(delay);
        updateBoolFunction();
    }

    private void updateBoolFunction()
    {
        waitTimeFin = true;
    }

    private IEnumerator FadeOutUI()
    {
        float elapsed = 0f;
        // Store original colors
        List<Color> originalImageColors = new List<Color>();
        foreach (var img in imagesToFade)
            originalImageColors.Add(img.color);

        List<Color> originalTextColors = new List<Color>();
        foreach (var txt in textsToFade)
            originalTextColors.Add(txt.color);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            for (int i = 0; i < imagesToFade.Count; i++)
            {
                Color c = originalImageColors[i];
                c.a = alpha;
                imagesToFade[i].color = c;
            }
            for (int i = 0; i < textsToFade.Count; i++)
            {
                Color c = originalTextColors[i];
                c.a = alpha;
                textsToFade[i].color = c;
            }
            yield return null;
        }
        // Ensure alpha is set to 0 at the end
        for (int i = 0; i < imagesToFade.Count; i++)
        {
            Color c = originalImageColors[i];
            c.a = 0f;
            imagesToFade[i].color = c;
        }
        for (int i = 0; i < textsToFade.Count; i++)
        {
            Color c = originalTextColors[i];
            c.a = 0f;
            textsToFade[i].color = c;
        }
        // Deactivate the pre-title screen after fade out
        gameObject.SetActive(false);
    }
}