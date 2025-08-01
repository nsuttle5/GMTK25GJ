using UnityEngine;

public class LevelIntroAutoStart : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartCurrentLevel();
        }
    }
}
