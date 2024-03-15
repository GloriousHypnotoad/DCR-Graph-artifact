using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintsController : MonoBehaviour
{
    GameObject _lock;
    GameObject _key;
    OnMouseEventEmitter _lockOnMouseEventEmitter;
    private event Action lockMouseDown;


void Awake()
{
    _lock = transform.Find(FileStrings.Lock).gameObject;
    _key = transform.Find(FileStrings.Key).gameObject;
    _lockOnMouseEventEmitter = _lock.GetComponent<OnMouseEventEmitter>();

    _lockOnMouseEventEmitter.SubscribeToOnMouseDown(OnLockMouseDown);
}

    private void OnLockMouseDown()
    {
        lockMouseDown?.Invoke();
    }
    public void SubscribeToOnLockMouseDown(Action subscriber)
    {
        lockMouseDown += subscriber;
    }

    internal void StartLockColorCycle(HashSet<Color> colors)
    {
        _lock.GetComponent<ColorCycler>().StartCycle(colors);        
    }

    internal void StopLockColorCycle()
    {
        if(_lock.GetComponent<ColorCycler>().IsRunning())
        {
            _lock.GetComponent<ColorCycler>().StopCycle();
        }
    }

    internal void SetLockColor(Color color)
    {
        if (color != Color.white)
        {
            Material lockMaterial = _lock.GetComponent<Renderer>().material;
            lockMaterial.color = color;
        }
    }

    internal void ToggleLock(bool isActive)
    {
        _lock.SetActive(isActive);
    }

    internal void ToggleKey(bool isActive)
    {
        _key.SetActive(isActive);
    }

    internal void SetKeyColor(Color color)
    {
        Material keyMaterial = _key.GetComponent<Renderer>().material;
        keyMaterial.color = color;
    }
}