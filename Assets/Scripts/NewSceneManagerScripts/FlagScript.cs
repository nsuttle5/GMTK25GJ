using UnityEngine;
using UnityEngine.SceneManagement;
public class FlagScript : MonoBehaviour
{

    private string[] MainMenus = { "Main Menu 1", "Main Menu 2", "Main Menu 3", "Main Menu 4", "Main Menu 5", "Main Menu 6", "Main Menu 7" };
    private string[] levels = { "SampleScene", "TickyTwo", "TickyAndTocky", "Ticky4", "Ticky5", "TickSix", "TickyRem" };

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player")) 
        {
            Scene scene = SceneManager.GetActiveScene();

            int sceneNum = 0;

            while (scene.name != levels[sceneNum])
            {
                sceneNum++;
            }

            SceneManager.LoadScene(MainMenus[sceneNum]);
        }
    }
}
