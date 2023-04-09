using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private float moveSpeed = 10f;
    private Vector3 endPosition = Vector3.zero;
    private float totalDistance;
    private float startTime;
    private Vector2 startPosition;
    public int damage = 10;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPosition.z = transform.position.z;
        startTime = Time.time;
        totalDistance = Vector2.Distance(transform.position, endPosition);
        Vector3 targetDirection = endPosition - transform.position;

        float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
        float adjustedAngle = angle;
        if (endPosition.x > startPosition.x)
        {
            adjustedAngle = -angle;
        }
        Quaternion targetRotation = Quaternion.Euler(new Vector3(90- adjustedAngle, angle, 0));
        rb.MoveRotation(targetRotation);
    }
    private void Update()
    {
        
    }

    // Update is called once per frame

    private void FixedUpdate()
    {

            float journeyLength = totalDistance;
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fracJourney = distCovered / journeyLength;
            Vector2 newPosition = Vector3.Lerp(startPosition, endPosition, fracJourney);
            rb.MovePosition(newPosition);
        if (fracJourney >= 1f)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        PlayerController character = other.GetComponent<PlayerController>();
        if (character != null)
        {
            character.TakeDamage(damage);
            Destroy(gameObject); // Destroy the arrow when it hits the character
        }
    }
}
