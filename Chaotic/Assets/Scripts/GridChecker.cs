using UnityEngine;
using System.Collections.Generic;

public class GridChecker : MonoBehaviour
{
    [SerializeField] private List<ItemChecker> itemCheckers;
    public bool AllItemsChecked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ItemChecker itemChecker in itemCheckers)
        {
            if (itemChecker.HasSucceeded)
            {
                AllItemsChecked = true;
            }
        }
    }
}
