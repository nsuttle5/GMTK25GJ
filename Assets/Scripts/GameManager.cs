using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Sprite playerIcon;
    public int playerLives = 3;
    [Header("Game Sequence")]
    [SerializeField] private GameEntry[] gameEntries;
    [SerializeField] private int currentGameIndex = 0;

    [Header("Scene Names")]
    [SerializeField] private string titleCardSceneName = "TitleCard";
    [SerializeField] private string creditsSceneName = "Credits";
    [SerializeField] private string preLevelSceneName = "preLevelStatus";

    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Current game info
    public GameEntry CurrentGame => gameEntries[currentGameIndex];
    public int CurrentGameNumber => currentGameIndex + 1;
    public int TotalGames => gameEntries.Length;

    void Awake()
    {
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
        StartCoroutine(GameFlowCoroutine());
    }

    private void LoadTitleCard()
    {
        SceneManager.LoadScene(titleCardSceneName);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(CurrentGame.levelSceneName);
    }

    private void LoadCredits()
    {
        SceneManager.LoadScene(creditsSceneName);
    }

    // Called by level logic when level is complete
    public void CompleteCurrentLevel()
    {
        StartCoroutine(GoToCreditsCoroutine());
    }

    private System.Collections.IEnumerator GoToCreditsCoroutine()
    {
        yield return null;
        LoadCredits();
    }

    public void GoToNextGame()
    {
        currentGameIndex++;
        if (currentGameIndex >= gameEntries.Length)
        {
            currentGameIndex = 0;
        }
        StartCoroutine(GameFlowCoroutine());
    }

    // Main game flow coroutine
    private System.Collections.IEnumerator GameFlowCoroutine()
    {
        // 1. Show Title Card (scene)
        LoadTitleCard();
        // Wait for user input to continue (TitleCardDisplay calls GameManager.Instance.GoToNextGame())
        yield return new WaitUntil(() => currentGameIndexHasAdvanced);
        // 2. Play Level (scene)
        LoadLevel();
        // Wait for level to complete (should be triggered by level logic calling CompleteCurrentLevel)
        // 3. Show Credits (scene)
        // After credits, GoToNextGame() will be called (e.g., from credits scene UI)
    }

    // Helper property for coroutine to detect when title card input advances the game
    private int lastGameIndex = -1;
    private bool currentGameIndexHasAdvanced => lastGameIndex != currentGameIndex;
    void OnEnable() { lastGameIndex = currentGameIndex; }
    void OnDisable() { lastGameIndex = currentGameIndex; }
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
