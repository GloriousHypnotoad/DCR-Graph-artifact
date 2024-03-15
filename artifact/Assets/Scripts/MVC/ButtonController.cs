using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonController : MonoBehaviour
{
    public bool ButtonEnabled { get; private set; }
    private ObjectRotation _objectRotation;
    private ObjectBobbing _objectBobbing;
    public event Action<float> _onPressed;
    private GameObject _buttonOpaque;
    private GameObject _buttonTransparent;
    private GameObject _buttonOpaquePushButton;
    private GameObject _buttonTransparentPushButton;
    private ObjectShake _objectShake;

    void Awake()
    {
        _objectBobbing = GetComponentInChildren<ObjectBobbing>();
        _objectRotation = GetComponent<ObjectRotation>();
        _objectShake = GetComponent<ObjectShake>();
        _buttonOpaque = transform.Find(FileStrings.ButtonOpaque).gameObject;
        _buttonTransparent = transform.Find(FileStrings.ButtonTransparent).gameObject;
        _buttonOpaquePushButton = transform.Find(FileStrings.ButtonOpaquePushButton).gameObject;
        _buttonTransparentPushButton = transform.Find(FileStrings.ButtonTransparentPushButton).gameObject;
    }
    void Start()
    {
    }
    public void TogglePushButtonAnimation(bool playerIsNearby)
    {
        _objectBobbing.ToggleAnimation(playerIsNearby);
    }

    public void ToggleRotation(bool isRotating)
    {
        _objectRotation.ToggleAnimation(isRotating);       
    }
    public void PressButton()
    {
        PerformQuickRotation();
        _onPressed?.Invoke(_objectRotation.getQuickRotationDuration());
    }

    public void PressButtonRefuse()
    {
        PerformShake();
    }

    public void PerformQuickRotation()
    {
        _objectRotation.PerformQuickRotation();
    }

    public void PerformShake()
    {
        _objectShake.StartShake();
    }

    public void SubscribeToOnPressed(Action<float> subscriber)
    {
        _onPressed += subscriber;
    }

    public void SetOpaque(bool opaque)
    {
        _buttonOpaque.SetActive(opaque);
        _buttonTransparent.SetActive(!opaque);
    }

    internal void StartPushButtonColorCycle(HashSet<Color> colors)
    {
        _buttonOpaquePushButton.GetComponent<ColorCycler>().StartCycle(colors);

        if (colors.First() != Color.white)
        {
            _buttonTransparentPushButton.GetComponent<ColorCycler>().StartCycle(colors);
        }
        
    }

    internal void SetPushButtonColor(Color color)
    {
        if (color != Color.white)
        {
            Material materialTransparent = _buttonTransparentPushButton.GetComponent<Renderer>().material;
            materialTransparent.color = color;
            materialTransparent.EnableKeyword("_EMISSION");
            materialTransparent.SetColor("_EmissionColor", color);
        }

        Material materialOpaque = _buttonOpaquePushButton.GetComponent<Renderer>().material;
        materialOpaque.color = color;
        materialOpaque.EnableKeyword("_EMISSION");
        materialOpaque.SetColor("_EmissionColor", color);
    }

    internal void StopPushButtonColorCycle()
    {
        if(_buttonOpaquePushButton.GetComponent<ColorCycler>().IsRunning())
        {
            _buttonOpaquePushButton.GetComponent<ColorCycler>().StopCycle();
        }

        if(_buttonTransparentPushButton.GetComponent<ColorCycler>().IsRunning())
        {
            _buttonTransparentPushButton.GetComponent<ColorCycler>().StopCycle();
        }
    }
}
