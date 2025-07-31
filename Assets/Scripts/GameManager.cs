using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Sequence")]
    [SerializeField] private GameEntry[] gameEntries;
    [SerializeField] private int currentGameIndex = 0;

    [Header("Scene Names")]
    [SerializeField] private string titleCardSceneName = "TitleCard";
    [SerializeField] private string creditsSceneName = "Credits";

    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Current game info
    public GameEntry CurrentGame => gameEntries[currentGameIndex];
    public int CurrentGameNumber => currentGameIndex + 1;
    public int TotalGames => gameEntries.Length;

    void Awake()
    {
        // Singleton pattern - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGameSequence()
    {
        currentGameIndex = 0;
        LoadTitleCard();
    }

    public void LoadTitleCard()
    {
        SceneManager.LoadScene(titleCardSceneName);
    }

    public void StartCurrentLevel()
    {
        if (currentGameIndex < gameEntries.Length)
        {
            SceneManager.LoadScene(CurrentGame.levelSceneName);
        }
        else
        {
            // All games completed - restart the loop
            RestartSequence();
        }
    }

    public void CompleteCurrentLevel()
    {
        SceneManager.LoadScene(creditsSceneName);
    }

    public void GoToNextGame()
    {
        currentGameIndex++;

        if (currentGameIndex < gameEntries.Length)
        {
            // Load next title card
            LoadTitleCard();
        }
        else
        {
            // Loop back to beginning - the endless franchise cycle!
            RestartSequence();
        }
    }

    void RestartSequence()
    {
        currentGameIndex = 0;
        LoadTitleCard();
    }
}

[System.Serializable]
public class GameEntry
{
    [Header("Game Info")]
    public string gameTitle = "Super Mario Bros 1";
    public string subtitle = "The Original Classic";
    public string levelSceneName = "Level1-1";

    [Header("Parody Elements")]
    public string franchiseJoke = "Now with 50% more jumping!";
    public Color titleColor = Color.white;

    [Header("Credits Customization")]
    public string productionCompany = "Nostalgic Games Studio";
    [Range(5, 25)]
    public int numberOfRoles = 15;
    [Header("Credits Scroll")]
    public float creditsScrollDistance = 12000f;
}
