using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutPlayerSetupHelper : MonoBehaviour
{
    [SerializeField] private LoadoutSelection loadoutSelection;
    [SerializeField] private HolsterSetupHelper holsterSetupHelper;
    private const string LOBBY_SCENE = "Networked Loadout";
    
    
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == LOBBY_SCENE)
        {
            loadoutSelection = GameObject.Find("[LoadoutSelection]").GetComponent<LoadoutSelection>();
        }
    }

    private void Start()
    {
        if (loadoutSelection)
        {
            loadoutSelection.SetHolsterSetupHelper(holsterSetupHelper);
        }
        else
        {
            Debug.Log("Unable to get Loadout selection");
        }
    }
}
