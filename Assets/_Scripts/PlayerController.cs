using Cinemachine;
using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class PlayerController : NetworkBehaviour, IHealthController
{

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameInput gameInput;
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] private Transform arrowSpawningPoint;
    private float speed = 7f;


    private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isShooting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //private float totalDistance; 
    private float MainAttackFireTime = 0;
    private NetworkVariable<Vector3> startPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> endPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> target = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Rigidbody rb;
    private int maxHealth = 21;
    private Vector3 direction;
    //private NetworkVariable<Quaternion> myQuaternion = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NavMeshAgent navMeshAgent;
    [SerializeField] HealthBar healthBar;





    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthBar.SetMaxHealth(maxHealth);
        navMeshAgent.speed = speed;
        navMeshAgent.autoBraking = true;
    }
    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        startPosition.Value = arrowSpawningPoint.position;
        if (Input.GetMouseButtonDown(0) && Time.time - MainAttackFireTime > 1)
        {
            MainAttackFireTime = Time.time;
            isShooting.Value = true;
            isRunning.Value = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                target.Value = hit.point;

            }
            Rotate();

            Invoke(nameof(FireArrow), 0.5f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.destination = hit.point;
            }
        }
        float distanceToTarget = Vector3.Distance(transform.position, navMeshAgent.destination);

        if (distanceToTarget < 0.1f)
        {
            isRunning.Value = false;
        }
        else
        {
            isRunning.Value = true;
           // transform.forward = navMeshAgent.velocity; //new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, navMeshAgent.velocity.z);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            followCamera.Priority = 20;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        { followCamera.Priority = 0; }


    }
    public void Death()
    {

    }
    private void FireArrow()
    {
        Vector3 targetDirection = target.Value - transform.position;
        float angle0 = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;


        float angle;
        if (angle0 >= 0)
        {
            angle = angle0;
        }
        else
        {
            angle = 360 - MathF.Abs(angle0);
        }
        Quaternion arrowRotation = Quaternion.Euler(new Vector3(0, angle, 0));
        FireArrowServerRpc(arrowRotation, new ServerRpcParams());
        isShooting.Value = false;
    }
    [ServerRpc]
    private void FireArrowServerRpc(Quaternion arrowRotation, ServerRpcParams serveRpcParams)
    {

        Debug.Log("sp:" + startPosition.Value + "arp" + arrowSpawningPoint.position);
        GameObject newArrow = Instantiate(arrow, startPosition.Value, arrowRotation);
        newArrow.GetComponent<ArrowController>().SetTag(gameObject.tag);
        newArrow.GetComponent<NetworkObject>().SpawnWithOwnership(serveRpcParams.Receive.SenderClientId);
    }
    private void Rotate()
    {
        Vector3 targetDirection;
        if (!isRunning.Value)
        {
            targetDirection = target.Value - transform.position;
        }
        else
        {
            targetDirection = endPosition.Value - transform.position;
        }


        float angle0 = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;


        float angle;
        if (angle0 >= 0)
        {
            angle = angle0;
        }
        else
        {
            angle = 360 - MathF.Abs(angle0);
        }

        Debug.Log(angle);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, angle, 0));

        rb.freezeRotation = true;
        rb.MoveRotation(targetRotation);
        rb.freezeRotation = false;
    }
    public bool IsRunning()
    {
        return isRunning.Value;
    }
    public bool IsShooting()
    {
        return isShooting.Value;
    }
    public Vector3 EndPosition()
    {
        return endPosition.Value;
    }
    public string GetTag() {
        return gameObject.tag;
    }
}
