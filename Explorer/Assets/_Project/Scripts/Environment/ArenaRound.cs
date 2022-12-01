using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class ArenaRound : MonoBehaviour
{
    [SerializeField] private List<ArenaEvent> arenaEvents = new List<ArenaEvent>();
    
    private int _currentEvent;
    private int _totalEvents;
    private GameObject _thisGameObject;
    
    public Action RoundHasStarted;
    public Action RoundHasEnded;


    private void Awake()
    {
        if (DebugLogger.IsNullOrEmptyError(arenaEvents, this, "Must be set in editor.")) return;

        _totalEvents = arenaEvents.Count;
        _thisGameObject = gameObject;
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    [Button]
    public void PopulateParameters()
    {
        arenaEvents = new List<ArenaEvent>();
        var tempArenaEvents = gameObject.GetComponentsInChildren<ArenaEvent>();
        foreach (var tempArenaEvent in tempArenaEvents)
        {
            arenaEvents.Add(tempArenaEvent);    //TODO maybe populate each event
        }
    }
    
    private void SubscribeToEvents()
    {
        foreach (var arenaEvent in arenaEvents)
        {
            if (DebugLogger.IsNullError(arenaEvent, this)) return;
            
            arenaEvent.HasEnded += StartNextEvent;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        foreach (var arenaEvent in arenaEvents)
        {
            if (DebugLogger.IsNullError(arenaEvent, this)) return;

            arenaEvent.HasEnded -= StartNextEvent;
        }
    }

    private void StartNextEvent()
    {
        if (_currentEvent >= _totalEvents)
        {
            EndRound();
            return;
        }

        _currentEvent++;
    }
    
    private void EndRound()
    {
        UnsubscribeFromEvents();
        
        //RoundHasEnded?.Invoke();
        
        EndRoundLocal();
    }

    private void EndRoundLocal()
    {
        _thisGameObject.SetActive(false);
    }

    public void BeginRound()
    {
        EnableRoundLocal();
        StartNextEvent();
        
        RoundHasStarted?.Invoke();
    }

    private void EnableRoundLocal()
    {
        if (!_thisGameObject) _thisGameObject = gameObject;

        _thisGameObject.SetActive(true);
    }
}
