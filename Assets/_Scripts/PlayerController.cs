using Cinemachine;
using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class PlayerController : NetworkBehaviour {

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameInput gameInput;
    //[SerializeField] private GameObject cameraContainer;
    //[SerializeField]//
    [SerializeField] CinemachineVirtualCamera followCamera;
    // private CameraScript cameraScript;
    private float speed = 7f;


    private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isShooting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //private float totalDistance; 
    private float MainAttackFireTime = 0;
    private NetworkVariable<Vector3> startPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> endPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> target = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Rigidbody rb;
    private float health = 21;
    private Vector3 direction;
    //private NetworkVariable<Quaternion> myQuaternion = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NavMeshAgent navMeshAgent;





    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // cameraScript = cameraContainer.SetPlayer();



    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent.speed = speed;
        navMeshAgent.autoBraking = true;




    }
    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        /* navMeshAgent.speed = speed;
         navMeshAgent.angularSpeed = 999f;
         navMeshAgent.acceleration = 999f;
         navMeshAgent.stoppingDistance = 0;
         navMeshAgent.autoBraking = true;*/


        if (Input.GetMouseButtonDown(0) && Time.time - MainAttackFireTime > 1)
        {
            MainAttackFireTime = Time.time;
            isShooting.Value = true;
            isRunning.Value = false;
            Rotate();
            startPosition.Value = transform.position;
            gameInput.SetMovementPosition(startPosition.Value);
            //myQuaternion.Value = Quaternion.FromToRotation(Vector3.forward, transform.forward);
            // myQuaternion.Value.Normalize();
            target.Value = gameInput.GetTargetPosition();
            Invoke(nameof(FireArrow), 0.5f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("The ray hit at: " + hit.point);
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
            transform.forward = navMeshAgent.velocity; //new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, navMeshAgent.velocity.z);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            followCamera.Priority = 20;



        }
        if (Input.GetKeyUp(KeyCode.Space))

        { followCamera.Priority = 0; }


    }


    /*private void FixedUpdate()
    {
        Vector3 targetPosition = gameInput.GetMovementVector();
        endPosition.Value = new(targetPosition.x, targetPosition.y, 0);
        direction = endPosition.Value - transform.position;
        direction.Normalize();
        float distanceToTarget = Vector3.Distance(transform.position, endPosition.Value);
        if (distanceToTarget > 0.1f)
        {

            Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
            Debug.Log(transform.position + " - " + newPosition);
            rb.MovePosition(newPosition);

            isRunning.Value = true;
            Rotate();

        }
        else
        {
            isRunning.Value = false;
        }
    }*/
    public void TakeDamage(int damage)
    {
        health = Math.Max(0, health - damage);
    }
    private void FireArrow()
    {
        Vector3 forward = Vector3.forward;
        Vector3 other = -target.Value + transform.position;
        Quaternion arrowRotation = Quaternion.FromToRotation(forward, other);
        FireArrowServerRpc(arrowRotation, new ServerRpcParams());
        isShooting.Value = false;
    }
    [ServerRpc]
    private void FireArrowServerRpc(Quaternion arrowRotation, ServerRpcParams serveRpcParams)
    {

        GameObject newArrow = Instantiate(arrow, startPosition.Value, arrowRotation);
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
        float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
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



}
