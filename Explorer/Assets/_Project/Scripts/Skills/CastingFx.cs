using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CastingFx : PooledObject
{
    [SerializeField] private List<SkillSO> fireSkills = new List<SkillSO>();
    [SerializeField] private List<SkillSO> iceSkills = new List<SkillSO>();
    [SerializeField] private List<SkillSO> electricSkills = new List<SkillSO>();
    [SerializeField] private List<ParticleSystem> fireCast;
    [SerializeField] private List<ParticleSystem> iceCast;
    [SerializeField] private List<ParticleSystem> electricCast;
    [SerializeField] private List<ParticleSystem> fireCasting;
    [SerializeField] private List<ParticleSystem> iceCasting;
    [SerializeField] private List<ParticleSystem> electricCasting;

    private Dictionary<SkillSO, List<ParticleSystem>> _castFx = new Dictionary<SkillSO, List<ParticleSystem>>();
    private Dictionary<SkillSO, List<ParticleSystem>> _castingFx = new Dictionary<SkillSO, List<ParticleSystem>>();
    private Dictionary<int, List<SkillSO>> _indexToSkills = new Dictionary<int, List<SkillSO>>();
    private Dictionary<SkillSO, int> _skillToIndex = new Dictionary<SkillSO, int>();


    protected override void Awake()
    {
        base.Awake();

        InitializeCastDictionary();
        InitializeCastingDictionary();
        InitializeIndexDictionaries();
    }

    private void InitializeIndexDictionaries()
    {
        var index = 0;
        foreach (var skill in fireSkills)
        {
            _skillToIndex.Add(skill, index);
        }
        _indexToSkills.Add(index, fireSkills);

        index++;
        foreach (var skill in iceSkills)
        {
            _skillToIndex.Add(skill, index);
        }
        _indexToSkills.Add(index, iceSkills);

        index++;
        foreach (var skill in electricSkills)
        {
            _skillToIndex.Add(skill, index);
        }
        _indexToSkills.Add(index, electricSkills);
    }

    private void InitializeCastDictionary()
    {
        foreach (var skill in fireSkills)
        {
            _castFx.Add(skill, fireCast);
        }

        foreach (var skill in iceSkills)
        {
            _castFx.Add(skill, iceCast);
        }

        foreach (var skill in electricSkills)
        {
            _castFx.Add(skill, electricCast);
        }
    }
    
    private void InitializeCastingDictionary()
    {
        foreach (var skill in fireSkills)
        {
            _castingFx.Add(skill, fireCasting);
        }

        foreach (var skill in iceSkills)
        {
            _castingFx.Add(skill, iceCasting);
        }
        
        foreach (var skill in electricSkills)
        {
            _castingFx.Add(skill, electricCasting);
        }
    }

    public void Cast(SkillSO skill)
    {
        StopCasting(skill);

        var castParticles = _castFx[skill];
        foreach (var system in castParticles)
        {
            system.Play();
        }

        SendCast(skill);
    }

    private void SendCast(SkillSO skill)
    {
        if (!ThisPhotonView) return;

        if (!ThisPhotonView.IsMine) return;

        var index = _skillToIndex[skill];
        ThisPhotonView.RPC(nameof(RPCCast), RpcTarget.OthersBuffered, index);
    }

    [PunRPC]
    private void RPCCast(int index)
    {
        var skills = _indexToSkills[index];
        var skill = skills[0];
        Cast(skill);
    }

    public void StopCasting(SkillSO skill)
    {
        var castingParticles = _castingFx[skill];
        foreach (var system in castingParticles)
        {
            system.Stop();
        }
        
        SendStopCasting(skill);
        Despawn();
    }

    private void SendStopCasting(SkillSO skill)
    {
        if (!ThisPhotonView) return;

        if (!ThisPhotonView.IsMine) return;

        var index = _skillToIndex[skill];
        ThisPhotonView.RPC(nameof(RPCStopCasting), RpcTarget.OthersBuffered, index);
    }

    [PunRPC]
    private void RPCStopCasting(int index)
    {
        var skills = _indexToSkills[index];
        var skill = skills[0];
        StopCasting(skill);
    }

    public void Casting(SkillSO skill)
    {
        var castingParticles = _castingFx[skill];
        foreach (var system in castingParticles)
        {
            system.Play();
        }
        
        SendCasting(skill);
    }

    private void SendCasting(SkillSO skill)
    {
        if (!ThisPhotonView) return;

        if (!ThisPhotonView.IsMine) return;

        var index = _skillToIndex[skill];
        ThisPhotonView.RPC(nameof(RPCCasting), RpcTarget.OthersBuffered, index);
    }

    [PunRPC]
    private void RPCCasting(int index)
    {
        var skills = _indexToSkills[index];
        var skill = skills[0];
        Casting(skill);
    }
}
