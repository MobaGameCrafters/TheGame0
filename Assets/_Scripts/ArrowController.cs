using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowController : MonoBehaviour {
    private float moveSpeed = 300f;
    private Vector2 startPosition;
    public int damage = 10;
    Rigidbody rb;
    Vector3 forward;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        forward = transform.forward;
        forward.z *= 1;
        forward.y *= (1);
        forward.x *= (1);
        forward.Normalize();
        Transform objectTransform = gameObject.transform;

        float xAngle = objectTransform.rotation.eulerAngles.x;
        float zAngle = objectTransform.rotation.eulerAngles.z;
        if (forward.x < 0f)
        {
            xAngle = 180 - xAngle;
        }
        Quaternion newRotation = Quaternion.Euler(xAngle, 0, zAngle);
        objectTransform.rotation = newRotation;
        // rb.MovePosition(startPosition);
    }
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = forward;
        rb.velocity = forward * moveSpeed * Time.fixedDeltaTime;
        transform.forward = -rb.velocity;
    }
    void OnTriggerEnter(Collider other)
    {
        //NetworkObject arrow = gameObject.GetComponent<NetworkObject>();
        if ((other.gameObject.CompareTag("Team1") && gameObject.tag == "Team2") || (other.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")) {
            /*PlayerController character = other.GetComponent<PlayerController>();
        if (character != null && arrow.OwnerClientId != character.OwnerClientId)
        {
            character.TakeDamage(damage);
            Destroy(gameObject);
        }
        MutantController minion = other.GetComponent<MutantController>();
        if (minion != null)
        {
            minion.TakeDamage(damage);*/
            HealthBar healthBar = other.GetComponentInChildren<HealthBar>();
            healthBar.TakeDamage(damage);
            Destroy(gameObject);
        
        }
    }
    public void SetTag(string tag)
    {
        gameObject.tag = tag;
    }
}
