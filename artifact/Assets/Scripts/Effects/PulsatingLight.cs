using UnityEngine;

public class PulsatingLight : MonoBehaviour
{
    
    private Light _lightComponent;
    private float _initialIntensity;
    private float _pulseIntensity;
    private float _pulseSpeed = 1.0f;
    private bool _isPulsating = false;
    
    void Start()
    {
        _lightComponent = GetComponent<Light>();
        _initialIntensity = _lightComponent.intensity;
        _pulseIntensity = _initialIntensity * 2;
    }

    void Update()
    {
        if (_isPulsating)
        {
            float sinValue = Mathf.Sin(Time.time * _pulseSpeed);
            float intensityModifier = (sinValue + 1) / 2 * 0.5f + 0.5f;
            _lightComponent.intensity = _initialIntensity * intensityModifier;
        }
    }

    public void TogglePulse(bool shouldPulsate)
    {
        _isPulsating = shouldPulsate;
        
        if (!shouldPulsate)
        {
            _lightComponent.intensity = _initialIntensity;
        }
    }
}
