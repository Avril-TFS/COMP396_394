using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Scoring : MonoBehaviour
{
    [Header("UI Properties")]
    public int score = 0;
    GameObject goScore;
    GameObject goTimer;
    GameObject goMessage;
    Text scoreText;
    Text timerText;
    Text messageText;
    public GameObject KeyPickupItemDisplay;

    [Header("Time Properties")]
    public float timer = 0;

    public GameObject gameOver;
    public bool isGameOver = false;

    public GameObject gameClear;

    // Start is called before the first frame update
    void Start()
    {   
        goScore = GameObject.Find("textScore");
        scoreText = goScore.GetComponent<Text>();
        goTimer = GameObject.Find("textTimer");
        timerText = goTimer.GetComponent<Text>();
        goMessage = GameObject.Find("textMessage");
        messageText = goMessage.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        // Convert timer to minutes and seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        
        timerText.text = string.Format("TIMER: {0:00}:{1:00}", minutes, seconds);
        //if (timer > 2f)
        //{
        //    //every 2 seconds add 20 pts to score
        //    score += 20;

        //    //reset timer 
        //    timer = 0;
        //}
        scoreText.text = "SCORE: " + score.ToString("#,0");
        if (Input.GetKeyUp(KeyCode.Escape) && isGameOver)
        {
            LOAD_MENU();
        }
        
    }

    public void sendMessageToUI(string message)
    {
        messageText.text = "Message: " + message;
        StartCoroutine(ClearMessageAfterDelay(3f));
    }

    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Clear the message after the delay
        messageText.text = "Message: ";
    }

    public void AddScore(int amount)
    {

        score += amount;
    }
    public void GameOver()
    {
        //I erased gameOver panel becaus we already had one
        //gameOver.SetActive(true);
        //Time.timeScale = 0;
        isGameOver = true;
        SceneManager.LoadScene("GameOverScene");

    }

    public void GameClear()
    {
        //show gameClear screen
        gameClear.SetActive(true);
        //Slow down the scene
        //Time.timeScale = 0.5f;
    }
    public void LOAD_MENU()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void KeypickUpUI()
    {
        KeyPickupItemDisplay.SetActive(true);
    }
}
