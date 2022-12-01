using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestElements : MonoBehaviour
{
    [SerializeField] private List<float> launchForce = new List<float>();

    private List<GameObject> _elements = new List<GameObject>();
    private List<Vector3> _defaultPositions = new List<Vector3>();
    private List<Quaternion> _defaultRotations = new List<Quaternion>();
    private int _currentElement;
    private int _currentLaunchForce;
    
    private const int DROP_HEIGHT = 7;


    private void Awake()
    {
        var numElements = transform.childCount;
        for (var elementIndex = 0; elementIndex < numElements; elementIndex++)
        {
            _elements.Add(transform.GetChild(elementIndex).gameObject);
        }

        var firstElement = transform.GetChild(0);
        var firstElementChildCount = firstElement.childCount;
        for (var elementObjectIndex = 0; elementObjectIndex < firstElementChildCount; elementObjectIndex++)
        {
            _defaultPositions.Add(firstElement.GetChild(elementObjectIndex).position);
            _defaultRotations.Add(firstElement.GetChild(elementObjectIndex).rotation);
        }

        foreach (var element in _elements)
        {
            element.SetActive(false);
        }
        
        Drop();
    }

    [Button]
    public void NextDrop()
    {
        _currentLaunchForce++;
        if (_currentLaunchForce >= launchForce.Count) _currentLaunchForce = 0;
        
        Drop();
    }

    private void Drop()
    {
        _elements[_currentElement].SetActive(true);

        var childCount = _elements[_currentElement].transform.childCount;
        for (var index = 0; index < childCount; index++)
        {
            var childTransform = _elements[_currentElement].transform.GetChild(index);
            childTransform.position = new Vector3(_defaultPositions[index].x, DROP_HEIGHT, _defaultPositions[index].z);            
            childTransform.rotation = _defaultRotations[index];

            var childRigid = childTransform.GetComponent<Rigidbody>();
            childRigid.velocity = Vector3.zero;
            childRigid.AddForce(launchForce[_currentLaunchForce] * Vector3.down, ForceMode.Impulse);
        }
    }
    
    [Button]
    public void NextElement()
    {
        _elements[_currentElement++].SetActive(false);
        
        if (_currentElement >= _elements.Count) _currentElement = 0;
        
        Drop();
    }
}
