using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Scoring : MonoBehaviour
{
    [Header("Score Properties")]
    public int score = 0;
    GameObject goScore;
    GameObject goTimer;
    Text scoreText;
    Text timerText;

    [Header("Time Properties")]
    public float timer = 0;

    public GameObject gameOver;
    public bool isGameOver = false; 

    // Start is called before the first frame update
    void Start()
    {   
        goScore = GameObject.Find("textScore");
        scoreText = goScore.GetComponent<Text>();
        goTimer = GameObject.Find("textTimer");
        timerText = goTimer.GetComponent<Text>();
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

    public void AddScore(int amount)
    {

        score += amount;
    }
    public void GameOver()
    {
        gameOver.SetActive(true);
        Time.timeScale = 0;
        isGameOver = true; 

    }
    public void LOAD_MENU()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
