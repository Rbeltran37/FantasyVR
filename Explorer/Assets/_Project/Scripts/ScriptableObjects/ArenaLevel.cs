using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;


public class ArenaLevel : MonoBehaviour
{
    [SerializeField] private GameObject levelRoot;
    [SerializeField] private GameObject levelManagerRoot;
    

    public void StartLevel()
    {
        levelRoot.SetActive(true);
        levelManagerRoot.SetActive(true);
    }

    public void EndLevel()
    {
        levelRoot.SetActive(false);
        levelManagerRoot.SetActive(false);
    }
}
