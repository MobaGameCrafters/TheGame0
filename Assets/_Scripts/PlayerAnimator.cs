using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    [SerializeField] PlayerController controller;
    private const string Is_Running = "IsRunning";
    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }

    private void Update()
    {
        animator.SetBool(Is_Running, controller.IsRunning());        
    }

}
