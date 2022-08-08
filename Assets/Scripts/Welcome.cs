using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Google.XR.Cardboard;
using TMPro;

public class Welcome : MonoBehaviour
{
    


    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void StartNoXR(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }


    public IEnumerator StartXR(string scene)
    {
        if (XRGeneralSettings.Instance == null)//probably in unity
        {
            Debug.Log("XRGeneralSettings.Instance=null");
            StartNoXR("LowPloly");
        }
        else
        {
            Debug.Log("Initializing XR...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
            }
            else
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
        }
    }
    public void StopXR()
    {
        Debug.Log("Stopping XR...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR stopped completely.");
    }
   
    public void ButtonVR()
    {
        Debug.Log("Button VR");
        StartCoroutine(StartXR("LowPloly"));
    }
    public void ButtonNoVR()
    {
        Debug.Log("Button No VR");
        StartNoXR("LowPloly");
    }

    public void ButtonQuit()
    {
       Debug.Log("Button Quit");
       Application.Quit();
    }
}
