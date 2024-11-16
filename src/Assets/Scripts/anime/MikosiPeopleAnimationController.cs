using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikosiPeopleAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;  // アニメーターコンポーネント取得用
    [SerializeField] private MikosiAnimationController mikosiAnimationController;

    void Start()
    {
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, Random.Range(0f, 1f));
    }
    private void Update()
    {
        if (this.transform.position.y > -0.35 && !animator.GetBool("Jump"))
        {
            animator.SetBool("Jump", true);
            if (mikosiAnimationController != null)
                mikosiAnimationController.Jump();
        }
            
        else if (this.transform.position.y <= -3.15 && animator.GetBool("Jump"))
        {
            animator.SetBool("Jump", false);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, Random.Range(0f, 1f));
            if (mikosiAnimationController != null)
                mikosiAnimationController.JumpDown();
        }
    }
}
