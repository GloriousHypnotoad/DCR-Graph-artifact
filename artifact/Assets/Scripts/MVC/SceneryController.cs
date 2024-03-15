    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryController : MonoBehaviour
{
    GameObject _sceneryOpaque;
    GameObject _sceneryTransparent;

    private List<IAnimatable> _animatableElementsOpaque;
    private List<IAnimatable> _animatableElementsTransparent;

    void Awake()
    {
        _sceneryOpaque = transform.Find(FileStrings.SceneryOpaque).gameObject;
        _sceneryTransparent = transform.Find(FileStrings.SceneryTransparent).gameObject;

        _animatableElementsOpaque = new List<IAnimatable>(transform.Find("SceneryOpaque/AnimatedElementsContainer").GetComponentsInChildren<IAnimatable>());
        _animatableElementsTransparent = new List<IAnimatable>(transform.Find("SceneryTransparent/AnimatedElementsContainer").GetComponentsInChildren<IAnimatable>());
    }

    public void ToggleAnimatedElements(bool isAnimated)
    {
        if (_sceneryOpaque != null && _sceneryOpaque.activeInHierarchy)
        {
            foreach (IAnimatable animatableElement in _animatableElementsOpaque)
            {
                animatableElement.ToggleAnimation(isAnimated);
            }
        }

        if (_sceneryTransparent != null && _sceneryTransparent.activeInHierarchy)
        {
            foreach (IAnimatable animatableElement in _animatableElementsTransparent)
            {
                animatableElement.ToggleAnimation(isAnimated);
            }
        }
    }
    public void SetOpaque(bool opaque)
    {
        _sceneryOpaque.SetActive(opaque);
        _sceneryTransparent.SetActive(!opaque);
    }
}