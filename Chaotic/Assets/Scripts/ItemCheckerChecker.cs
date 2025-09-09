using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ItemCheckerChecker : MonoBehaviour
{
    [SerializeField] private List<ItemChecker> itemCheckers;
    private List<ItemChecker> checkedItems = new List<ItemChecker>();
    private int count;
    private Animator animator;
    private AudioSource audioSource;
    public bool AllItemsChecked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ItemChecker itemChecker in itemCheckers)
        {
            if (itemChecker.HasSucceeded)
            {
                checkedItems.Add(itemChecker);
            }
        }
        if (Check())
        {
            AllItemsChecked = true;
        }
        if (AllItemsChecked)
        {
            animator.SetTrigger("Open");
            audioSource.Play();
        }
    }

    bool Check()
    {
        return itemCheckers.All(item => checkedItems.Contains(item));
    }
}
