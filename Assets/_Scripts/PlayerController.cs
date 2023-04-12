using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
[SerializeField] private float speed = 10f;
    [SerializeField] private GameObject arrow;
    [SerializeField] TextMeshProUGUI hpBar;
    private Camera  _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isShooting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private float totalDistance; 
    private float startTime; 
    private NetworkVariable<Vector3> startPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> endPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Rigidbody rb;
    private float health=20;
    private NetworkVariable<Quaternion> myQuaternion = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private void Initialize() {
        _mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn(){
        Initialize();
    }
    private void Update() {
        rb = GetComponent<Rigidbody>();
        if (!IsOwner || !Application.isFocused) return;
  
            //Movement
            if (Input.GetMouseButtonDown(1))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _mouseInput.x = Input.mousePosition.x;
            _mouseInput.y = Input.mousePosition.y;
            endPosition.Value = _mainCamera.ScreenToWorldPoint(_mouseInput);
            startPosition.Value = transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the hit object is a character
                if (hit.collider.CompareTag("Character") && gameObject.GetComponent<NetworkObject>().OwnerClientId != hit.collider.GetComponent<NetworkObject>().OwnerClientId)
                {
                    isShooting.Value = true;
                    isRunning.Value = false;
                    Rotate();
                    myQuaternion.Value = Quaternion.FromToRotation(Vector3.forward, transform.forward);
                    myQuaternion.Value.Normalize();
                    Invoke(nameof(FireArrow), 0.5f);
                }
            }else
            {
                isRunning.Value = endPosition.Value != transform.position;
                totalDistance = Vector2.Distance(transform.position, endPosition.Value);
                startTime = Time.time;
            }

        } }


    private void FixedUpdate()
    {
        if (isRunning.Value)
            {
                // calculate the time it should take to run the total distance
                float journeyLength = totalDistance;
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
            Vector2 newPosition = Vector3.Lerp(startPosition.Value, endPosition.Value, fracJourney);
            rb.MovePosition(newPosition);
            // Rotate
            if (endPosition.Value != transform.position)
            {
                Rotate();
            }
            // check if we've reached the target position
            if (fracJourney >= 1f && gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.LocalClientId)
            {
                isRunning.Value = false;
            }
        }
    }
    public void TakeDamage(int damage)
    {
        health= Math.Max(0,health-damage);
        Debug.Log(health);
    }
    private void FireArrow()
    {
        Vector3 forward = Vector3.forward;
        Vector3 other = -endPosition.Value + transform.position;
        Quaternion arrowRotation = Quaternion.FromToRotation(forward, other);
        FireArrowServerRpc(arrowRotation, new ServerRpcParams());
        isShooting.Value = false;
    }
    [ServerRpc]
     private void FireArrowServerRpc(Quaternion arrowRotation,ServerRpcParams serveRpcParams)
    {
        
        GameObject newArrow = Instantiate(arrow, startPosition.Value, arrowRotation);
        newArrow.GetComponent<NetworkObject>().SpawnWithOwnership(serveRpcParams.Receive.SenderClientId);
     }
    private void Rotate()
    {
        Vector3 targetDirection = endPosition.Value - transform.position;
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
