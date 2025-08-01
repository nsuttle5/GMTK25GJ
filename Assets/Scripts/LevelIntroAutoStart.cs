using UnityEngine;

public class LevelIntroAutoStart : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance?.StartGameSequence();
    }
}
