using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    //countdown
    public Text countdown;
    public float startTime;
    public float timeRemaining;

    public bool hasTimeRunOut;
    private bool wasSpawned;

    //aka difficulty
    int startCardNum;
    //current score
    public static int score;
    public PlayerScore scoreData; //highscore

    public GameObject cardPrefab;
    public GameObject CardGrid;
    public GameObject testcard;

    //ui
    UIManager uiManager;

    //cards array
    List<GameObject> cardList;
    List<int> indexList;
    int matchedCards; //all successfully matched cards

    //matching
    public bool canMatch = true; //so we can't match more while previous pair havent finished
    private CardController matchA;
    private CardController matchB;

    //match sound
    public AudioClip correctSound, failSound;
    AudioSource audSource;
    
    void Start()
    {
        hasTimeRunOut = false;
        wasSpawned = false;

        score = 0;
        startCardNum = 8; //starting difficulty
        matchedCards = 0;

        startTime = 60;
        timeRemaining = 60;

        uiManager = this.gameObject.GetComponent<UIManager>();
        audSource = this.GetComponent<AudioSource>();

        cardList = new List<GameObject>();

        scoreData = SaveManager.LoadScore();

        SpawnCards();
    }

    void Update()
    {
        if (hasTimeRunOut == false) //timer
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                int seconds = (int)timeRemaining;
                countdown.text = seconds.ToString();
            }
            else
            {
                timeRemaining = 0;
                hasTimeRunOut = true;
                CalculateScore();
            }
        }
    }

    public void SpawnCards()
    {
        //create a grid
        if (CardGrid.GetComponent<FlexibleGridLayout>() == null)
        {
            CardGrid.AddComponent<FlexibleGridLayout>();
            CardGrid.GetComponent<FlexibleGridLayout>().spacing = new Vector2(15, 15);
        }
        
        for (int i = 0; i < startCardNum; i++)
        {
            GameObject card = Instantiate(cardPrefab, new Vector3(cardPrefab.transform.position.x, cardPrefab.transform.position.y, cardPrefab.transform.position.z), Quaternion.identity);
            card.transform.SetParent(CardGrid.transform, false);
            cardList.Add(card);
        }
        Debug.Log("initial cards in list = " + cardList.Count);

        ShuffleBoard();
        wasSpawned = true;
    }

    #region Match

    public void MatchCards(CardController cardInd) 
    {
        if (matchA == null) //first card
        {
            matchA = cardInd;
        }
        else //second card
        {
            matchB = cardInd;
            //match both cards
            if (matchA.faceIndex == matchB.faceIndex)
            {
                if (cardList.Count > 0)
                {
                    cardList.Remove(matchA.gameObject);
                    cardList.Remove(matchB.gameObject);
                    Debug.Log("cards remaining in list = " + cardList.Count);
                }
                //set both as matched
                Debug.Log("matched");
                //play right sound
                audSource.clip = correctSound;
                audSource.Play();

                matchedCards = matchedCards + 2;

                matchA.matched = true;
                matchB.matched = true;

                //save to matched array in the case the board needs reshuffling

                //reset both active indexes
                matchA = null;
                matchB = null;

                if (cardList.Count == 0)//if list empty then you win
                    CalculateScore();
            }
            else 
            {
                //reset both cards to initial state
                StartCoroutine(WaitAndReset());
                //play wrong sound
                audSource.clip = failSound;
                audSource.Play();

                //lose a point for every failed match
                if (score > 0)
                    score = score--;

                Debug.Log("not a match");
            }
        }
    }

    IEnumerator WaitAndReset() //wait so you can see the second card face
    {
        canMatch = false;

        yield return new WaitForSeconds(1f);

        matchA.ResetCard();
        matchB.ResetCard();

        //reset both active indexes
        matchA = null;
        matchB = null;
        canMatch = true;
    }
    #endregion

    #region Shuffle

    public void ShuffleBoard() 
    {
        if (cardList.Count == startCardNum) //shuffling fresh deck
        {
            //array with all indexes of pairs
            int[] facesIndexes = new int[cardList.Count/2]; //because pairs
            for (int i = 0; i < facesIndexes.Length; i++)
            {
                int randomIndex = Random.Range(1, 16);
                if (!facesIndexes.Contains(randomIndex))
                    facesIndexes[i] = randomIndex;
            }
            indexList = facesIndexes.ToList<int>();

            List<GameObject> tempCards = new List<GameObject>(cardList); //clone list into expendable temp list with same values
            foreach (int index in indexList)
            {
                //1st element
                GameObject temp = tempCards.ElementAt(Random.Range(0, tempCards.Count));
                
                temp.GetComponent<CardController>().faceIndex = index;
                tempCards.Remove(temp);
                //its pair
                temp = tempCards.ElementAt(Random.Range(0, tempCards.Count));
                temp.GetComponent<CardController>().faceIndex = index;
                tempCards.Remove(temp);
            }
        }
        else
        {
            int[] facesIndexes = new int[cardList.Count];
            for (int i = 0; i < facesIndexes.Length; i++)
            {
                facesIndexes[i] = cardList[i].GetComponent<CardController>().faceIndex; //fill with existing card indexes
            }
            indexList = facesIndexes.ToList<int>();

            int indexRange = startCardNum - matchedCards;

            for (int i = 0; i < cardList.Count; i++)
            {
                int randomPos = Random.Range(0, (indexRange));

                int ind = indexList.ElementAt(randomPos);

                cardList.ElementAt(i).GetComponent<CardController>().faceIndex = ind;

                indexList.RemoveAt(randomPos);
                indexRange--;
            }
        }
        if (wasSpawned == true) //add that it shows card faces for a second after reshuffle
        {
            StartCoroutine(WaitAfterShuffle());
        }

    }

    IEnumerator WaitAfterShuffle() //wait so you can see the second card face
    {
        canMatch = false;

        //show all faces first
        foreach (GameObject card in cardList) 
        {
            card.GetComponent<CardController>().ShowFace();
        }

        yield return new WaitForSeconds(2f);

        foreach (GameObject card in cardList)
        {
            card.GetComponent<CardController>().ResetCard();
        }
        canMatch = true;
    }

    #endregion

    #region Finish
    public void CalculateScore() 
    {
        if (score != 0)
        { 
            score = score + (int)(timeRemaining + startCardNum); //on next levels add new and old score
        }
        else 
        { 
        score = (int)(timeRemaining + startCardNum); //change startcardnum to all matched cards only                                         
        }

        if (hasTimeRunOut == false)
            uiManager.WinCondition(score);
        else uiManager.LoseCondition(score);
    }


    public void NextLevel() //maybe not reload scene but clean and respawn objects/timer?
    {
        uiManager.NextLv();
        //reset timer
        startTime = startTime + 5.0f; //+5 seconds
        timeRemaining = startTime;

        startCardNum = startCardNum + 2;
        wasSpawned = false;
        SpawnCards();

    }

    public void RetryLv() 
    {
        if (score > scoreData.playerHighscore) //check if current score is higher than saved highscore
        { 
            SaveManager.SaveData(scoreData); 
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload scene
    }

    public void MenuReturn()
    {
        SceneManager.LoadScene("MenuScene");
    }

    #endregion
}
