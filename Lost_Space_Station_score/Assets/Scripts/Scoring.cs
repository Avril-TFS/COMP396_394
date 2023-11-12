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
    Text scoreText;
    GameObject goHealth;
    Text healthText;
    public int health = 100;
    public GameObject damgeUI;
    public Text damageText;
    public bool isDamaged = false;
    public GameObject gameOver;
    public bool isGameOver = false;

    [Header("Time Properties")]
    public float timer = 0;

   

    // Start is called before the first frame update
    void Start()
    {   
        goScore = GameObject.Find("textScore");
        scoreText = goScore.GetComponent<Text>();

        goHealth = GameObject.Find("textHealth");
        healthText = goHealth.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            //every 2 seconds add 20 pts to score
            score += 20;

            //reset damage to false after 2 sec
            if (isDamaged)
            {
                isDamaged = false;
                damgeUI.SetActive(false);

            }

            //reset timer 
            timer = 0;
        }
        scoreText.text = "SCORE: " + score.ToString("#,0");
        healthText.text = "HEALTH: " + health.ToString("#,0");
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
    public void DisplayDamage(int amount)
    {
        damgeUI.SetActive(true);
        damageText.text = "- " + amount.ToString("#,0");
        isDamaged = true;

    }
}
