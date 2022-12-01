using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorSlide : MonoBehaviour
{
    public Transform door;
    public Vector3 targetPos;
    public float totalTimeToOpen;
    public float travelDistance;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = new Vector3(door.transform.position.x, door.transform.position.y + travelDistance, door.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void OpenDoor()
    {
        StartCoroutine(StartOpenDoor(targetPos, totalTimeToOpen));
    }
    
    IEnumerator StartOpenDoor(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = door.transform.position;

        while (time < duration)
        {
            door.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        door.transform.position = targetPosition;
    }
}
