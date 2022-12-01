using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ExplosionContainer : PooledSkillAbility
{
    private Dictionary<ElementFxSO, Explosion> _elementFxSoToExplosion = new Dictionary<ElementFxSO, Explosion>();
    private Dictionary<ElementFxSO, int> _elementFxSoToIndex = new Dictionary<ElementFxSO, int>();
    private Dictionary<int, ElementFxSO> _indexToElementFxSo = new Dictionary<int, ElementFxSO>();

    private GameObject _currentExplosionGameObject;
    private Explosion _currentExplosion;


    protected override void Awake()
    {
        for (var index = 0; index < ThisTransform.childCount; index++)
        {
            var child = ThisTransform.GetChild(index);
            var explosionGameObject = child.gameObject;
            var explosion = explosionGameObject.GetComponent<Explosion>();
            var elementFxSo = explosion.GetElementFxSo();

            _elementFxSoToExplosion.Add(elementFxSo, explosion);
            _elementFxSoToIndex.Add(elementFxSo, index);
            _indexToElementFxSo.Add(index, elementFxSo);
            explosionGameObject.SetActive(false);
        }
        
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (!ElementFxSo) return;
        
        SetElementFxSO(ElementFxSo);

        Explode();
    }

    private void Explode()
    {
        if (!_currentExplosionGameObject) return;
        
        _currentExplosionGameObject.SetActive(true);
        
        SendExplode();
    }

    private void SendExplode()
    {
        if (!ThisPhotonView) return;

        if (!ThisPhotonView.IsMine) return;

        var index = _elementFxSoToIndex[ElementFxSo];
        ThisPhotonView.RPC(nameof(RPCExplode), RpcTarget.OthersBuffered, index, Value);
    }

    [PunRPC]
    private void RPCExplode(int index, float value)
    {
        var elementFxSo = _indexToElementFxSo[index];
        SetElementFxSO(elementFxSo);
        SetValue(value);
        Explode();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        if (_currentExplosionGameObject) _currentExplosionGameObject.SetActive(false);
    }

    public override void SetElementFxSO(ElementFxSO elementFxSo)
    {
        base.SetElementFxSO(elementFxSo);
        
        var currentExplosion = _elementFxSoToExplosion[ElementFxSo];
        _currentExplosion = currentExplosion;
        _currentExplosionGameObject = _currentExplosion.GetGameObject();
    }

    public override void SetValue(float value)
    {
        if (value <= NOT_APPLIED) return;
        
        Value = value;
        
        _currentExplosion.SetValue(value);
        SetScale();
    }

    private void SetScale()
    {
        ThisTransform.localScale = new Vector3(Value, Value, Value);
    }
}
