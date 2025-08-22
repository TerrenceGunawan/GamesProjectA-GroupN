using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLightFlicker : MonoBehaviour
{
    public List<Light> lights = new List<Light>();

    public float flickerChance = 0.3f;
    public float minFlickerInterval = 3f;
    public float maxFlickerInterval = 6f;
    public float flickerSpeed = 0.06f;

    void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(wait);

            if (Random.value <= flickerChance)
            {
                int flickTimes = Random.Range(1, 7);
                for (int i = 0; i < flickTimes; i++)
                {
                    SetLights(false);
                    yield return new WaitForSeconds(flickerSpeed);
                    SetLights(true);
                    yield return new WaitForSeconds(flickerSpeed);
                }
            }
        }
    }

    void SetLights(bool state)
    {
        foreach (var light in lights)
        {
            if (light != null)
                light.enabled = state;
        }
    }
}
