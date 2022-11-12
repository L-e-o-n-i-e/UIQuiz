using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject scrlVGreen;
    public GameObject scrlVRed;
    public GameObject submitButton;
    AudioSource audioSource;
    public AudioClip happySound;
    public AudioClip unhappySound;
    Button submitBttn;
    Transform ContentGreen;
    Transform ContentRed;

    #region Timer Related
    public int nbBttSpawnedStart = 4;
    public int increaseWhenWin = 2;
    public int timeBeforeReset = 2;
    public float timeBeforeStart = 4;
    float timeOfReset;
    float timeofStart;
    #endregion


    public Button buttonPrefab;
    Button buttonToSpawn;
    bool btnSpawned = false;
    bool btnAreWhite = false;
    Dictionary<string, Color> btnDictionary;
    int nbOfGreens = 0;
    List<GameObject> cards;

    enum GamePhase { SettingButtons, MovingButtons, CheckAnswer, Reset }
    GamePhase gamePhase;
    bool win = false;
    bool IsVerified = false;
    bool isReset = false;


    private void Awake()
    {
        ContentGreen = scrlVGreen.transform.GetChild(0).GetChild(0);
        ContentRed = scrlVRed.transform.GetChild(0).GetChild(0);
        submitBttn = submitButton.GetComponent<Button>();
        audioSource = gameObject.GetComponent<AudioSource>();

    }
    void Start()
    {
        btnDictionary = new Dictionary<string, Color>();
        cards = new List<GameObject>();
        gamePhase = GamePhase.SettingButtons;
        ResetTime();

    }

    // Update is called once per frame
    void Update()
    {
        switch (gamePhase)
        {
            case GamePhase.SettingButtons:
                SettingButtons(nbBttSpawnedStart);

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
    private void SettingButtons(int nbBttSpawned)
    {
        if (!btnSpawned)
        {
            // Left side populated with correct number of cards 
            for (int i = 0; i < nbBttSpawned; i++)
            {
                buttonToSpawn = GameObject.Instantiate(buttonPrefab, ContentGreen);

                Button button = buttonToSpawn.gameObject.GetComponent<Button>();
                // Player cannot move cards 
                button.enabled = false;
                // Guess button deactivated
                submitBttn.enabled = false;

                // Cards are numbered randomly 
                string number = RandomNumberInString();
                if (btnDictionary.ContainsKey(number))
                {
                    number = RandomNumberInString();
                }
                buttonToSpawn.GetComponentInChildren<Text>().text = number;

                // Cards are colored
                Color color = RandomColor();
                //Counting nb of green Cards
                if (color.Equals(Color.green))
                {
                    nbOfGreens++;
                }
                buttonToSpawn.GetComponentInChildren<Image>().color = color;

                MoveButton move = buttonToSpawn.GetComponent<MoveButton>();

                button.onClick.AddListener(move.Move);

                //Populate the dictionary
                btnDictionary.Add(number, color);
                cards.Add(buttonToSpawn.gameObject);

            }
            btnSpawned = true;
        }

        // After a set time, moves to Guessing phase
        if (Time.time >= timeofStart)
        {
            Debug.Log("Moving Phase!");
            gamePhase = GamePhase.MovingButtons;
        }
    }

    private void ButtonsGoWhite()
    {

        if (!btnAreWhite)
        {
            for (int i = 0; i < nbBttSpawnedStart; i++)
            {
                Button button = ContentGreen.GetChild(i).GetComponent<Button>();
                Debug.Log(button.gameObject.name + i);
                Image imgButton = ContentGreen.GetChild(i).GetComponent<Image>();
                //Player can now move cards
                button.enabled = true;

                imgButton.color = Color.white;

            }
            btnAreWhite = true;
        }

    }

    public void MovingButtons()
    {

        //Card colors disappear
        ButtonsGoWhite();

        //Player pressing Guess button will move to “Calculate End” phase
        submitBttn.enabled = true;
    }

    public void SubmitClicked()
    {
        //ResetTime();
        Debug.Log("Submit Clicked");
        timeOfReset = Time.time + timeBeforeReset;
        gamePhase = GamePhase.CheckAnswer;
    }


    public void CheckAnswer()
    {
        Debug.Log("CheckAnswer()");
        if (!IsVerified)
        {
            //Unclickable buttons
            submitBttn.GetComponent<Button>().enabled = false;
            UnclickableCards();

            bool isGreen = true;
            int GreenFound = 0;

            int nbChild = ContentGreen.childCount;
            Debug.Log("nb of child in green view : " + nbChild);

            //Verify ScrollView Green
            int i = 0;
            while (i < nbChild && isGreen)
            {
                string buttonNumber = ContentGreen.GetChild(i).gameObject.GetComponentInChildren<Text>().text;

                Debug.Log("numero du bouton = " + buttonNumber);
                if (btnDictionary.ContainsKey(buttonNumber))
                {
                    Color valueColor = btnDictionary[buttonNumber];
                    Debug.Log(valueColor + "is the color of : " + buttonNumber);
                    if (!valueColor.Equals(Color.green))
                    {
                        isGreen = false;
                        Debug.Log("Mauvaise reponse!");
                    }
                    else
                    {
                        //Keep track nb of green cards found
                        GreenFound++;
                    }

                }
                else
                {
                    Debug.Log(buttonNumber + "Ne fait pas partie du Dictionnaire ...!!");
                }
                i++;
            }


            if (isGreen)
            {

                if (nbOfGreens - GreenFound == 0)
                {
                    Debug.Log("win!");
                    win = true;
                }
                else
                {
                    Debug.Log("Lose!");

                    win = false;
                }


            }
            else
            {
                Debug.Log("Lose!");

                win = false;
            }



            if (win)
            {
                Win();
            }
            else
            {
                Lose();

            }
        }

        Debug.Log("timeOfReset in CheckAnswer: " + timeOfReset);
        if (Time.time >= timeOfReset)
        {
            Debug.Log("Enter Reset Phase");

            gamePhase = GamePhase.Reset;
        }
    }

    private void Win()
    {
        //guess button turns green 
        //Add 2 more cards next round
        // Play happy sound
        submitBttn.GetComponent<Image>().color = Color.green;
        nbBttSpawnedStart = nbBttSpawnedStart + increaseWhenWin;
        audioSource.PlayOneShot(happySound);
        IsVerified = true;
    }
    private void Lose()
    {
        //If you lose, 
        //guess button turns red 
        //Next round, start over from starting cards 
        //Play unhappy sou
        submitBttn.GetComponent<Image>().color = Color.red;
        nbBttSpawnedStart = 4;
        audioSource.PlayOneShot(unhappySound);
        IsVerified = true;
    }

    private void ResetRound(bool win)
    {
        
            Debug.Log("ResetRound()");
            //Game calculates if you win or loss

            ClearCards();
            ResetVariables();
            ResetTime();

            submitBttn.GetComponent<Image>().color = Color.white;

            gamePhase = GamePhase.SettingButtons;

    }
    private void ResetTime()
    {
        timeofStart = Time.time + timeBeforeStart;
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
            cards[i].gameObject.GetComponent<Button>().enabled = false;
        }
    }
    private void ResetVariables()
    {

        nbOfGreens = 0;
        btnAreWhite = false;
        cards.Clear();
        btnDictionary.Clear();
        btnSpawned = false;
        IsVerified = false;
    }

    private string RandomNumberInString()
    {
        return Random.Range(1, 999).ToString();
    }

    private Color RandomColor()
    {
        return Random.Range(0, 2) == 0 ? Color.green : Color.red;
    }


}

