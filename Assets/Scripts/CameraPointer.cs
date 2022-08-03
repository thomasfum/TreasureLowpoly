//Todo:
// - mode without VR
// - animate treasure ?
// - do not step on water
// - tree coliders ?
// - rock coliders 
// - sound when level completed


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
using UnityEngine.UI;


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

    private const float _maxDistance2 = 1000;
   
    private AudioSource audioSource;

    void Start()
    {
        GazeRingTimer.enabled = false;
        audioSource = GameObject.Find("sound_2").GetComponent<AudioSource>();
                
        ObjectController.InitFirst();
        
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
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {

        //------------------------------------------------------------------------------------------------------
        // Rotate camera with mouse in unity editor
        if (Input.GetMouseButton(0))
        {
            
            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }
        /*
                //float angleY = currentRotation.y;
                float angle = transform.rotation.eulerAngles.x;
                MyLog.Log("---> "+ angle);
                //Debug.Log("---> "+ angle);
                if ((angle > 10) && (angle < 35))
                {
                    Vector3 dir = (transform.forward / (2 * angle)) * Time.deltaTime * 100;
                    dir.y = 0;
                    transform.position += dir;
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                }
                else
                    audioSource.Stop();
                */

        //------------------------------------------------------------------------------------------------------
        // Casts ray towards camera's down direction, to detect fllor and calculate height
        RaycastHit hitfloor;
        int layerMaskFloor = 1 << 7;
        if (Physics.Raycast(transform.position, Vector3.down, out hitfloor, _maxDistance2, layerMaskFloor))
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
                }
                _gazedAtObject = hit.transform.gameObject;
                if (isObjectController(_gazedAtObject))
                {
                    _gazedAtObject.SendMessage("OnPointerEnter");
                    bGrowing = true;
                    GrowingTime = 0;
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
            if (GrowingTime> durat)
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
               
                bShrinking = false;
            }
        }
        if (fire_start_time != 0)
            if (Time.time - fire_start_time > 0.8f)
            {
                _gazedAtObject?.SendMessage("OnPointerClick");
                fire_start_time = 0;
                //MyLog.Log("hit object fire");
            }
        if (GazeRingTimer.enabled)
        {
            //Rotate gaze
            GazeRingTimer.transform.Rotate(Vector3.forward, Time.deltaTime*400);
            //audioSource.Stop();
            audioSource.loop = false;
        }
        else
        {
            if (onFloor == true)
            {
                //move forward
                float angle = transform.rotation.eulerAngles.x;
                //MyLog.Log("---> "+ angle);
                //Debug.Log("---> "+ angle);
                if ((angle > 8) && (angle < 35))
                {
                    /*
                    Vector3 dir = (transform.forward / (2 * angle)) * Time.deltaTime * 100;
                    audioSource.pitch = 20 / angle;
                    */
                    Vector3 dir = (transform.forward * angle / 800) * Time.deltaTime * 100;
                    audioSource.pitch = 0.8f + (angle / 40);

                    dir.y = 0;
                    transform.position += dir;
                    if (!audioSource.isPlaying)
                    {
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                }
                else
                    //audioSource.Stop();
                    audioSource.loop = false;
            }
            else
                //audioSource.Stop();
                audioSource.loop = false;
        }
     

       
    }
}
