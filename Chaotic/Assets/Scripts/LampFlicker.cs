using System.Collections;
using UnityEngine;

public class PointLampFlicker : MonoBehaviour
{
    private Light pointLight;

    [Header("Flicker Settings")]
    public float flickerChance = 0.3f;
    public float minFlickerInterval = 3f;
    public float maxFlickerInterval = 6f;
    public float flickerSpeed = 0.06f;

    void Start()
    {
        pointLight = GetComponent<Light>();
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(wait);

            if (Random.value <= flickerChance)
            {
                int flickTimes = Random.Range(1, 4);
                for (int i = 0; i < flickTimes; i++)
                {
                    pointLight.enabled = false;
                    yield return new WaitForSeconds(flickerSpeed);
                    pointLight.enabled = true;
                    yield return new WaitForSeconds(flickerSpeed);
                }
            }
        }
    }
}
