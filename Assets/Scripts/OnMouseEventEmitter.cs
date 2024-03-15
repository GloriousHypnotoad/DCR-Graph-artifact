using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnMouseEventEmitter : MonoBehaviour
{
    private event Action _mouseDown;
    void OnMouseDown()
    {
        _mouseDown?.Invoke();
    }
    internal void SubscribeToOnMouseDown(Action subscriber)
    {
        _mouseDown += subscriber;
    }
}
