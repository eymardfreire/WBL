using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAnimationController : MonoBehaviour
{
   private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Call this method to trigger the hit animation
    public void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }
}
