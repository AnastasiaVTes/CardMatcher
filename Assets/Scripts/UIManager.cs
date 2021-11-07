using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gamescr;
    public GameObject winscr;
    public GameObject losescr;

    public GameObject winScore;
    public GameObject loseScore;

    BoardManager boardM;

    void Start()
    {
        boardM = GetComponent<BoardManager>();
    }


    public void NextLv() 
    {
        winscr.SetActive(false);
        losescr.SetActive(false);
        gamescr.SetActive(true);

        foreach (Transform child in boardM.CardGrid.transform) //clean used card children
        {
            Destroy(child.gameObject);
        }
    }
    

    public void WinCondition(int finalscore) 
    {
        //show congratulations screen and the score
        gamescr.SetActive(false);
        winscr.SetActive(true);

        winScore.GetComponent<Text>().text = "Your score is = " + finalscore.ToString();
    }


    public void LoseCondition(int finalscore)
    {
        //hide game ui and show lose ui
        gamescr.SetActive(false);
        losescr.SetActive(true);

        loseScore.GetComponent<Text>().text = "Your score is = " + finalscore.ToString();
    }

}
