using System.Collections;
using UnityEngine;

public class CalibrationInitiation : MonoBehaviour
{
    public bool rightTriggerButton = false;
    public bool leftTriggerButton = false;
    [SerializeField] private HeightCalibration heightCalibration;
    
    public void leftTriggerActivate()
    {
        leftTriggerButton = true;
    }
    
    public void rightTriggerActivate()
    {
        rightTriggerButton = true;
    }
    
    public void leftTriggerDeactivate()
    {
        leftTriggerButton = false;
    }
    
    public void rightTriggerDeactivate()
    {
        rightTriggerButton = false;
    }

    public IEnumerator TriggerCheck()
    {
        while (!leftTriggerButton || !rightTriggerButton)
        {
            yield return null;
        }
        heightCalibration.SetUserHeight();
        yield return null;
    }
}
