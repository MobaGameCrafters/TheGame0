using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantAnimator : MonoBehaviour
{
    private Animator animator;
    [SerializeField] MutantController controller;
    private const string Is_Attacking = "IsAttacking";
    private const string Is_Dying = "IsDying";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
       // animator.SetBool(Is_Dying, controller.IsDying());
       //animator.SetBool(Is_Attacking, controller.IsAttacking());
    }
    public void SetAnimationState(MutantAnimationState state)
    {
        animator.SetBool(Is_Dying, state.IsDying);
        animator.SetBool(Is_Attacking, state.IsAttacking);
    }
}

