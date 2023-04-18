using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MutantController : NetworkBehaviour
{
    Rigidbody rb;
    private readonly int damage = 5;
    private NetworkVariable<bool> isAttacking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isDying = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly float speed = 30f;
    private float attackTime=0;
    private int health;
    private int maxHealth = 21;
    private Quaternion originalRotation;
    private Transform healthBar;
    private Transform healthBarMax;
    private float healthBarWidth;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
        healthBar = transform.Find("healthbar").Find("CurrentHP");
        healthBarMax = transform.Find("healthbar").Find("MaxHP");
        healthBarWidth = healthBar.transform.localScale.x;
        health = maxHealth;
    }

    private void FixedUpdate()
    {
        Vector3 direction = new(-3.0f / 2.0f, -1.0f, 0.0f);
        direction.Normalize();
        if (!isDying.Value)
        {
        rb.velocity = speed * Time.fixedDeltaTime * direction;
        } 
    }
    private void OnCollisionStay(Collision collision)
    {
        PlayerController character = collision.collider.GetComponent<PlayerController>();
        if (character != null && !isDying.Value)
        {
            Vector3 direction = -transform.position + character.transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
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
        healthBar.transform.localScale = new Vector3( healthBarWidth*health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        healthBar.transform.position = new Vector3(healthBar.transform.position.x-(healthBarWidth - healthBarWidth * health / maxHealth)/4, healthBar.transform.position.y, healthBar.transform.position.z);
        healthBarMax.transform.localScale = new Vector3(Mathf.Min( healthBarWidth - healthBarWidth * health / maxHealth,healthBarWidth), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        healthBarMax.transform.position = new Vector3(healthBarMax.transform.position.x - (healthBarWidth-healthBarWidth * health / maxHealth) / 4, healthBarMax.transform.position.y, healthBarMax.transform.position.z);
    }
    private void Death()
    {
        Destroy(gameObject);
    }
}
