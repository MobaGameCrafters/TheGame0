using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
[SerializeField] private float speed = 10f;
    private Camera  _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    private bool isRunning;
    private float totalDistance; // new variable to store the total distance to run
    private float startTime; // new variable to store the start time of the run
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Initialize() {
        _mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn(){
        Initialize();
    }
     private void Update() {
          if (!IsOwner || !Application.isFocused) return;
        //Movement
        if (Input.GetMouseButtonDown(1))
        {
            _mouseInput.x = Input.mousePosition.x;
            _mouseInput.y = Input.mousePosition.y;
            _mouseInput.z = _mainCamera.nearClipPlane;
            endPosition = _mainCamera.ScreenToWorldPoint(_mouseInput);
            isRunning = endPosition != transform.position;
            totalDistance = Vector3.Distance(transform.position, endPosition);
            startTime = Time.time;
            startPosition = transform.position;
 
        }
            if (isRunning)
            {
                // calculate the time it should take to run the total distance
                float journeyLength = totalDistance;
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;


               transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);
 
                // Rotate
                if (endPosition != transform.position)
            {
                Vector3 targetDirection = endPosition - transform.position;
                targetDirection.z = 0f;
                targetDirection.y = 0f;
                transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * 5);
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
