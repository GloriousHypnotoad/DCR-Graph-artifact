using System;
using UnityEngine;

public class ActivtyExecuteButtonLightController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void toggleLightsEnabled(bool enabled)
    {
        applyToAllLights(light => light.enabled = enabled);
    }

    public void setLightColors(Color color)
    {
        applyToAllLights(light => light.color = color);
    }

    private void applyToAllLights(Action<Light> action)
    {
        Light[] lights = transform.Find("Lights").gameObject.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            action(light);
        }
    }
}