using UnityEngine;

/// <summary>
/// Parabolic Missile
/// < para > Calculating trajectory and steering </para >
/// <para>ZhangYu 2019-02-27</para>
/// </summary>
public class Missile : MonoBehaviour {

    public Transform target; //target
    public float height = 16f; // parabolic height
    public float gravity = 9.8f; // gravitational acceleration
    private Vector3 position; //My position
    private Vector3 dest; //Target location
    private Vector3 velocity; //Motion Velocity
    private float time = 0; // Motion time
    public bool predicting;

    private void Start() {
        dest = target.position;
        position = transform.position;
        velocity = ParabolaPhysics.GetParabolaInitVelocity(position, dest, gravity, height, 0);
        transform.LookAt(ParabolaPhysics.GetParabolaNextPosition(position, velocity, gravity, Time.deltaTime));
    }

    private void Update() {
        if (predicting)
        {
            PredictPath();
        }
    }

    public void PredictPath()
    {
        // Computational displacement
        float deltaTime = Time.deltaTime;
        position = ParabolaPhysics.GetParabolaNextPosition(position, velocity, gravity, deltaTime);
        transform.position = position;
        time += deltaTime;
        velocity.y += gravity * deltaTime;

        // Computational steering
        transform.LookAt(ParabolaPhysics.GetParabolaNextPosition(position, velocity, gravity, deltaTime));

        // Simply simulate collision detection
        if (position.y <= dest.y) enabled = false;
    }

}