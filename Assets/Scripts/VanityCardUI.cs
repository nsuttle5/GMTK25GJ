using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VanityCardUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI jokeText;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI companyText;
    public Image background;
    public float fadeDuration = 0.75f;
    public float displayDuration = 1.5f;

    // Call this from GameManager: yield return vanityCardUI.ShowVanity(...)
    // Now accepts year and company
    public IEnumerator ShowVanity(string title, string joke, Color bgColor, string year, string company)
    {
        // Set content
        if (titleText) titleText.text = title;
        if (jokeText) jokeText.text = joke;
        if (yearText) yearText.text = year;
        if (companyText) companyText.text = company;
        if (background) background.color = bgColor;
        if (canvasGroup) canvasGroup.alpha = 0f;
        gameObject.SetActive(true);

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSecondsRealtime(displayDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
