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
    Text scoreText;

    [Header("Time Properties")]
    public float timer = 0;

    public GameObject gameOver;
    public bool isGameOver = false; 

    // Start is called before the first frame update
    void Start()
    {   
        goScore = GameObject.Find("textScore");
        scoreText = goScore.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            //every 2 seconds add 20 pts to score
            score += 20;

            //reset timer 
            timer = 0;
        }
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
