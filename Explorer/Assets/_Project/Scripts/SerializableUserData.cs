/*
 * This class is a serializable public class that's used for
 * saving and loading data. ONLY put primitive data types.
 * ENUM's need to be represented by integers below. 
 */

[System.Serializable]
public class SerializableUserData
{
    public float height;
    public int handedness;
    public int smoothTurning;
    public int snapTurning;
    public int snapTurnDegrees;
    public int comfort;

    private const int DEFAULT_HANDEDNESS = 1;
    private const int DEFAULT_SMOOTH_TURNING = 0;
    private const int DEFAULT_SNAP_TURNING = 2;
    private const int DEFAULT_SNAP_TURN_DEGREES = 0;
    private const int DEFAULT_COMFORT = 0;

    public SerializableUserData(UserDataReferenceHelper referenceHelper)
    {
        handedness = referenceHelper.handednessValue;
        smoothTurning = referenceHelper.smoothTurningValue;
        snapTurning = referenceHelper.snapTurningValue;
        snapTurnDegrees = referenceHelper.snapTurnDegreesValue;
        comfort = referenceHelper.comfortValue;
    }
    
    public SerializableUserData()
    {
        handedness = DEFAULT_HANDEDNESS;
        smoothTurning = DEFAULT_SMOOTH_TURNING;
        snapTurning = DEFAULT_SNAP_TURNING;
        snapTurnDegrees = DEFAULT_SNAP_TURN_DEGREES;
        comfort = DEFAULT_COMFORT;
    }
}
