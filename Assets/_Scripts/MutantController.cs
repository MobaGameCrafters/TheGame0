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
    public bool IsRunning { get; set; }
     }
public class MutantController : NetworkBehaviour, IHealthController
{
    //Rigidbody rb;
    private readonly int damage = 5;
    private NetworkVariable<MutantAnimationState> animationState = new(
    new MutantAnimationState() { IsAttacking = false, IsDying = false, IsRunning=true },
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
    private readonly float speed = 5f;
    private float attackTime=0;
    private readonly int maxHealth = 21;
    [SerializeField] HealthBar healthBar;
    [SerializeField] MutantAnimator animator;
    private NetworkObject target;
    private string line;
    private NavMeshAgent navMeshAgent;
    private Vector3[] path;
 
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        


        //rb = GetComponent<Rigidbody>();
        healthBar.SetMaxHealth(maxHealth);
        navMeshAgent.speed = speed;
        navMeshAgent.autoBraking = true;
        switch (line+gameObject.tag)
        {
            case "midTeam1":
                path = new Vector3[] { new Vector3(125f, 0f, 125f), new Vector3(100f, 0f, 100f), new Vector3(70f, 0f, 70f), new Vector3(55f, 0f, 55f),  new Vector3(25f, 0f, 25f) };
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
        navMeshAgent.autoBraking = true;
    }

    private void Update()
    {
        if (animationState.Value.IsDying) return;
        if (Vector3.Distance(transform.position,navMeshAgent.destination) < navMeshAgent.stoppingDistance)
            {
            if (target == null)
            {
            RemoveWaypoint();
            }else
            {
                navMeshAgent.SetDestination(transform.position);
            }
            SetPathDestination();
        }
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        navMeshAgent.SetDestination(transform.position);
        Debug.Log("entered");
    }*/
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("stayed");
        if (collision.collider.TryGetComponent(out ArrowController arrow))
        {
            return;
        }
        HealthBar oponentHealthBar = collision.gameObject.GetComponentInChildren<HealthBar>();
        if (oponentHealthBar == null || !oponentHealthBar.IsAlive()) {

            animationState.Value.IsAttacking = false;
            animationState.Value.IsRunning = true;
            animator.SetAnimationState(animationState.Value);
            navMeshAgent.enabled = true;
            SetPathDestination();

            return;
        }
        if (!animationState.Value.IsDying && ((collision.gameObject.CompareTag("Team1") && gameObject.tag == "Team2") || (collision.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")) )
        {

            navMeshAgent.enabled = false;
            // target = collision.gameObject;
            if (attackTime == 0 || Time.time - attackTime > 2.2)
            {

                oponentHealthBar.TakeDamage(damage);
                attackTime = Time.time;
            }
            animationState.Value.IsAttacking = true;
            animationState.Value.IsRunning = false;
            animator.SetAnimationState(animationState.Value);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        //target=null;
        animationState.Value.IsAttacking = false;
        animationState.Value.IsRunning = true;
        animator.SetAnimationState(animationState.Value);
        navMeshAgent.enabled = true;
        if (target == null)
        {
            SetPathDestination();
        }
        else
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ArrowController _)) { return; }
        if (((other.gameObject.CompareTag("Team1") && gameObject.tag == "Team2") || (other.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")))
        {
            target = other.gameObject.GetComponent<NetworkObject>();
            navMeshAgent.SetDestination(target.transform.position);
        }
    }
   private void OnTriggerStay(Collider other)
    {
        /* if (other.TryGetComponent(out ArrowController arrow) && target) { return; }
         if (((other.gameObject.CompareTag("Team1") && gameObject.tag == "Team2")|| (other.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")))
         {

                 navMeshAgent.SetDestination(target.transform.position);
                 if (Vector3.Distance(target.transform.position, transform.position) > 1.0f) 
                 else
             {
                 navMeshAgent.SetDestination(transform.position);
             }
             }*/
        if (target != null && navMeshAgent.enabled)
        {
        navMeshAgent.SetDestination(target.transform.position);
        }
    } 
   private void OnTriggerExit(Collider other)
    {
        target = null;
        SetPathDestination();
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

        if (path.Length>0)
        {
        navMeshAgent.SetDestination(path[0]);
        }else
        {
            Despawn();
        }
    }
    private void RemoveWaypoint()
    {
        List<Vector3> pathList = new(path);
        pathList.RemoveAt(0);
        path = pathList.ToArray();
    }
      public void Death()
    {
        animationState.Value.IsAttacking = false;
        animationState.Value.IsDying = true;
        animator.SetAnimationState(animationState.Value);
        gameObject.GetComponent<Collider>().enabled = false;
        //navMeshAgent.SetDestination(transform.position);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        navMeshAgent.enabled = false;
        Invoke(nameof(Despawn), 2f);
    }
    private void Despawn()
    {
        NetworkObject.Despawn(gameObject);
    }
}
