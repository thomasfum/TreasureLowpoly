using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToWelcome : MonoBehaviour
{


    public GameObject CanvasWelcome = null;
    public GameObject CanvasCredit = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ButtonWelcome()
    {
        Debug.Log("Button Welcome");
        CanvasCredit.SetActive(false);
        CanvasWelcome.SetActive(true);

    }
}
