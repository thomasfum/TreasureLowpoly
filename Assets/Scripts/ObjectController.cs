//-----------------------------------------------------------------------
// <copyright file="ObjectController.cs" company="Google LLC">
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
using UnityEngine;

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// The material to use when this object is inactive (not being gazed at).
    /// </summary>
    public Material InactiveMaterial;

    /// <summary>
    /// The material to use when this object is active (gazed at).
    /// </summary>
    public Material GazedAtMaterial;

    // The objects are about 1 meter in radius, so the min/max target distance are
    // set so that the objects are always within the room (which is about 5 meters
    // across).
    private const float _minObjectDistance = 2.5f;
    private const float _maxObjectDistance = 15f;
    private const float _minObjectHeight = 0.5f;
    private const float _maxObjectHeight = 3.0f;

    private Renderer _myRenderer;
    private AudioSource audioSource;
    Transform[][] allPositions;
    private static int selectedPos=-1;

    private void findTreasurePositions()
    {
        allPositions = new Transform[LevelManager.GetMaxLevel()][];
        for (int i = 0; i < LevelManager.GetMaxLevel(); i++)
        { 
            GameObject TreasurePositions = GameObject.Find("TreasurePositions_L"+(i+1));
            allPositions[i] = TreasurePositions.GetComponentsInChildren<Transform>(true);
        }
        /*
         * Debug.Log("---> TreasurePositions Nb: " + (allPositions.Length - 1));
        foreach (Transform child in allPositions)
        {
            if (child != allPositions[0])
                Debug.Log("---> child: " + child.name);
        }
        */
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        SetMaterial(false);
        audioSource = GameObject.Find("sound_1").GetComponent<AudioSource>();
        findTreasurePositions();
    }

   public static void InitFirst()
    {
        //init
        GameObject Treasure = GameObject.Find("Treasure");
        //Debug.Log("---> Treasure:" + Treasure);
        Transform[] allTreasure = Treasure.GetComponentsInChildren<Transform>(true);
        //Debug.Log("---> allTreasure Nb: " + (allTreasure.Length - 1));
        allTreasure[1].gameObject.SetActive(true);
        allTreasure[1].SendMessage("Init");
    }



    private static IEnumerator Fade(GameObject g, GameObject randomSib, Vector3 pos, Quaternion rot)
    {
        Color c = g.GetComponent<MeshRenderer>().material.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
        {
            if (alpha < 0.1)
            {
                c.a = 1;
                g.GetComponent<MeshRenderer>().material.color = c;
                g.transform.parent.position = pos;
                g.transform.parent.rotation = rot;
                randomSib.SetActive(true);
                g.SetActive(false);
                break;
            }
            c.a = alpha;
            g.GetComponent<MeshRenderer>().material.color = c;
            Debug.Log("Fade:"+alpha);
             yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// Teleports this instance randomly when triggered by a pointer click.
    /// </summary>
    public void TeleportRandomly()
    {
        //find
        int NewselectedPos = Random.Range(1, allPositions[LevelManager.GetCurrentLevel()-1].Length);
        if (selectedPos == NewselectedPos)
            NewselectedPos++;
        if (NewselectedPos > allPositions[LevelManager.GetCurrentLevel()-1].Length - 1)
            NewselectedPos = 1;
        selectedPos = NewselectedPos;

        // Picks a random sibling, activates it and deactivates itself.
        int sibIdx = transform.GetSiblingIndex();
        int numSibs = transform.parent.childCount;
        sibIdx = (sibIdx + Random.Range(1, numSibs)) % numSibs;
        GameObject randomSib = transform.parent.GetChild(sibIdx).gameObject;
        
        // Moves the parent to the new position (siblings relative distance from their parent is 0).
        
        transform.parent.position = allPositions[LevelManager.GetCurrentLevel()-1][selectedPos].position;
        transform.parent.rotation = allPositions[LevelManager.GetCurrentLevel()-1][selectedPos].rotation;
       

       randomSib.SetActive(true);

      //  StartCoroutine(Fade(gameObject, randomSib, allPositions[LevelManager.GetCurrentLevel() - 1][selectedPos].position, allPositions[LevelManager.GetCurrentLevel() - 1][selectedPos].rotation));

        gameObject.SetActive(false);
        SetMaterial(false);
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        SetMaterial(true);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        SetMaterial(false);
    }

    /// <summary>
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnPointerClick()
    {
        LevelManager.IncreaseTreasureCount();
        audioSource.Play();
        TeleportRandomly();
    }

    public void Init()
    {
        findTreasurePositions();
        TeleportRandomly();
    }
    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    /// <param name="gazedAt">
    /// Value `true` if this object is being gazed at, `false` otherwise.
    /// </param>
    private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }
}
