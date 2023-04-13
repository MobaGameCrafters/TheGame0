using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantAnimator : MonoBehaviour
{
    private Animator animator;
    [SerializeField] MutantController controller;
    private const string Is_Attacking = "IsAttacking";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(Is_Attacking, controller.IsAttacking());
    }

}

