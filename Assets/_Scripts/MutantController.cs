using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MutantController : NetworkBehaviour
{
    Rigidbody rb;
    private readonly int damage = 5;
    private NetworkVariable<bool> isAttacking = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isDying = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly float speed = 30f;
    private float attackTime=0;
    private int health;
    private readonly int maxHealth = 21;
    private Quaternion originalRotation;
    [SerializeField] HealthBar healthBar;
    private readonly string characterTag = "Character";
    private Vector3 targetPosition;
    private bool inRange;
    private Vector3 baseDirection;
  

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
        health = maxHealth;
        healthBar.SetHealth(health);
        healthBar.SetMaxHealth(maxHealth);
        if(gameObject.tag == "Team1")
        {
            baseDirection= new(-1.0f, 0.0f, -1.0f);
        }
        else if (gameObject.tag == "Team2")
        {
            baseDirection = new(1.0f, 0.0f, 1.0f);
        }
        baseDirection.Normalize();
    }
    

    private void FixedUpdate()
    {
        if (!inRange)
        {

        if (!isDying.Value)
        {
                transform.rotation = Quaternion.LookRotation(baseDirection);
                rb.velocity = speed * Time.fixedDeltaTime * baseDirection;
        } 
        } 
        if (inRange && !isAttacking.Value) 
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            Vector3 direction = (targetPosition - transform.position);
            direction.Normalize();
            direction.y = 0f;
            if (!isDying.Value && distance>1f)
            {
            transform.rotation = Quaternion.LookRotation(direction);
                rb.velocity = speed * Time.fixedDeltaTime * direction;
            }
        }
        if(isAttacking.Value)
        {
            rb.velocity = Vector3.zero;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        //PlayerController character = collision.collider.GetComponent<PlayerController>();
        if (!isDying.Value && ((collision.gameObject.CompareTag("Team1") && gameObject.tag == "Team2") || (collision.gameObject.CompareTag("Team2") && gameObject.tag == "Team1")))
        {
            Vector3 direction = -transform.position + collision.transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            if(attackTime == 0 || Time.time - attackTime > 2.2)
            {
                PlayerController character = collision.collider.GetComponent<PlayerController>();
                if (character) { character.TakeDamage(damage); }
                MutantController npc = collision.collider.GetComponent<MutantController>();
                if (npc) { npc.TakeDamage(damage); }
                attackTime = Time.time;
            }
            rb.velocity = Vector3.zero;
            isAttacking.Value=true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
         transform.rotation = originalRotation;
        rb.velocity = baseDirection * speed * Time.fixedDeltaTime;
        isAttacking.Value = false;
    }
    public bool IsAttacking()
    {
        return isAttacking.Value;
    }
    public bool IsDying()
    {
        return isDying.Value;
    }
    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            rb.velocity = Vector3.zero;
            isDying.Value = true;
            gameObject.GetComponent<Collider>().enabled = false;
            Invoke(nameof(Death), 2.0f);
        }
        healthBar.SetHealth(health);

    }
    private void Death()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Team1") && gameObject.tag == "Team2")|| (other.gameObject.CompareTag("Team2") && gameObject.tag == "Team1"))
        {
   
                targetPosition = other.transform.position;
            targetPosition.y = 0;
                inRange = true;
    
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(characterTag))
        {

            targetPosition = other.transform.position;
            inRange = true;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }
    public void SetTag(string tag)
    {
        gameObject.tag = tag;
    }
}
