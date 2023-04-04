using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
[SerializeField] private float speed = 3f;
    private Camera  _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    
    private void Initialize() {
        _mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn(){
        Initialize();
    }
    private void Update() {
        if(!IsOwner || !Application.isFocused) return;
//Movement
       _mouseInput.x = Input.mousePosition.x;
       _mouseInput.y = Input.mousePosition.y;
       _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
        transform.position = Vector3.MoveTowards(current:transform.position,target:mouseWorldCoordinates, maxDistanceDelta: Time.deltaTime * speed);

// Rotate
if(mouseWorldCoordinates != transform.position){
    Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0f;
    transform.up = targetDirection;
}

    }   
}
