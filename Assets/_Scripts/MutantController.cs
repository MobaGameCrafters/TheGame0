using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MutantController : NetworkBehaviour
{
    Rigidbody rb;
    private readonly int damage = 5;
    private NetworkVariable<bool> isAttacking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly float speed = 30f;
    private float attackTime=0;
    private int health = 20;
    private Quaternion originalRotation;
     
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(-3.0f / 2.0f, -1.0f, 0.0f);
        direction.Normalize();
         rb.velocity = speed * Time.fixedDeltaTime * direction;
    }
    private void OnCollisionStay(Collision collision)
    {
        PlayerController character = collision.collider.GetComponent<PlayerController>();
        if (character != null)
        {
            Vector3 direction = -transform.position + character.transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            //transform.LookAt(collision.collider.transform.position);
            if(attackTime == 0 || Time.time - attackTime > 2.2)
            {
            character.TakeDamage(damage);
            attackTime = Time.time;
            }
            rb.velocity = Vector3.zero;
            isAttacking.Value=true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        Vector3 direction = new Vector3(-3.0f / 2.0f, -1.0f, 0.0f);
        direction = Vector3.Normalize(direction);
        transform.rotation = originalRotation;
        rb.velocity = direction * speed * Time.fixedDeltaTime;
        isAttacking.Value = false;
    }
    public bool IsAttacking()
    {
        return isAttacking.Value;
    }
    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
