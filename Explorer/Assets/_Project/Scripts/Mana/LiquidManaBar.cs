using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidManaBar : MonoBehaviour
{
    [SerializeField] private ManaPool manaPool;
    [SerializeField] private LiquidBar liquidBar;

    [SerializeField] private bool followCamera = true;
    
    private Color _color;        //TODO May be obsolete

    private const float WAIT_TIME = .02f;
    private const float INCREMENT_AMOUNT = .01f;
    

    void Start()
    {
        if (DebugLogger.IsNullError(liquidBar, this, "Must be set in editor.")) return;

        _color = liquidBar.barColor;
    }

    private void Update()
    {
        UpdateManaBar();
    }

    private void LateUpdate()
    {
        if (!followCamera) return;
        
        transform.LookAt(Camera.main.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    private void SetSize(float sizeNormalized) 
    {
        if (!liquidBar) return;

        if (sizeNormalized < 0)
            sizeNormalized = 0;
        
        var isLower = liquidBar.currentFillAmount > sizeNormalized;

        StartCoroutine(SmoothBarTransition(sizeNormalized, isLower));
    }

    private void SetColor(float size) {
        
        if (!liquidBar) return;

        _color = liquidBar.barColor;
        
        if (size <= .25f)
            _color = Color.red;
        else if (size <= .5f)
            _color = Color.yellow;
        else
            _color = Color.green;

        liquidBar.barColor = _color;
        liquidBar.material.SetColor("_Color", _color);
        liquidBar.material.SetFloat("_Color", .5f);
    }

    private IEnumerator SmoothBarTransition(float targetSize, bool isLower)
    {

        if (!liquidBar) yield break;

        var currentScale = liquidBar.targetFillAmount;
        var adjustedScale = currentScale;
        if (isLower) {

            while (currentScale > targetSize) {
                
                adjustedScale -= INCREMENT_AMOUNT;
                if (adjustedScale <= 0) adjustedScale = 0;
                
                liquidBar.targetFillAmount = adjustedScale;

                currentScale = liquidBar.targetFillAmount;
                if (currentScale < 0)
                    liquidBar.targetFillAmount = 0;

                //SetColor(currentScale);

                yield return new WaitForSeconds(WAIT_TIME);
            }
        }
        else {

            while (currentScale < targetSize) {

                adjustedScale += INCREMENT_AMOUNT;
                if (adjustedScale > 1) adjustedScale = 1;
                
                liquidBar.targetFillAmount = adjustedScale;

                currentScale = liquidBar.targetFillAmount;
                if (currentScale > 1)
                    liquidBar.targetFillAmount = 1;

                //SetColor(currentScale);

                yield return new WaitForSeconds(WAIT_TIME);
            }
        }

        yield return null;
    }
    
    private void UpdateManaBar()
    {
        if (DebugLogger.IsNullError(manaPool, this, "Must be set in editor."))
        {
            enabled = false;
            return;
        }

        SetSize(manaPool.GetManaPercentage());
    }
}
