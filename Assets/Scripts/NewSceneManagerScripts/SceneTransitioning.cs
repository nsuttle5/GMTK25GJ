using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioning : MonoBehaviour
{
    [SerializeField] public string[] levels;
    
    public void onButtonClick()
    {
        SceneManager.LoadScene(levels[0]);
    }
}
