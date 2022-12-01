using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Transform barTransform;
    [SerializeField] private Image barImage;
    [SerializeField] private bool followCamera = true;

    private Color _color;
    private Coroutine _coroutine;
    private float _targetSize;

    private const float WAIT_TIME = .02f;
    private const float INCREMENT_AMOUNT = .01f;
    private const float DEFAULT_BAR_SCALE = 1f;
    private const float NULL_VALUE = 0f;
    private const float RED_PERCENTAGE = .25f;
    private const float YELLOW_PERCENTAGE = .5f;


    // Start is called before the first frame update
    void Awake()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;

        if (DebugLogger.IsNullWarning(thisTransform, this, "Should be set in editor."))
        {
            thisTransform = transform;
        }

        if (DebugLogger.IsNullWarning(barTransform, this, "Should be set in editor. Attempting to find."))
        {
            barTransform = transform.Find("Bar");
            if (DebugLogger.IsNullError(barTransform, this, "Should be set in editor. Was not found.")) return;
        }

        if (DebugLogger.IsNullWarning(barImage, this, "Should be set in editor. Attempting to find."))
        {
            barImage = barTransform.GetComponentInChildren<Image>();
            if (DebugLogger.IsNullError(barImage, this, "Should be set in editor. Was not found.")) return;
        }
        
        _color = barImage.color;
        
        health.WasHit += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (health) health.WasHit -= UpdateHealthBar;
    }

    private void LateUpdate()
    {
        if (!followCamera) return;

        var camera = Camera.main;
        if (!camera) return;
        thisTransform.LookAt(camera.transform);
        thisTransform.localEulerAngles = new Vector3(0, thisTransform.localEulerAngles.y, 0);
    }

    private void SetSize(float sizeNormalized) {

        if (sizeNormalized < 0)
            sizeNormalized = 0;
        
        var isLower = barTransform.localScale.x > sizeNormalized;

        _targetSize = sizeNormalized;
        

        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        
        _coroutine = CoroutineCaller.Instance.StartCoroutine(SmoothBarTransition(isLower));
    }

    private void SetColor(float size) {

        if (size <= RED_PERCENTAGE)
            _color = Color.red;
        else if (size <= YELLOW_PERCENTAGE)
            _color = Color.yellow;
        else
            _color = Color.green;

        barImage.color = _color;
    }

    private IEnumerator SmoothBarTransition(bool isLower) {

        var currentScale = barTransform.localScale.x;

        if (isLower) {

            while (currentScale > _targetSize) {

                barTransform.localScale = new Vector3(currentScale - INCREMENT_AMOUNT, DEFAULT_BAR_SCALE, DEFAULT_BAR_SCALE);

                currentScale = barTransform.localScale.x;
                if (currentScale < 0)
                    barTransform.localScale = new Vector3();

                SetColor(currentScale);

                yield return new WaitForSeconds(WAIT_TIME);
            }
        }
        else {

            while (currentScale < _targetSize) {

                barTransform.localScale = new Vector3(currentScale + INCREMENT_AMOUNT, DEFAULT_BAR_SCALE, DEFAULT_BAR_SCALE);

                currentScale = barTransform.localScale.x;
                if (currentScale > 1)
                    barTransform.localScale = new Vector3(1,1,1);

                SetColor(currentScale);

                yield return new WaitForSeconds(WAIT_TIME);
            }
        }
    }
    
    private void UpdateHealthBar(float damage)
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        
        SetSize(health.GetHealthPercentage());
    }

    public void ResetObject()
    {
        if (DebugLogger.IsNullError(barTransform, this, "Must be set in editor.")) return;
        
        barTransform.localScale = Vector3.one;
        SetColor(DEFAULT_BAR_SCALE);
        UpdateHealthBar(NULL_VALUE);
        
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;

        health.WasHit += UpdateHealthBar;
    }
}
