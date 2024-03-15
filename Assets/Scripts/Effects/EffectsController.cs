using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    private GameObject _fog;
    private GameObject _doorAndWalls;
    private GameObject _glitter;
    private GameObject _glitterKey;
    private GameObject _glitterLock;
    private GameObject _sceneryLight;
    private GameObject _glitterBurst;
    private GameObject _godray;
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _emissionModule;
    private float _initialEmissionRate;

    void Awake()
    {
        _fog = transform.Find(FileStrings.Fog).gameObject;
        _doorAndWalls = transform.Find(FileStrings.DoorAndWalls).gameObject;
        _glitter = transform.Find(FileStrings.Glitter).gameObject;
        _glitterKey = transform.Find(FileStrings.GlitterKey).gameObject;
        _glitterLock = transform.Find(FileStrings.GlitterLock).gameObject;
        _sceneryLight = transform.Find(FileStrings.SceneryLight).gameObject;
        _glitterBurst = transform.Find(FileStrings.GlitterBurst).gameObject;
        _godray = transform.Find(FileStrings.GodRay).gameObject;
        _particleSystem =_glitter.GetComponent<ParticleSystem>();
        _emissionModule = _particleSystem.emission;
        _initialEmissionRate = _emissionModule.rateOverTime.constant;
    }

    public void ToggleFog(bool isActive)
    {
        _fog.SetActive(isActive);
    }

    internal void ToggleDoorAndWalls(bool isActive)
    {
        _doorAndWalls.SetActive(isActive);
    }

    public void ToggleGlitter(bool isActive)
    {
        _glitter.SetActive(isActive);
    }

    internal void ToggleGlitterKey(bool isActive)
    {
        _glitterKey.SetActive(isActive);
    }

    internal void ToggleGlitterLock(bool isActive)
    {
        _glitterLock.SetActive(isActive);
    }

    public void ChangeGlitterColor(Color color)
    {
        ParticleSystem ps = _glitter.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startColor = color;
    }

    internal void ChangeGlitterKeyColor(Color color)
    {
        ParticleSystem ps = _glitterKey.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startColor = color;
    }

    internal void ChangeGlitterLockColor(Color color)
    {
        ParticleSystem ps = _glitterLock.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startColor = color;
    }

    internal void ChangeSceneryLightColor(Color color)
    {
        _sceneryLight.GetComponent<Light>().color = color;
    }

    public void ToggleSceneryLight(bool isPending)
    {
        _sceneryLight.SetActive(isPending);
    }

    public void GlitterBurst(float duration)
    {
        _glitterBurst.GetComponent<ParticleSystem>().Play();
    }

    internal void TogglePulseOnSceneryLight(bool isPulsating)
    {
        _sceneryLight.GetComponent<PulsatingLight>().TogglePulse(isPulsating);
    }

    internal void ToggleGodray(bool isActive)
    {
        GodRay godRay =_godray.GetComponent<GodRay>();
        if (godRay.GetIsActive() != isActive)
        {
            godRay.toggleActive(isActive);
        }
    }

    internal void ChangeGodrayColor(Color color)
    {
        GameObject godRay = transform.Find("GodRay/GodrayColumn").gameObject;
        Material godRayMaterial = godRay.GetComponent<Renderer>().material;
        godRayMaterial.color = color;
        godRayMaterial.EnableKeyword("_EMISSION");
        godRayMaterial.SetColor("_EmissionColor", color);
    }

    internal void SetGlitterRate(float v)
    {
        _emissionModule.rateOverTime = v;
    }

    internal void ResetGlitterRate()
    {
        _emissionModule.rateOverTime = _initialEmissionRate;
    }

    internal void StartGlitterColorCycle(HashSet<Color> colors)
    {
        _glitter.GetComponent<ParticleColorCycler>().StartCycle(colors);
    }

    internal void StopGlitterColorCycle()
    {
        _glitter.GetComponent<ParticleColorCycler>().StopCycle();
    }

    internal void StartGlitterLockColorCycle(HashSet<Color> colors)
    {
        _glitterLock.GetComponent<ParticleColorCycler>().StartCycle(colors);
    }

    internal void StopGlitterLockColorCycle()
    {
         _glitterLock.GetComponent<ParticleColorCycler>().StopCycle();
    }
}
