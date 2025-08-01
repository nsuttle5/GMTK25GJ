using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelIntroUI : MonoBehaviour
{
    public Image blackScreen;
    public TextMeshProUGUI worldNameText;
    public Image playerIcon;
    public TextMeshProUGUI livesText;
    public float fadeDuration = 1f;
    public float displayDuration = 1.5f;

    void Awake()
    {
        // Destroy duplicate LevelIntroUI objects
        var others = FindObjectsByType<LevelIntroUI>(FindObjectsSortMode.None);
        if (others.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        SetAlpha(1f);
    }

    public void Show(string worldName, Sprite playerSprite, int lives)
    {
        worldNameText.text = worldName;
        playerIcon.sprite = playerSprite;
        livesText.text = "x " + lives;
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Fade in (already black, so just wait)
        SetAlpha(1f);
        yield return new WaitForSeconds(fadeDuration);
        // Wait with info displayed
        SetAlpha(1f);
        yield return new WaitForSeconds(displayDuration);
        // Fade out
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(1f - t / fadeDuration);
            yield return null;
        }
        SetAlpha(0f);
        gameObject.SetActive(false); // Hide UI after intro
        // Optionally: Unpause gameplay here
    }

    void SetAlpha(float a)
    {
        if (blackScreen != null)
        {
            var c = blackScreen.color;
            c.a = a;
            blackScreen.color = c;
        }
        if (worldNameText != null)
        {
            var c = worldNameText.color;
            c.a = a;
            worldNameText.color = c;
        }
        if (playerIcon != null)
        {
            var c = playerIcon.color;
            c.a = a;
            playerIcon.color = c;
        }
        if (livesText != null)
        {
            var c = livesText.color;
            c.a = a;
            livesText.color = c;
        }
    }
}
