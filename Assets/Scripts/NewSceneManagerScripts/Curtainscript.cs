using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

public class Curtainscript : MonoBehaviour
{
    public TextMeshProUGUI World;
    public TextMeshProUGUI Lives;
    private string[] levels = { "SampleScene", "TickyTwo", "TickyAndTocky", "Ticky4", "Ticky5", "TickSix", "TickyRem" };
    
    private void Start()
    {

        string SceneName = SceneManager.GetActiveScene().name;
        int sceneNum = 0;

        while (SceneName != levels[sceneNum])
        {
            sceneNum++;
        }

        World.text = "World-" + (sceneNum + 1); 
    }
}
