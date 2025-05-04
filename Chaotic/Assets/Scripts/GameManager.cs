using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Light basementLight;
    [SerializeField] private ItemChecker itemChecker;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RenderSettings.fog = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (itemChecker.HasSucceeded)
        {
            RenderSettings.fog = true;
            basementLight.enabled = false;
        }
    }
}
