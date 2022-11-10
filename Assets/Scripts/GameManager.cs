using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    
    public GameObject scrlVGreen;
    public GameObject scrlVRed;
    public GameObject submitButton;
    public Button buttonPrefab;
    Button buttonToSpawn;
    

    Transform ContentGreen;
    Transform ContentRed;
    float nbBttSpawned = 4;

    private void Awake()
    {
        ContentGreen = scrlVGreen.transform.GetChild(0).GetChild(0);
        ContentRed = scrlVRed.transform.GetChild(0).GetChild(0);

        

    }
    void Start()
    {
       

        for (int i = 0; i < nbBttSpawned; i++)
        {
            buttonToSpawn = GameObject.Instantiate(buttonPrefab, ContentGreen);

            Button button = buttonToSpawn.gameObject.GetComponent<Button>();

            buttonToSpawn.GetComponentInChildren<Text>().text = RandomNumberInString();
            buttonToSpawn.GetComponentInChildren<Image>().color = RandomColor();
            MoveButton move = buttonToSpawn.GetComponent<MoveButton>();

            button.onClick.AddListener(move.Move);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private string RandomNumberInString()
    {
        return Random.Range(1, 1000).ToString();
    }

    private Color RandomColor()
    {
        return Random.Range(0, 2) == 0 ? Color.green : Color.red;
    }

    //public void MoveItToOtherScrllView()
    //{
    //    Debug.Log(name + "appuye!");


    //    Transform parentT = gameObject.transform.parent;

    //    if (GameObject.ReferenceEquals(parentT.gameObject, ContentGreen.gameObject))
    //        gameObject.transform.SetParent(ContentRed);
    //    else
    //    {
    //        gameObject.transform.SetParent(ContentGreen);
    //    }
    //}
}
