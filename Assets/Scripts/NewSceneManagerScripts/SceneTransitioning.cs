using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioning : MonoBehaviour
{

 
    [SerializeField] public string[] levels;


    public void onButtonClick()
    {
        Scene scene = SceneManager.GetActiveScene() ;
        
        char sceneNum = scene.name[scene.name.Length - 1];

        Debug.Log(sceneNum);
        
        switch (sceneNum)
        {
            case '1':
                SceneManager.LoadScene(levels[1]);
                break;

            case '2':
                SceneManager.LoadScene(levels[2]);
                break;
            
            case '3':
                SceneManager.LoadScene(levels[3]);
                break;

            case '4':
                SceneManager.LoadScene(levels[4]);
                break;

            case '5':
                SceneManager.LoadScene(levels[5]);
                break;

            case '6':
                SceneManager.LoadScene(levels[6]);
                break;

            case '7':
                SceneManager.LoadScene(levels[7]);
                break;

            default:
                SceneManager.LoadScene(levels[0]);

                break;

        }
    }
}
