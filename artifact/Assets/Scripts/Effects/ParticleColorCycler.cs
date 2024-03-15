using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorCycler : MonoBehaviour
{
    private HashSet<Color> _colors;
    private ParticleSystem _particleSystem;
    private float _waitTime = 1f;
    private Coroutine _colorCycleCoroutine;
    private bool _isCycling = true;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
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
                var mainModule = _particleSystem.main;
                mainModule.startColor = color;

                yield return new WaitForSeconds(_waitTime);
            }
        }
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
