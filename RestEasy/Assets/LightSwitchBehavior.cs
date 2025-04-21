using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchBehavior : MonoBehaviour
{
    public Light[] controlledLights;

    bool lightsOn = false;

    void OnMouseDown()
    {
        if (lightsOn)
        {
            foreach (Light light in controlledLights)
            {
                light.enabled = false;
            }

            lightsOn = false;
        }
        else
        {
            foreach (Light light in controlledLights)
            {
                light.enabled = true;
            }

            lightsOn = true;
        }
    }
}
