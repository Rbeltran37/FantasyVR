using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChamberSand : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    [SerializeField] [MinMaxFloatRange(1, 10)] private RangedFloat interval;


    private int _count;

    
    private void Awake()
    {
        _count = particleSystems.Capacity;

        StartCoroutine(PlayParticles());
    }

    private IEnumerator PlayParticles()
    {
        while (true)
        {
            var randomInterval = Random.Range(interval.minValue, interval.maxValue);
            yield return new WaitForSeconds(randomInterval);

            var randomIndex = Random.Range(0, _count);
            particleSystems[randomIndex].Play(true);
        }
    }
}
