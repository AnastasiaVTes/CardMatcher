using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public Text highscoreTxt;
    public static int highscore;

    public PlayerScore scoreData;

    void Start()
    {
        scoreData = SaveManager.LoadScore();
        if (scoreData != null) //if there is a saved highscore
            highscore = scoreData.playerHighscore;
    }


    public void CheckScore() 
    {
        highscoreTxt.gameObject.SetActive(!highscoreTxt.gameObject.activeInHierarchy); //turn on and off

        if (BoardManager.score > highscore)
        { 
            highscore = BoardManager.score;
            //save new highscore
            scoreData.playerHighscore = highscore;
            SaveManager.SaveData(scoreData);
        }

        //set new highscore if it's higher than previous one
        highscoreTxt.text = "Your highest score = " + highscore.ToString();
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


}
