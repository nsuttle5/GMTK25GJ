using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleCardDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI gameTitle;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI franchiseJoke;
    public TextMeshProUGUI pressAnyKey;
    public TextMeshProUGUI gameNumber;

    private bool canStart = false;

    void Start()
    {
        SetupTitleCard();
        Invoke("EnableStart", 1f); // Wait 1 second before allowing input
    }

    void Update()
    {
        if (canStart && Input.anyKeyDown)
        {
            StartLevel();
        }

        // Blink the "Press Any Key" text
        if (canStart && pressAnyKey != null)
        {
            float alpha = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            Color color = pressAnyKey.color;
            color.a = alpha;
            pressAnyKey.color = color;
        }
    }

    void SetupTitleCard()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        GameEntry currentGame = GameManager.Instance.CurrentGame;

        // Set the text content
        if (gameTitle != null)
        {
            gameTitle.text = currentGame.gameTitle;

            // Debug the title
            Debug.Log($"Setting title to: '{currentGame.gameTitle}' with color: {currentGame.titleColor}");

            // Ensure title is visible - force white if color is too dark/transparent
            Color titleColor = currentGame.titleColor;
            if (titleColor.a < 0.5f || (titleColor.r + titleColor.g + titleColor.b) < 1.5f)
            {
                titleColor = Color.white;
                Debug.Log("Title color was too dark/transparent, forcing to white");
            }
            gameTitle.color = titleColor;
        }

        if (subtitle != null)
            subtitle.text = currentGame.subtitle;

        if (franchiseJoke != null)
            franchiseJoke.text = currentGame.franchiseJoke;

        if (gameNumber != null)
            gameNumber.text = $"Game #{GameManager.Instance.CurrentGameNumber}";

        if (pressAnyKey != null)
            pressAnyKey.gameObject.SetActive(false); // Hide initially
    }

    void EnableStart()
    {
        canStart = true;
        if (pressAnyKey != null)
            pressAnyKey.gameObject.SetActive(true);
    }

    void StartLevel()
    {
        // GameManager.Instance?.StartCurrentLevel();
        GameManager.Instance?.GoToNextGame();
    }
}
