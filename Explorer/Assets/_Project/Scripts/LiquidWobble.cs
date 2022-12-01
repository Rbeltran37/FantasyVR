using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class LiquidWobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    public float MaxWobble = 0.01f;
    public float MaxNoiseWobble = 0.05f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    public Renderer renderer;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountWave;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float wobbleAmountToAddWave;
    float pulse;
    float time = 0.5f;
    public float reducer;

    private void Update()
    {
        /*DebugLogger.Info(rend.material.shader.name);
        if (rend.material.shader.name.CompareTo("Shader Graphs/Liquid") != 0)
            return;*/
        
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddWave = Mathf.Lerp(wobbleAmountToAddWave, 0, Time.deltaTime * (Recovery));
 
        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
        wobbleAmountWave = wobbleAmountToAddWave * Mathf.Sin(pulse * time);

        // send it to the shader
        renderer.material.SetFloat("WobbleX", wobbleAmountX);
        renderer.material.SetFloat("WobbleZ", wobbleAmountZ);
        renderer.material.SetFloat("NoiseWobble", wobbleAmountWave/reducer);
 
        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;
 
 
        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddWave += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxNoiseWobble, 0f, MaxNoiseWobble);
 
        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
 
 
 
}