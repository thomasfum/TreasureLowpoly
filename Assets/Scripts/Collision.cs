using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    static bool col = false;
    static string colName = "";

    // Update is called once per frame
    void Update()
    {
        
    }
    public static bool isColliding()
    {
        return col;
    }

    public static string getCollidingName()
    {
        return colName;
    }

    void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Touching " + other.name);
        if (other.name.StartsWith("Rock") || other.name.StartsWith("Tree"))
        {
            col = true;
            colName = other.name;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Not Touching " + other.name);
        col = false;
        colName = "";
    }
 
    

}
