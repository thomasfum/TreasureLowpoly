using Siccity.GLTFUtility;
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
    static private SpriteRenderer TreasureUI =null;

    static private AudioSource audioSource;


    static private int MaxLevel = 5;
    static private int[] TotalTreasurePerLevel = { 3, 3, 3, 3, 3 };
    public static LevelManager instance;

    private Rect Safe;

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
        if (GameObject.Find("TreasureUI") != null)
        {
            TreasureUI = GameObject.Find("TreasureUI").GetComponent<SpriteRenderer>();
        }
        else
            Debug.LogError("TreasureUI not found");
        


        audioSource = GameObject.Find("sound_3").GetComponent<AudioSource>();
        /*
         Safe = Screen.safeArea;
         Debug.Log("safe x="+(int)Safe.xMin);
         //TxtScore..transform.position.x = 100;
         TxtScore.GetComponent<RectTransform>().localPosition += new Vector3((int)+Safe.xMin, 0, 0);
         TxtLevel.GetComponent<RectTransform>().localPosition += new Vector3((int)+Safe.xMin, 0, 0);
        */

        TxtScore.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width/30, 0, 0);
        TxtLevel.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width / 30, 0, 0);
        TreasureUI.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width / 30, 0, 0);

        /*
        // Create new mesh.
        AnimationClip[] animClips;
        GameObject result = Importer.LoadFromFile(Application.dataPath + "/flamingo.glb", new ImportSettings(), out animClips);

        // Setup animation, if there is any.
        // Taking only the first clip for now. Should be pretty easy to extend it  to generalize
        if (animClips.Length > 0)
        {
            Animation anim = result.AddComponent<Animation>();
            animClips[0].legacy = true;
            anim.AddClip(animClips[0], animClips[0].name);
            anim.Play(animClips[0].name);
            
        }
        */

    }

    // Update is called once per frame
    void Update()
    {

      //  MyLog.Log("->" + Screen.safeArea.xMin+ " - "+Screen.safeArea.xMax+ " - " + Screen.safeArea.yMin + " - " + Screen.safeArea.yMax);
    }

    private static void DisplayScore()
    {
        if (TxtScore != null)
            TxtScore.text = TreasureCount + " / " + TotalTreasurePerLevel[Level-1];
    }
    private static void DisplayLevel()
    {
        if (TxtLevel != null)
            TxtLevel.text = "Level " + Level +" / " + MaxLevel;
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

    public static int GetScore()
    {
        return TreasureCount;
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
