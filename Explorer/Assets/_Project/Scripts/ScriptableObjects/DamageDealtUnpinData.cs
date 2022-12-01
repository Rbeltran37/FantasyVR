using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combat/DamageDealtUnpinData", order = 1)]
public class DamageDealtUnpinData : ScriptableObject
{
    public Force force = Force.One;
    public Unpin unpin = Unpin.One;
    
    private const int ZERO = 0;
    private const int FORCE_ONE = 2000;
    private const int FORCE_TWO = 4000;
    private const int FORCE_THREE = 6000;
    private const int FORCE_FOUR = 8000;
    private const int FORCE_FIVE = 10000;
    private const int UNPIN_ONE = 50;
    private const int UNPIN_TWO = 100;
    private const int UNPIN_THREE = 150;
    private const int UNPIN_FOUR = 200;
    private const int UNPIN_FIVE = 250;
    
    public enum Force
    {
        Zero = ZERO,
        One = FORCE_ONE,
        Two = FORCE_TWO,
        Three = FORCE_THREE,
        Four = FORCE_FOUR,
        Five = FORCE_FIVE,
    }
    
    public enum Unpin
    {
        Zero = ZERO,
        One = UNPIN_ONE,
        Two = UNPIN_TWO,
        Three = UNPIN_THREE,
        Four = UNPIN_FOUR,
        Five = UNPIN_FIVE,
    }

    public float GetForce()
    {
        return (float) force;
    }

    public float GetUnpin()
    {
        return (float) unpin;
    }
}
