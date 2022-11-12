using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Objects in the Scene
    public GameObject scrlVGreen;
    public GameObject scrlVRed;
    public GameObject submitButton;
    AudioSource audioSource;
    public AudioClip happySound;
    public AudioClip unhappySound;
    Button submitBtnBtnCmpnt;
    Image submitBtnImgCmpnt;
    Transform ContentGreen;
    Transform ContentRed;
    #endregion

    #region Timer Related
    public int timeBeforeReset = 2;
    public float timeBeforeStart = 4;
    float timeOfReset;
    float timeofStart;
    #endregion

    #region Cards
    public Button buttonPrefab;
    public int nbCardsToSpawn = 4;
    public int increaseNbWhenWin = 2;
    Button cardToSpawn;
    bool spawnedAlready = false;
    bool cardsAreWhite = false;
    Dictionary<string, Color> btnDictionary;
    int nbOfGreensInTotal = 0;
    List<GameObject> cards;
    List<Image> listOfImgCmpntOfCards;
    List<Button> listOfbtnCmpntOfCards;
    #endregion

    #region Game Flow
    enum GamePhase { SettingButtons, MovingButtons, CheckAnswer, Reset }
    GamePhase gamePhase;
    bool win = false;
    bool IsVerified = false;
    bool isReset = false;
    #endregion

    private void Awake()
    {
        //Caching Content Objects
        ContentGreen = scrlVGreen.transform.GetChild(0).GetChild(0);
        ContentRed = scrlVRed.transform.GetChild(0).GetChild(0);
        
        submitBtnBtnCmpnt = submitButton.GetComponent<Button>();
        submitBtnImgCmpnt = submitButton.GetComponent<Image>();
        audioSource = gameObject.GetComponent<AudioSource>();

    }

    void Start()
    {
        //prepare for caching
        btnDictionary = new Dictionary<string, Color>();
        cards = new List<GameObject>();
        listOfImgCmpntOfCards = new List<Image>();
        listOfbtnCmpntOfCards = new List<Button>();

        gamePhase = GamePhase.SettingButtons;
        ResetTime();

    }

    void Update()
    {
        switch (gamePhase)
        {
            case GamePhase.SettingButtons:
                SettingButtons(nbCardsToSpawn);

                break;
            case GamePhase.MovingButtons:
                MovingButtons();

                break;
            case GamePhase.CheckAnswer:
                CheckAnswer();

                break;
            case GamePhase.Reset:
                ResetRound(win);

                break;
            default:
                Debug.Log("Fallen into the black hole...?!");
                break;
        }

    }

    private void SettingButtons(int nbCardsToSpawn)
    {
        if (!spawnedAlready)
        {
            submitBtnBtnCmpnt.enabled = false;

            for (int i = 0; i < nbCardsToSpawn; i++)
            {
                cardToSpawn = GameObject.Instantiate(buttonPrefab, ContentGreen);

                Button btnCompntOfCard = cardToSpawn.gameObject.GetComponent<Button>();
                btnCompntOfCard.enabled = false;

                // Cards are numbered randomly 
                string randomNumber = RandomNumberToString();
                if (btnDictionary.ContainsKey(randomNumber))
                    randomNumber = RandomNumberToString();

                cardToSpawn.GetComponentInChildren<Text>().text = randomNumber;

                // Cards are colored randomly
                Color colorOfTheCard = RandomColor();
                if (colorOfTheCard.Equals(Color.green))
                    nbOfGreensInTotal++;

                Image imgCompntOfCard = cardToSpawn.GetComponentInChildren<Image>();
                imgCompntOfCard.color = colorOfTheCard;

                //Cards can move
                MoveButton move = cardToSpawn.GetComponent<MoveButton>();
                btnCompntOfCard.onClick.AddListener(move.Move);

                //Caching
                btnDictionary.Add(randomNumber, colorOfTheCard);
                cards.Add(cardToSpawn.gameObject);
                listOfbtnCmpntOfCards.Add(btnCompntOfCard);
                listOfImgCmpntOfCards.Add(imgCompntOfCard);

            }
            spawnedAlready = true;
        }

        //Moves to Guessing phase
        if (Time.time >= timeofStart)
        {
            gamePhase = GamePhase.MovingButtons;
        }
    }

    private void CardsGoWhite()
    {

        if (!cardsAreWhite)
        {
            for (int i = 0; i < nbCardsToSpawn; i++)
            {
                listOfbtnCmpntOfCards[i].enabled = true;
                listOfImgCmpntOfCards[i].color = Color.white;
            }
            cardsAreWhite = true;
        }

    }

    public void MovingButtons()
    {
        CardsGoWhite();
        submitBtnBtnCmpnt.enabled = true;
    }

    public void SubmitClicked()
    {
        ResetTime();
        gamePhase = GamePhase.CheckAnswer;
    }
    
    public void CheckAnswer()
    {
        if (!IsVerified)
        {
            submitBtnBtnCmpnt.enabled = false;
            UnclickableCards();

            bool cardIsGreen = true;
            int greenCardsFound = 0;

            //Verify ScrollView Green
            int nbChildInContentGreen = ContentGreen.childCount;
            int i = 0;
            while (i < nbChildInContentGreen && cardIsGreen)
            {
                string cardNumber = ContentGreen.GetChild(i).gameObject.GetComponentInChildren<Text>().text;
                
                if (btnDictionary.ContainsKey(cardNumber))
                {
                    Color colorOfTheCard = btnDictionary[cardNumber];

                    if (!colorOfTheCard.Equals(Color.green))
                        cardIsGreen = false;
                    else
                        greenCardsFound++;
                }
                else
                { 
                    Debug.Log(cardNumber + " not found");
                }
                i++;
            }


            if (cardIsGreen && (nbOfGreensInTotal - greenCardsFound == 0))
            {
                win = true;
                Win();
            }
            else
            {
                win = false;
                Lose();
            }
        }
        
        if (Time.time >= timeOfReset)
        {
            gamePhase = GamePhase.Reset;
        }
    }

    private void Win()
    {
        submitBtnImgCmpnt.color = Color.green;
        nbCardsToSpawn = nbCardsToSpawn + increaseNbWhenWin;
        PlaySound(happySound);
        IsVerified = true;
    }

    private void Lose()
    {
        submitBtnImgCmpnt.color = Color.red;
        nbCardsToSpawn = 4;
        PlaySound(unhappySound);
        IsVerified = true;
    }

    private void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

    private void ResetRound(bool win)
    {        
        ClearCards();
        ResetVariables();
        ResetTime();

        submitBtnImgCmpnt.color = Color.white;

        gamePhase = GamePhase.SettingButtons;

    }

    private void ResetTime()
    {
        timeofStart = Time.time + timeBeforeStart;
        timeOfReset = Time.time + timeBeforeReset;
    }

    private void ClearCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject.Destroy(cards[i].gameObject);
        }
    }

    private void UnclickableCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            listOfbtnCmpntOfCards[i].enabled = false;
        }
    }

    private void ResetVariables()
    {

        nbOfGreensInTotal = 0;
        cardsAreWhite = false;
        spawnedAlready = false;
        IsVerified = false;

        //Clear the Collections
        cards.Clear();
        btnDictionary.Clear();
        listOfbtnCmpntOfCards.Clear();
        listOfImgCmpntOfCards.Clear();
    }

    private string RandomNumberToString()
    {
        return Random.Range(1, 1000).ToString();
    }

    private Color RandomColor()
    {
        return Random.Range(0, 2) == 0 ? Color.green : Color.red;
    }

}

