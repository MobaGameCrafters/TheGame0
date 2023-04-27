using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;
using UnityEngine.AI;


public class MutantBossAnimationState
{
    public bool IsSwiping { get; set; }
    public bool IsDying { get; set; }
    public bool IsRunning { get; set; }
}
public class MutantBossController : NetworkBehaviour, IHealthController
{
    private NetworkVariable<MutantBossAnimationState> animationState = new(
    new MutantBossAnimationState() { IsSwiping = false, IsDying = false,IsRunning=false },
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
    private Vector3 basePosition = new Vector3(100, 0, 100);
    [SerializeField] HealthBar healthBar;
    private int maxHealth = 50;
    private NavMeshAgent navMeshAgent;
    private readonly float speed = 5f;
    private GameObject target;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthBar.SetMaxHealth(maxHealth);
        navMeshAgent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetTag()
    {
        return gameObject.tag;
    }
    public void Death()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        ArrowController arrow = other.GetComponent<ArrowController>();
        if (arrow == null)
        {
            target = other.gameObject;
            if (Vector3.Distance(target.transform.position, transform.position) > 1f)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        target = null;
        navMeshAgent.SetDestination(basePosition);
    }
}
