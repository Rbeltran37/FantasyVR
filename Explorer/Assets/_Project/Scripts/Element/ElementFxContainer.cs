using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class ElementFxContainer : PooledObject
{
    private Dictionary<ElementFxSO, ElementFx> _elementFxDataToElementFx = new Dictionary<ElementFxSO, ElementFx>();
    private Dictionary<int, ElementFx> _indexToElementFx = new Dictionary<int, ElementFx>();
    private Dictionary<ElementFx, int> _elementFxToIndex = new Dictionary<ElementFx, int>();
    private Dictionary<Element.Effectiveness, int> _effectivenessToIndex = new Dictionary<Element.Effectiveness, int>();
    private Dictionary<int, Element.Effectiveness> _indexToEffectiveness = new Dictionary<int, Element.Effectiveness>();

    private const int INEFFECTIVE_INDEX = 0;
    private const int NORMAL_INDEX = 1;
    private const int EFFECTIVE_INDEX = 2;


    protected override void Awake()
    {
        base.Awake();

        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        for (var index = 0; index < ThisTransform.childCount; index++)
        {
            var child = ThisTransform.GetChild(index);
            var elementGameObject = child.gameObject;
            var elementFx = elementGameObject.GetComponent<ElementFx>();
            var elementFxSo = elementFx.GetElementFxSo();

            _elementFxDataToElementFx.Add(elementFxSo, elementFx);
            _indexToElementFx.Add(index, elementFx);
            _elementFxToIndex.Add(elementFx, index);
        }
        
        _effectivenessToIndex.Add(Element.Effectiveness.Ineffective, INEFFECTIVE_INDEX);
        _effectivenessToIndex.Add(Element.Effectiveness.Normal, NORMAL_INDEX);
        _effectivenessToIndex.Add(Element.Effectiveness.Effective, EFFECTIVE_INDEX);
        _indexToEffectiveness.Add(INEFFECTIVE_INDEX, Element.Effectiveness.Ineffective);
        _indexToEffectiveness.Add(NORMAL_INDEX, Element.Effectiveness.Normal);
        _indexToEffectiveness.Add(EFFECTIVE_INDEX, Element.Effectiveness.Effective);
    }
    
    public bool MeetsMinVelocityThreshold(ElementProperty elementProperty, float velocity)
    {
        var elementFxData = elementProperty.elementFxSo;
        return velocity > elementFxData.minVelocityThreshold;
    }

    public float GetMinVelocity(ElementProperty elementProperty)
    {
        var elementFxData = elementProperty.elementFxSo;
        return elementFxData.minVelocityThreshold;
    }
    
    public void PlayFx(ElementProperty elementProperty, Element.Effectiveness effectiveness, float velocity, float size)
    {
        var elementFxData = elementProperty.elementFxSo;
        var elementFx = _elementFxDataToElementFx[elementFxData];
        elementFx.PlayFx(effectiveness, velocity, size);

        SendPlayFx(elementFx, effectiveness, velocity, size);
    }
    
    private void SendPlayFx(ElementFx elementFx, Element.Effectiveness effectiveness, float velocity, float size)
    {
        if (!ThisPhotonView) return;

        if (!ThisPhotonView.IsMine) return;

        var index = _elementFxToIndex[elementFx];
        var effectivenessIndex = _effectivenessToIndex[effectiveness];
        ThisPhotonView.RPC(nameof(RPCPlayFx), RpcTarget.OthersBuffered, index, effectivenessIndex, velocity, size);
    }

    [PunRPC]
    private void RPCPlayFx(int index, int effectivenessIndex, float velocity, float size)
    {
        var elementFx = _indexToElementFx[index];
        var effectiveness = _indexToEffectiveness[effectivenessIndex];
        elementFx.PlayFx(effectiveness, velocity, size);
    }
}
