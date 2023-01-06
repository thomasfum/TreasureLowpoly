using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrot : MonoBehaviour
{


    // These are  'a' and 'b' in the ellipse formula, think aphelion and perihelion
    float trajectoryHeight = 4; // y
    float trajectoryWidth = 12; // x



    float timeSinceBeginning = 0;

    public float speed = 10;
    Vector3 startingPoint;
    // Start is called before the first frame update
    void Start()
    {
        startingPoint = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceBeginning += Time.deltaTime;
        var positionInEllipse = LerpEllipse(trajectoryWidth, trajectoryHeight, timeSinceBeginning, startingPoint, speed);
        Vector3 movementDirection = positionInEllipse - transform.localPosition;
        transform.localPosition = positionInEllipse;
        transform.rotation = Quaternion.LookRotation(movementDirection);


    }

    public Vector3 LerpEllipse(float horizontalAxis, float verticalAxis, float time, Vector3 center, float duration)
    {
        var x = center.x + (horizontalAxis * Mathf.Cos((time / duration) * Mathf.PI * 2));
        var z = center.z + (verticalAxis * Mathf.Sin((time / duration) * Mathf.PI * 2));
        var y = center.y + (Mathf.Cos((time / duration/2) * Mathf.PI * 2))/2;
        return new Vector3(x,y, z);
    }

}
