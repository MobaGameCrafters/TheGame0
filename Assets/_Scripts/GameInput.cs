using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private Vector3 movePosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private void OnLeftClick()
    {
        targetPosition = Mouse.current.position.ReadValue();
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
    }
    private void OnRightClick()
    {
        movePosition = Mouse.current.position.ReadValue();
        movePosition = Camera.main.ScreenToWorldPoint(movePosition);
    }
    public Vector3 GetMovementVector()
    {
               return movePosition;
    }
    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
    public void SetMovementPosition(Vector3 position)
    {
        movePosition = position;
    }
}
