using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
[SerializeField] private float speed = 10f;
    private Camera  _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    private bool isRunning;
    private float totalDistance; 
    private float startTime; 
    private Vector2 startPosition;
    private Vector3 endPosition;
    Rigidbody rb;

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
            _mouseInput.x = Input.mousePosition.x;
            _mouseInput.y = Input.mousePosition.y;
            endPosition = _mainCamera.ScreenToWorldPoint(_mouseInput);
            isRunning = endPosition != transform.position;
            totalDistance = Vector2.Distance(transform.position, endPosition);
            startTime = Time.time;
            startPosition = transform.position;

        } }
    private void FixedUpdate()
    {
        if (isRunning)
            {
                // calculate the time it should take to run the total distance
                float journeyLength = totalDistance;
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
            Vector2 newPosition = Vector3.Lerp(startPosition, endPosition, fracJourney);
            rb.MovePosition(newPosition);
            // Rotate
            if (endPosition != transform.position)
            {
                Vector3 targetDirection = endPosition - transform.position;
                float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, angle, 0));

                rb.freezeRotation = true;
                rb.MoveRotation(targetRotation);
                rb.freezeRotation = false;

            }
            // check if we've reached the target position
            if (fracJourney >= 1f)
            {
                isRunning = false;
            }
        }
    }

 

    public bool IsRunning()
    {
        
        return isRunning;
    }
}
