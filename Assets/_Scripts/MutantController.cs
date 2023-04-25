using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class MutantAnimationState
{
    public bool IsAttacking { get; set; }
    public bool IsDying { get; set; }
     }
public class MutantController : NetworkBehaviour, IHealthController
{
    NetworkRigidbody rb;
    private readonly int damage = 5;
    private NetworkVariable<MutantAnimationState> animationState = new(
    new MutantAnimationState() { IsAttacking = false, IsDying = false },
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
    private readonly float speed = 5f;
    private float attackTime=0;
    private readonly int maxHealth = 21;
       [SerializeField] HealthBar healthBar;
    [SerializeField] MutantAnimator animator;
    private GameObject target;
    private bool inRange;
    private string line;
    private int targetIndex = 0;
    private NavMeshAgent navMeshAgent;
    private Vector3[] path;
 
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        


        rb = GetComponent<NetworkRigidbody>();
        healthBar.SetMaxHealth(maxHealth);
        navMeshAgent.speed = speed;
        //navMeshAgent.autoBraking = true;
        switch (line+gameObject.tag)
        {
            case "midTeam1":
                path = new Vector3[] { new Vector3(10f, 0f, 10f) };
                break;
            case "midTeam2":
                path = new Vector3[] { new Vector3(190f, 0f, 190f) };
                break;
            case "botTeam1":
                path = new Vector3[] { new Vector3(188f, 0f, 24f), new Vector3(176f, 0f, 12f), new Vector3(12f, 0f, 12f) };
                break;
            case "botTeam2":
                path = new Vector3[] { new Vector3(176f, 0f, 12f), new Vector3(188f, 0f, 24f), new Vector3(188f, 0f, 188f) };
                break;
            case "topTeam1":
                path = new Vector3[] { new Vector3(24f, 0f, 188f), new Vector3(12f, 0f, 176f), new Vector3(12f, 0f, 12f) };
                break;
            case "topTeam2":
                path = new Vector3[] { new Vector3(12f, 0f, 176f), new Vector3(24f, 0f, 188f), new Vector3(188f, 0f, 188f) };
                break;
        }
        SetPathDestination();
        navMeshAgent.stoppingDistance = 1.0f;
    }

    private void Update()
    {
        if (animationState.Value.IsDying) return;
        if (inRange)
        {
            if(Vector3.Distance(target.transform.position,transform.position)>1f)
            navMeshAgent.SetDestination(target.transform.position);
        }
        else if (Vector3.Distance(transform.position,navMeshAgent.destination) < navMeshAgent.stoppingDistance)
            {
            SetPathDestination();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        ArrowController arrow = collision.collider.GetComponent<ArrowController>();

        if (arrow == null) {
             if (!animationState.Value.IsDying && ((collision.gameObject.CompareTag("Team1") && gameObject.tag == "Team2") || (collision.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")) )
        {
            target = collision.gameObject;
            if (attackTime == 0 || Time.time - attackTime > 2.2)
            {
                HealthBar healthBar = collision.gameObject.GetComponentInChildren<HealthBar>();
                healthBar.TakeDamage(damage);
                attackTime = Time.time;
            }
            animationState.Value.IsAttacking = true;
            animator.SetAnimationState(animationState.Value);
        }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
         target=null;
        animationState.Value.IsAttacking = false;
        animator.SetAnimationState(animationState.Value);
    }
      public void Death()
    {
        animationState.Value.IsAttacking = false;
        animationState.Value.IsDying = true;
        animator.SetAnimationState(animationState.Value);
        gameObject.GetComponent<Collider>().enabled = false;
        Invoke(nameof(Despawn), 2f);
    }
    private void Despawn()
    {
        NetworkObject.Despawn(gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        ArrowController arrow = other.GetComponent<ArrowController>();
        if (arrow == null) { 
        if (((other.gameObject.CompareTag("Team1") && gameObject.tag == "Team2")|| (other.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")))
        {
   
                target = other.gameObject;
                inRange = true;
            }
        }
    }
   private void OnTriggerExit(Collider other)
    {
        target = null;
        navMeshAgent.SetDestination(path[targetIndex]);
        inRange = false;
    }
    public void SetTag(string tag)
    {
        gameObject.tag = tag;
    }
    public string GetTag()
    {
        return gameObject.tag;
    }
    public void SetLine(string inputLine)
    {
        line = inputLine;
    }
    private void SetPathDestination()
    {

        if (targetIndex < path.Length)
        {
        navMeshAgent.SetDestination(path[targetIndex]);
        targetIndex++;
        }else
        {
            Despawn();
        }
    }
}
