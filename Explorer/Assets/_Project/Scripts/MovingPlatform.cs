using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(.3f, .7f);
        pointA = this.transform.position;
        pointB = new Vector3(this.transform.position.x + 1, pointA.y, pointA.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(pointA, pointB, Mathf.PingPong(Time.time * speed, 1));
    }
}
