using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerStartScreen : MonoBehaviour
{
    public GameObject StartScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("showstartgamebutton", 10f);
    }
    void showstartgamebutton()
    {
        StartScene.SetActive(true);
    }
    public void startgame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
