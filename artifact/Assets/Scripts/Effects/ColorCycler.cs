using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ColorCycler : MonoBehaviour
{
    private HashSet<Color> _colors;
    private Material _material;
    private float _waitTime = 1f;
    private Coroutine _colorCycleCoroutine;
    private bool _isCycling = true;

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    public void StartCycle(HashSet<Color> colors)
    {
        _colors = colors;
        _isCycling = true;
        _colorCycleCoroutine = StartCoroutine(CycleColors());
    }

    IEnumerator CycleColors()
    {
        while (_isCycling)
        {
            foreach (var color in _colors)
            {
                _material.color = color;
                
                yield return new WaitForSeconds(_waitTime);
            }
        }
    }

    public bool IsRunning()
    {
        return _isCycling;
    }

    public void StopCycle()
    {
        _isCycling = false;
        if (_colorCycleCoroutine != null)
        {
            StopCoroutine(_colorCycleCoroutine);
        }
    }
}