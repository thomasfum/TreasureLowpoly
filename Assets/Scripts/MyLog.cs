using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyLog : MonoBehaviour
{

    static private TextMeshProUGUI Txt=null;
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("HUD_Text") != null)
         Txt = GameObject.Find("HUD_Text").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void Log(string txt)
    {
        if(Txt!=null)
            Txt.text = txt;
    }
}
