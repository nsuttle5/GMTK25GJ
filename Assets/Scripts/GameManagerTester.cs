using UnityEngine;

public class GameManagerTester : MonoBehaviour
{
    void Update()
    {
        // Test keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing Title Card Load");
            GameManager.Instance?.LoadTitleCard();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Testing Level Load");
            GameManager.Instance?.StartCurrentLevel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Testing Credits Load");
            GameManager.Instance?.CompleteCurrentLevel();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Testing Next Game");
            GameManager.Instance?.GoToNextGame();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Testing Start Sequence");
            GameManager.Instance?.StartGameSequence();
        }

        // Display current game info
        if (GameManager.Instance != null)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                var currentGame = GameManager.Instance.CurrentGame;
                Debug.Log($"Current Game: {currentGame.gameTitle} - {currentGame.franchiseJoke}");
                Debug.Log($"Game {GameManager.Instance.CurrentGameNumber} of {GameManager.Instance.TotalGames}");
                Debug.Log($"Level Scene: {currentGame.levelSceneName}");
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "GameManager Tester");
        GUI.Label(new Rect(10, 30, 300, 20), "T = Title Card, L = Level, C = Credits");
        GUI.Label(new Rect(10, 50, 300, 20), "N = Next Game, S = Start Sequence");
        GUI.Label(new Rect(10, 70, 300, 20), "I = Show Current Game Info");

        if (GameManager.Instance != null)
        {
            var currentGame = GameManager.Instance.CurrentGame;
            GUI.Label(new Rect(10, 100, 400, 20), $"Current: {currentGame.gameTitle}");
            GUI.Label(new Rect(10, 120, 400, 20), $"Game {GameManager.Instance.CurrentGameNumber}/{GameManager.Instance.TotalGames}");
        }
        else
        {
            GUI.Label(new Rect(10, 100, 300, 20), "GameManager not found!");
        }
    }
}
