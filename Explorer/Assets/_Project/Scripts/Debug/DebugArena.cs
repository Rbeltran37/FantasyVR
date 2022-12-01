using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DebugArena : MonoBehaviour
{
    [SerializeField] private Text roundText;
    [SerializeField] private Text arenaEventText;
    [SerializeField] private Text enemiesKilledText;
    [SerializeField] private Text timeText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<ArenaRound> arenaRounds = new List<ArenaRound>();
    [SerializeField] private List<ArenaEvent> arenaEvents =  new List<ArenaEvent>();
    [SerializeField] private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();
    
    private int _currentRound;
    private int _numEnemiesKilled;
    private float _currentTime;

    private const string ROUND = "Round: ";
    private const string ARENA_EVENT = "Event: ";
    private const string ENEMIES_KILLED = "Enemies Killed: ";
    private const string TIME = "Time: ";
    
    
    private void Awake()
    {
        foreach (var round in arenaRounds)
        {
            if (!round)
            {
                DebugLogger.Error("Awake", "round in arenaRounds is null.", this);
                return;
            }

            if (roundText)
            {
                round.RoundHasStarted += () => { roundText.text = $"{ROUND}{++_currentRound}"; };
            }
            else
            {
                DebugLogger.Debug("Awake", $"roundText is null. Must be assigned in editor.", this);
            }
        }

        foreach (var arenaEvent in arenaEvents)
        {
            if (!arenaEvent)
            {
                DebugLogger.Error("Awake", $"arenaEvent in arenaEvents is null.", this);
                return;
            }

            if (audioSource)
            {
                arenaEvent.HasEnded += () => { audioSource.Play(); };
            }
            else
            {
                DebugLogger.Debug("Awake", "audioSource is null. Must be assigned in editor.", this);
            }

            if (arenaEventText)
            {
                arenaEvent.HasStarted += () => { arenaEventText.text = $"{ARENA_EVENT}{arenaEvent.GetType().Name}"; };
            }
            else
            {
                DebugLogger.Debug("Awake", "arenaEventText is null. Must be assigned in editor.", this);
            }
        }

        foreach (var enemySpawner in enemySpawners)
        {
            if (!enemySpawner)
            {
                DebugLogger.Error("Awake", "enemySpawner in enemySpawners is null.", this);
                return;
            }
            
            if (enemiesKilledText)
            {
                enemySpawner.EnemyHasBeenKilled += () =>
                {
                    enemiesKilledText.text = $"{ENEMIES_KILLED}{++_numEnemiesKilled}";
                };
            }
            else
            {
                DebugLogger.Debug("Awake", "enemiesKilledText is null.", this);
            }
        }
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        if (timeText) timeText.text = $"{TIME}{time.ToString("hh':'mm':'ss")}";
    }

    [Button]
    private void PopulateParameters(GameObject arenaParent)
    {
        if (!arenaParent)
        {
            var arenaLevel = FindObjectOfType<ArenaLevel>();
            if (!arenaLevel)
            {
                DebugLogger.Debug("PopulateParameters", "Unable to ArenaLevel in scene.", this);
                return;
            }
            
            arenaParent = arenaLevel.gameObject;
        }
        
        arenaRounds = new List<ArenaRound>();
        var rounds = arenaParent.GetComponentsInChildren<ArenaRound>();
        foreach (var round in rounds)
        {
            arenaRounds.Add(round);
        }
        
        arenaEvents = new List<ArenaEvent>();
        var tempArenaEvents = arenaParent.GetComponentsInChildren<ArenaEvent>();
        foreach (var tempArenaEvent in tempArenaEvents)
        {
            arenaEvents.Add(tempArenaEvent);
        }

        enemySpawners = new List<EnemySpawner>();
        var tempSpawners = arenaParent.GetComponentsInChildren<EnemySpawner>();
        foreach (var spawner in tempSpawners)
        {
            enemySpawners.Add(spawner);
        }
    }
}
