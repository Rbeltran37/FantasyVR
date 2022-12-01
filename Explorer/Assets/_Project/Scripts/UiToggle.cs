using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiToggle : MonoBehaviour
{
    [SerializeField] private GameObject uiCanvas;

    public void ToggleUICanvas()
    {
        if (uiCanvas.activeSelf)
        {
            uiCanvas.SetActive(false);
        }
        else
        {
            uiCanvas.SetActive(true);
        }
    }
}
