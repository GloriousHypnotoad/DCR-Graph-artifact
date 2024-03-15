using System;
using UnityEngine;

public class ActivityDetectionTrigger : MonoBehaviour
{
    private event Action _mouseOver;
    private event Action _mouseExit;
    private event Action _mouseDown;
    private event Action _simulatedMouseOver;
    private event Action _simulatedMouseExit;
    private event Action _simulatedMouseDown;

    // Triggers
    void OnMouseOver()
    {
        if (GameSettings.ActiveCamera != CameraMode.BirdsEye)
        {
            _mouseOver?.Invoke();
        }
        else
        {
            _simulatedMouseOver.Invoke();
        }
    }
    void OnMouseExit()
    {
        if (GameSettings.ActiveCamera != CameraMode.BirdsEye)
        {
            _mouseExit?.Invoke();
        }
        else
        {
            _simulatedMouseExit.Invoke();
        }
    }
    void OnMouseDown()
    {
        if (GameSettings.ActiveCamera != CameraMode.BirdsEye)
        {
            _mouseDown?.Invoke();
        }
        else
        {
            _simulatedMouseDown.Invoke();
        }
    }

    // Subscribe to Triggers
    public void SubscribeToOnMouseOver(Action subject)
    {
        _mouseOver += subject;
    }
    public void SubscribeToOnMouseExit(Action subject)
    {
        _mouseExit += subject;
    }
    public void SubscribeToOnMouseDown(Action subject)
    {    
        _mouseDown += subject;
    }
    public void SubscribeToOnSimulatedMouseOver(Action subject)
    {
        _simulatedMouseOver += subject;
    }
    public void SubscribeToOnSimulatedMouseExit(Action subject)
    {
        _simulatedMouseExit += subject;
    }
    public void SubscribeToOnSimulatedMouseDown(Action subject)
    {    
        _simulatedMouseDown += subject;
    }
}