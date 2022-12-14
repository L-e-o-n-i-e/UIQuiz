using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveButton : MonoBehaviour
{
    GameObject scrlVGreen;
    GameObject scrlVRed;
    Transform ContentGreen;
    Transform ContentRed;
    private void Awake()
    {
         scrlVGreen = GameObject.Find("Scroll View Green");
         scrlVRed = GameObject.Find("Scroll View Red");
    }
    public void Move()
    {
        ContentGreen = scrlVGreen.transform.GetChild(0).GetChild(0);
        ContentRed = scrlVRed.transform.GetChild(0).GetChild(0);

        Transform parentT = gameObject.transform.parent;

        if (GameObject.ReferenceEquals(parentT.gameObject, ContentGreen.gameObject))
            gameObject.transform.SetParent(ContentRed);
        else
        {
            gameObject.transform.SetParent(ContentGreen);
        }
    }
}
