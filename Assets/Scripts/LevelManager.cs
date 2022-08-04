using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static int Level =1;
    
    private static int TreasureCount=0;


    static private TextMeshProUGUI TxtScore = null;
    static private TextMeshProUGUI TxtLevel = null;
    static private TextMeshProUGUI TxtMessage = null;

    static private AudioSource audioSource;


    static private int MaxLevel = 4;
    static private int[] TotalTreasurePerLevel = { 2, 3, 4, 5 };
    public static LevelManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (GameObject.Find("HUD_Score") != null)
        {
            TxtScore = GameObject.Find("HUD_Score").GetComponent<TextMeshProUGUI>();
            DisplayScore();
        }
        if (GameObject.Find("HUD_Level") != null)
        {
            TxtLevel = GameObject.Find("HUD_Level").GetComponent<TextMeshProUGUI>();
            DisplayLevel();
        }
        if (GameObject.Find("HUD_Message") != null)
        {
            TxtMessage = GameObject.Find("HUD_Message").GetComponent<TextMeshProUGUI>();
            TxtMessage.text = "";
        }

        audioSource = GameObject.Find("sound_3").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private static void DisplayScore()
    {
        if (TxtScore != null)
            TxtScore.text = TreasureCount + " / " + TotalTreasurePerLevel[Level-1];
    }
    private static void DisplayLevel()
    {
        if (TxtLevel != null)
            TxtLevel.text = "Level " + Level;
    }
    private static void DisplayMessage(string message, bool exit)
    {
        TxtMessage.text = message;
        instance.StartCoroutine(Fade(exit));
    }
    private static IEnumerator Fade(bool exit)
    {
        Color c = TxtMessage.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
        {
            if (alpha < 0.1)
            {
                TxtMessage.text = "";

                if (exit == true)
                {
                    GameObject Main = GameObject.Find("Main");
                    Main?.SendMessage("BackToWelcomeScene");
                }

            }
            c.a = alpha;
            TxtMessage.color = c;
            //Debug.Log("Fade:"+alpha);
            if(alpha == 1f)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(0.2f);
        }
    }
    public static int GetCurrentLevel()
    {
        return Level;
    }
    public static int GetMaxLevel()
    {
        return MaxLevel;
    }
    public static void IncreaseTreasureCount()
    {
        

        TreasureCount++;
        if(TreasureCount >= TotalTreasurePerLevel[Level - 1])
        {
            audioSource.Play();
            TreasureCount = 0;
            Level++;
            if (Level > MaxLevel)
            {
                Level--;
                DisplayMessage("Finished", true);
            }
            else
                DisplayMessage("Level Completed",false);
            DisplayLevel();
        }
        DisplayScore();
    }
}
