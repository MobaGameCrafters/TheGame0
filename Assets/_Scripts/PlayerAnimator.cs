using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    [SerializeField] PlayerController controller;
    private const string Is_Running = "IsRunning";
    private const string Is_Shooting = "IsShooting";
    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }

    private void Update()
    {
      //  animator.SetBool(Is_Running, controller.IsRunning());
       // animator.SetBool(Is_Shooting, controller.IsShooting());
    }
    public void SetAnimationState(PlayerAnimationState state)
    {
        animator.SetBool(Is_Running, state.isRunning);
        animator.SetBool(Is_Shooting, state.isShooting);
    }
}
