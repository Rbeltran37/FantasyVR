using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public float pressingDistance;
    public bool pressed;
    public Transform buttonBackPanel;
    public ButtonEvent buttonPressed;

    Vector3 startPos;
    Rigidbody rb;
    SpringJoint springJoint;
    float buttonDistance = 0;
    float startDistance = 0;
    

    void Start()
    {
        startDistance = ButtonDistance();
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        springJoint = GetComponent<SpringJoint>();
    }

    void Update()
    {
        if (ButtonDistance() <= pressingDistance)
        {
            BackStopForButton();
            
            if (!pressed)
            {
                pressed = true;
                buttonPressed?.Invoke();
            }
        } else
        {
            pressed = false;
        }
        
        if (ButtonDistance() > startDistance)
        {
            transform.position = startPos;
        }
    }

    private float ButtonDistance()
    {
        buttonDistance = Vector3.Distance(transform.position, buttonBackPanel.position);
        return buttonDistance;
    }

    private void BackStopForButton()
    {
        springJoint.spring = 0;
        rb.isKinematic = true;
    }

    [Button]
    public void ResetButton()
    {
        transform.position = startPos;
        springJoint.spring = 10;
        rb.isKinematic = false;
    }
}