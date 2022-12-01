using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class Pendulum : MonoBehaviour {
    
    [Range(0f, 360f)]
    public float angle;
    
    [Range(0f, 5f)]
    public float speed;

    [Range(0f, 10f)]
    public float startTime = 0.0f;
    
    
    private Quaternion start, end;

    private void Start()
    {
        start = PendulumRotation(angle);
        end = PendulumRotation(-angle);
    }

    private void FixedUpdate()
    {
        startTime += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(start, end, (Mathf.Sin(startTime * speed + Mathf.PI / 2) + 1.0f) / 2.0f);
    }

    void ResetTimer()
    {
        startTime = 0.0f;
    }

    Quaternion PendulumRotation(float angle)
    {
        var pendulumRotation = transform.rotation;
        var angleZ = pendulumRotation.eulerAngles.z + angle;

        if (angleZ > 180)
        {
            angleZ -= 360;
        }
        else if (angleZ < -180)
        {
            angleZ += 360;
        }
        
        pendulumRotation.eulerAngles = new Vector3(pendulumRotation.eulerAngles.x, pendulumRotation.y, angleZ);
        return pendulumRotation;
    }
}
