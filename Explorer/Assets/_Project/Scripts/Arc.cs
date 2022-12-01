using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;

public class Arc : MonoBehaviour
{
    Vector3[] points = new Vector3[100];
    bool draw = true;
    LineRenderer lineRenderer;
    int count = 0;
    Vector3[] followPoints;
    private bool follow = false;
    int currentWaypoint = 1;
    float speed = .05f;
    private float currentSpeed;
    float timeBetweenPoints = .2f;

    private void Start () {
        if(GetComponent<Renderer>().material.HasProperty("_Color"))
            GetComponent<Renderer>().material.color = Color.cyan;
        
        currentSpeed = speed;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material (Shader.Find("Particles/Additive"));
        //lineRenderer.SetColors(Color.yellow,Color.green);
        lineRenderer.SetWidth(1f,1f);

        while(draw && count < points.Length && GetComponent<MeshRenderer>().enabled){
            points[count] = transform.position;
            count++;
            //Invoke("NullFunction", timeBetweenPoints);
        }
    }

    private void NullFunction()
    {
        
    }

    private void Update () {
        lineRenderer.SetVertexCount(count);
        for(int i = 0; i < count; i++) {
            lineRenderer.SetPosition(i, points[i]);
        }
        if(follow){
            if(currentWaypoint < followPoints.Length){
                transform.position = Vector3.Lerp(followPoints[currentWaypoint-1],followPoints[currentWaypoint],currentSpeed);
                currentSpeed += speed;
                if(transform.position == followPoints[currentWaypoint]){
                    currentWaypoint++;
                    currentSpeed = 0;
                }
            }
            else{
                Debug.Log("Boom!!!!");
                transform.position = followPoints[followPoints.Length-1];
            }
        }
    }

    private void OnCollisionEnter (Collision other){
        draw = false;
        points[count] = other.contacts[0].point;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void FollowPoints(Vector3[] points, int length){
        followPoints = new Vector3[length];
        for(var i = 0; i < length; i++){
            followPoints[i] = points[i];
        }
        follow = true;
    }
}
