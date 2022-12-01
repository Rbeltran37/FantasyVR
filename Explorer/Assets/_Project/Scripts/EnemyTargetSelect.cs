using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyTargetSelect : MonoBehaviour
{
    [SerializeField] private CustomUserControlAI customUserControlAi;
    
    private void Awake()
    {
        ReferenceManager.enemyAi = customUserControlAi;
        ReferenceManager.enemyAi.navTarget = ReferenceManager.PlayerOne;
    }
}
