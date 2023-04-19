using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject mainCamera;
    private Transform healthBar;
    private Transform healthBarMax;
    private float healthBarWidth;
    private int health;
    private int maxHealth;
    private void Start()
    {
        mainCamera = Camera.main.gameObject;
        healthBar = transform.Find("CurrentHP");
        healthBarMax = transform.Find("MaxHP");
        healthBarWidth = healthBar.transform.localScale.x;
    }

    private void Update()
    {
        healthBar.transform.localScale = new Vector3(healthBarWidth * health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //healthBar.transform.position = new Vector3(healthBar.transform.position.x - (healthBarWidth - healthBarWidth * health / maxHealth) / 4, healthBar.transform.position.y, healthBar.transform.position.z);
        healthBarMax.transform.localScale = new Vector3(Mathf.Min(healthBarWidth - healthBarWidth * health / maxHealth, healthBarWidth), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //healthBarMax.transform.position = new Vector3(healthBarMax.transform.position.x - (healthBarWidth - healthBarWidth * health / maxHealth) / 4, healthBarMax.transform.position.y, healthBarMax.transform.position.z);
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
    public void SetHealth(int healthInput)
    {
        health = healthInput;
    }
    public void SetMaxHealth(int healthInput)
    {
        health = healthInput;
        maxHealth = healthInput;
    }
}
