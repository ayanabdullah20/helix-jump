using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioSource mainaudio;
    public AudioSource gameoveraudio;
    public PlayfabManager playfabManager;
    public static GameManager instance;
    public Player player;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerhighscore = PlayerPrefs.GetFloat("highscore", 0);
    }
    public TMP_Text scoretext;
    public GameObject gameoverpanel;
    public TMP_Text scoreongameovertext;
    public TMP_Text highscoreongameovertext;
    public float score=0;
    private float playerhighscore=0;
    public void addscore(float val)
    {
        managedifficulty();
        score += val;
        scoretext.text = "Score: " + score.ToString("0");
        if(score > playerhighscore)
        {
            playerhighscore = score;
            PlayerPrefs.SetFloat("highscore", playerhighscore);
        }
    }   
    public void ongameover()
    {

        scoretext.gameObject.SetActive(false);
        mainaudio.Stop();
        gameoveraudio.Play();
        gameoverpanel.SetActive(true);
        scoreongameovertext.text = "Score: " + score.ToString("0");
        highscoreongameovertext.text = "High Score: " + playerhighscore.ToString("0");
        playfabManager.PostHighScore((int)playerhighscore);
        

    }
    public void restartgame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void managedifficulty()
    {
        if(player.speed < 4)
        {
            player.speed += 0.15f;
        }
    }
}
