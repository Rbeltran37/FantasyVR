using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSizeAbility : SkillAbility
{
    [SerializeField] private Transform skillInstanceTransform;
    [SerializeField] private Axis axisToScale = Axis.All;

    private bool _isXScaled;
    private bool _isYScaled;
    private bool _isZScaled;
    private Vector3 _defaultScale;
    private float _defaultXScale;
    private float _defaultYScale;
    private float _defaultZScale;
    
    private enum Axis
    {
        All,
        X,
        Y,
        Z,
        NotX,
        NotY,
        NotZ,
    }


    protected override void Awake()
    {
        base.Awake();

        _isXScaled = axisToScale == Axis.All || axisToScale == Axis.X || axisToScale == Axis.NotY ||
                     axisToScale == Axis.NotZ;

        _isYScaled = axisToScale == Axis.All || axisToScale == Axis.Y || axisToScale == Axis.NotX ||
                     axisToScale == Axis.NotZ;

        _isZScaled = axisToScale == Axis.All || axisToScale == Axis.Z || axisToScale == Axis.NotX ||
                     axisToScale == Axis.NotY;

        _defaultScale = skillInstanceTransform.localScale;
        _defaultXScale = _defaultScale.x;
        _defaultYScale = _defaultScale.y;
        _defaultZScale = _defaultScale.z;
    }

    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        skillInstanceTransform.localScale = _defaultScale;
    }

    protected override void Activate()
    {
        if (Level == NOT_APPLIED) return;

        skillInstanceTransform.localScale = GetAppliedScale();
    }

    public Vector3 GetAppliedScale()
    {
        var x = _isXScaled ? Value * _defaultXScale : _defaultXScale;
        var y = _isYScaled ? Value * _defaultYScale : _defaultYScale;
        var z = _isZScaled ? Value * _defaultZScale : _defaultZScale;
        return new Vector3(x, y, z);
    }
}
