using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidHealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private LiquidBar liquidBar;

    [SerializeField] private bool followCamera = true;
    
    private Color _color;

    private const float WAIT_TIME = .02f;
    private const float INCREMENT_AMOUNT = .01f;

    
    void Start()
    {
        _color = liquidBar.barColor;

        if (health)
        {
            health.WasHit += UpdateHealthBar;
            health.WasAdded += UpdateHealthBar;
        }
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.WasHit -= UpdateHealthBar;
            health.WasAdded -= UpdateHealthBar;
        }
    }

    private void LateUpdate()
    {
        if (!followCamera) return;
        
        transform.LookAt(Camera.main.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    private void SetSize(float sizeNormalized) {
        
        if (!liquidBar) return;

        if (sizeNormalized < 0)
            sizeNormalized = 0;
        
        bool isLower = liquidBar.currentFillAmount > sizeNormalized;

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

                SetColor(currentScale);

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

                SetColor(currentScale);

                yield return new WaitForSeconds(WAIT_TIME);
            }
        }

        yield return null;
    }
    
    private void UpdateHealthBar(float damage)
    {
        if (!health) return;

        SetSize(health.GetHealthPercentage());
    }
}
