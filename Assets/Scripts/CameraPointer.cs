//Todo:
// - animate treasure ? fade
// - wind => no


//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Management;

/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour
{
    private const float _maxDistance = 10;
    private GameObject _gazedAtObject = null;
    private float fire_start_time=0;

    public SpriteRenderer GazeRing;
    public SpriteRenderer GazeRingTimer;
    private bool bGrowing=false;
    private float GrowingTime = 0;
    private bool bShrinking = false;
    private float ShrinkingTime = 0;
    
    public float sensitivity = 10f;
    public float maxYAngle = 80f;
    private Vector2 currentRotation;
    private bool onFloor = true;
    public GameObject [] info;
    public Transform Effect;
    private Transform currentFX = null;
    private const float _maxDistance2 = 1000;
    
    private AudioSource audioSource;
    

    private float TimeSinceLastTreasureOrInfo = 0;
    private bool needPad = false;
    private Canvas m_Canvas=null;

    [SerializeField] private RectTransform ExitImage;



    [SerializeField] private bl_Joystick Joystick;//Joystick reference for assign in inspector

    [SerializeField] private float Speed = 5;


    void Start()
    {
        GazeRingTimer.enabled = false;
        audioSource = GameObject.Find("sound_2").GetComponent<AudioSource>();
        ObjectController.InitFirst();
        TimeSinceLastTreasureOrInfo = 0;

        GameObject tempObject = GameObject.Find("Canvas");
        if (tempObject != null)
        {
            //If we found the object , get the Canvas component from it.
            m_Canvas = tempObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Canvas not found");
        }
    }

    public void Button_BackToWelcome()
    {
        GameObject Main = GameObject.Find("Main");
        Main?.SendMessage("BackToWelcomeScene");
    }
    private bool isObjectController(GameObject go)
    {
        if (go != null)
        {
            Component[] components = go.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                if (component.GetType().ToString() == "ObjectController")
                {
                    return true;
                }
            }
        }
        return false;
    }


    IEnumerator DestroyFx(float time)
    {
        yield return new WaitForSeconds(time);
        // Code to execute after the delay
        if(currentFX!=null)
            Destroy(currentFX.gameObject);
        currentFX = null;
    }

    // 1 left
    // 2 front left
    // 3 center
    // 4 front right
    // 5 right
    void ShowIndicator( int position)
    {
        currentFX= Instantiate(Effect, info[position - 1].transform);
        StartCoroutine(DestroyFx(5));
    }


    void OnDrawGizmosSelected()
    {
        GameObject Treasure = GameObject.Find("Treasure");
        Transform[] activeTreasure = Treasure.GetComponentsInChildren<Transform>(false);
        Vector3 cam = new Vector3(this.transform.forward.x, 0, this.transform.forward.z);
        Vector3 treasure = new Vector3(activeTreasure[0].localPosition.x - this.transform.position.x , 0, activeTreasure[0].localPosition.z-this.transform.position.z);

        Gizmos.color = Color.magenta;
        //Debug.Log("Gizmo");
        Gizmos.DrawRay(this.transform.localPosition, cam*10);
        Gizmos.DrawRay(this.transform.localPosition, treasure);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
        if (XRGeneralSettings.Instance != null)
        {
            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                Joystick.gameObject.SetActive(false);
                needPad = false;
            }
            else
                needPad = true;
        }
#else
        needPad = true;
#endif


        TimeSinceLastTreasureOrInfo += Time.deltaTime;
        if (TimeSinceLastTreasureOrInfo >= 10)//10sec
        {
            GameObject Treasure = GameObject.Find("Treasure");
            //Debug.Log("---> Treasure:" + Treasure);
            Transform[] activeTreasure = Treasure.GetComponentsInChildren<Transform>(false);
            if (activeTreasure.Length > 0)
            {
                // Vector2 cam = new Vector2(this.transform.forward.x, this.transform.forward.z);
                // Vector2 treasure = new Vector2( activeTreasure[0].position.x - this.transform.position.x,  activeTreasure[0].position.z- this.transform.position.z);

                Vector2 cam = new Vector2(transform.forward.x, transform.forward.z);
                Vector2 treasure = new Vector2( activeTreasure[0].localPosition.x- transform.position.x, activeTreasure[0].localPosition.z- transform.position.z);



                float Angle = Vector2.SignedAngle(cam, treasure);
                float Dist = Vector2.Distance(new Vector2(activeTreasure[0].localPosition.x , activeTreasure[0].localPosition.z ), new Vector2( transform.position.x, transform.position.z));
                
                //Debug.Log("Need Info about treasure: " + treasure + " vs " + cam+" a="+ Angle +" d="+Dist);
                
                    if ((Angle > -15) && (Angle < 15))
                    {
                        if (Dist > 10)
                        {
                            MyLog.Log("front:" + Angle);
                            Debug.Log("treasure in front:" + Angle);
                            ShowIndicator(3);
                        }
                    }
                    else
                    {
                        if (Angle < 0)
                        {
                            if (Angle > -25)
                            {
                                if (Dist > 7)
                                {
                                    MyLog.Log("front right:" + Angle);
                                    Debug.Log("front right:" + Angle);
                                    ShowIndicator(4);
                                }
                            }
                            if (Angle <= -25)
                            {
                                MyLog.Log("right:" + Angle);
                                Debug.Log("right:" + Angle);
                                ShowIndicator(5);
                            }
                        }
                        else
                        {
                            if (Angle > 25)
                            {
                                MyLog.Log("left:" + Angle);
                                Debug.Log("left:" + Angle);
                                ShowIndicator(1);
                            }
                            if (Angle <= 25)
                            {
                                if (Dist > 7)
                                {
                                    MyLog.Log("front left:" + Angle);
                                    Debug.Log("treasure at front left:" + Angle);
                                    ShowIndicator(2);
                                }
                            }
                        }
                    }
                

            }
            TimeSinceLastTreasureOrInfo = 0;
        }
        //------------------------------------------------------------------------------------------------------
        // Rotate camera with mouse in unity editor
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(1))
            {
                currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
                currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
                currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
                currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            }
        }


        //------------------------------------------------------------------------------------------------------
        // Rotate camera with pad
        if(needPad==true)
        {
            float v = Joystick.Vertical; //get the vertical value of joystick
            float h = Joystick.Horizontal;//get the horizontal value of joystick
            //Debug.Log("---> " + v + " , " + h);

            currentRotation.x += h * Speed/1.5f;
            currentRotation.y -= v * Speed / 1.5f;
            //currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

            //Debug.Log("---> "  + h+ "= "+ currentRotation.x);

            transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }

        //------------------------------------------------------------------------------------------------------
        // Casts ray towards camera's down direction, to detect floor and calculate height
        RaycastHit hitfloor;
        int layerMaskFloor = 1 << 7;
        //Vector3 testPos = transform.position;
        Vector3 testPos = transform.position + transform.forward;
        if (Physics.Raycast(testPos, Vector3.down, out hitfloor, _maxDistance2, layerMaskFloor))
        {
            Vector3 pos = hitfloor.point; //get the position where the ray hit the ground
                                          //shoot a raycast up from that position towards the object
            Ray upRay = new Ray(pos, transform.position - pos);

            //get a point (vector3) in that ray 1.6 units from its origin
            Vector3 upDist = upRay.GetPoint(1.6f);
            //smoothly interpolate its position
            transform.position = Vector3.Lerp(transform.position, upDist, 0.5f);
            onFloor = true;
        }
        else
            onFloor = false;


        //------------------------------------------------------------------------------------------------------
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed at
        RaycastHit hit;
        int layerMaskObjects = 1 << 6;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance, layerMaskObjects))
        {
            // Debug.Log("---->hit");
            // New GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {

                // New GameObject.
                if (isObjectController(_gazedAtObject))
                {
                    GazeRing.size = new Vector2(1f, 1f);
                    GazeRingTimer.enabled = false;
                    GazeRing.enabled = true;
                    _gazedAtObject?.SendMessage("OnPointerExit");
                    TimeSinceLastTreasureOrInfo = 0;
                }
                _gazedAtObject = hit.transform.gameObject;
                if (isObjectController(_gazedAtObject))
                {
                    _gazedAtObject.SendMessage("OnPointerEnter");
                    bGrowing = true;
                    GrowingTime = 0;
                    TimeSinceLastTreasureOrInfo = 0;
                    /*
                    fire_start_time = Time.time;
                    //MyLog.Log("hit object");
                    GazeRing.size= new Vector2(3f, 3f);
                    GazeRingTimer.size = new Vector2(3f, 3f);
                    GazeRingTimer.enabled= true;
                    GazeRing.enabled = false;
                    */
                }
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            if (_gazedAtObject != null)
            {
                bShrinking = true;
                ShrinkingTime = 0;
            }
            //if (isObjectController(_gazedAtObject))
            //    _gazedAtObject.SendMessage("OnPointerExit");
            _gazedAtObject = null;
            GazeRing.size = new Vector2(1f, 1f);
            GazeRingTimer.enabled = false;
            GazeRing.enabled = true;
            bGrowing = false;
            fire_start_time = 0;


        }
        /*
        // Checks for screen touches.
        if (Google.XR.Cardboard.Api.IsTriggerPressed)
        {
            _gazedAtObject?.SendMessage("OnPointerClick");
        }
        */

        if (bGrowing == true)
        {
            float durat = 0.3f;
            GrowingTime += Time.deltaTime;
            float valueToLerp = Mathf.Lerp(1f, 3f, GrowingTime / durat);
            GazeRing.size = new Vector2(valueToLerp, valueToLerp);
            if (GrowingTime > durat)
            {
                fire_start_time = Time.time;
                //MyLog.Log("hit object");
                GazeRing.size = new Vector2(3f, 3f);
                GazeRingTimer.size = new Vector2(3f, 3f);
                GazeRingTimer.enabled = true;
                GazeRing.enabled = false;
                bGrowing = false;
            }
        }
        if (bShrinking == true)
        {
            float durat = 0.2f;
            ShrinkingTime += Time.deltaTime;
            float valueToLerp = Mathf.Lerp(3f, 1f, ShrinkingTime / durat);
            GazeRing.size = new Vector2(valueToLerp, valueToLerp);
            if (ShrinkingTime > durat)
            {
                TimeSinceLastTreasureOrInfo = 0;
                bShrinking = false;
            }
        }
        if (fire_start_time != 0)
            if (Time.time - fire_start_time > 0.8f)
            {
                _gazedAtObject?.SendMessage("OnPointerClick");
                fire_start_time = 0;
                TimeSinceLastTreasureOrInfo = 0;
                //MyLog.Log("hit object fire");
            }
        if (GazeRingTimer.enabled || bGrowing==true)
        {
            //Rotate gaze
            GazeRingTimer.transform.Rotate(Vector3.forward, Time.deltaTime * 400);
            audioSource.loop = false;
        }
        else
        {
            if (onFloor == true)
            {

                bool col = Collision.isColliding();
                float angle = transform.rotation.eulerAngles.x;
                if (col == false)
                {
                    //move forward
                    //MyLog.Log("---> "+ angle);
                    //Debug.Log("---> "+ angle);
                    if ((angle > 8) && (angle < 38))
                    {
                        Vector3 dir = new Vector3();

                        if ((angle > 8) && (angle < 8 + 15))
                        {
                            //dir = (cam.transform.forward * angle * speed / 50);
                            dir = (transform.forward * angle / 800) * Time.deltaTime * 100;
                            audioSource.pitch = 0.8f + (angle / 40);
                        }
                        if ((angle >= 8 + 15) && (angle < 38))
                        {
                            //dir = (cam.transform.forward * (38 - angle) * speed / 50);
                            dir = (transform.forward * (38 - angle) / 800) * Time.deltaTime * 100;
                            audioSource.pitch = 0.8f + ((38-angle) / 40);
                        }

                        

                        dir.y = 0;
                        transform.position += dir;
                        if (!audioSource.isPlaying)
                        {
                            audioSource.loop = true;
                            audioSource.Play();
                        }
                    }
                    else
                        audioSource.loop = false;
                }
                else//collision
                {
                    // Debug.Log("------------>colllll+ " + Collision.getCollidingName());
                    audioSource.loop = false;
                }
            }
            else // not on floor
                audioSource.loop = false;
        }
       
    }
    
}
