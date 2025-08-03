using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine.Rendering;
using System.Collections;
using Unity.VisualScripting;
public class Curtainscript : MonoBehaviour
{
    private bool waitTimeFin = false;
    public float curtainSpeed; 
    public TextMeshProUGUI World;
    public TextMeshProUGUI Lives;
    public GameObject player;
    public RectTransform rectTransform;
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
        Lives.text = "Lives - " + player.GetComponent<PlayerHealth>().currentHealth.ToString();
    }

    private void FixedUpdate()
    {
        if (waitTimeFin)
        {
            rectTransform.transform.position += new Vector3(0, curtainSpeed, 0);
        }
        else
        {
            StartCoroutine(CurtainScroll(5f));
        }
    }
    IEnumerator CurtainScroll(float delay)
    {

        yield return new WaitForSeconds(delay);

        updateBoolFunction();
    }

    private void updateBoolFunction()
    {
        waitTimeFin = true;
    }
}
