using UnityEngine;
using System.Collections.Generic;

public class GridChecker : MonoBehaviour
{
    [SerializeField] private List<ItemChecker> itemCheckers;
    [SerializeField] private bool forSafe = false;
    [SerializeField] private Animator animator;
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
            else
            {
                AllItemsChecked = false;
                break;
            }
        }
        if (forSafe)
        {
            animator.SetBool("animate", AllItemsChecked);
        }
    }
}
