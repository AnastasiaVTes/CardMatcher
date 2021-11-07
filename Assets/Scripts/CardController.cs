using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public BoardManager boardManager;

    private Image img;
    public Sprite[] faces;
    public Sprite back;
    public int faceIndex;

    public bool matched = false;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();
        img = gameObject.GetComponent<Image>();
    }

    public void Click()
    {
        if (matched == false)
        {
            if(img.sprite == back)
            {
                if (boardManager.canMatch == true)
                {
                    img.sprite = faces[faceIndex];
                    boardManager.MatchCards(this);
                }
            }
        }
    }

    public void ShowFace() 
    {
        img.sprite = faces[faceIndex];
    }

    public void ResetCard()
    {
        img.sprite = back;
    }

}
