using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;



public interface IHealthController
{
    void Death();
    string GetTag();
}
public class HealthBar : MonoBehaviour
{
    public Camera mainCamera;
    private Image healthBar;
    private Image healthBarMax;
    private int health;
    private int maxHealth;
    public IHealthController healthController;
    private void Start()
    {
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.name == "Foreground")
            {
                healthBar = image;
            }
            else if (image.name == "Background")
            {
                healthBarMax = image;
            }
        }
         GameObject parentObject = transform.parent.gameObject;
        mainCamera = Camera.main;
        PlayerController playerController = parentObject.GetComponent<PlayerController>();
        MutantController mutantController = parentObject.GetComponent<MutantController>();

        if (playerController != null)
        {
            healthController = playerController;
        }
        else if (mutantController != null)
        {
            healthController = mutantController;
        }
        string Team = healthController.GetTag();
        if (Team == "Team1") { healthBar.color = new Color(52f / 255f, 152f / 255f, 219f / 255f); }
        else if (Team == "Team2") { healthBar.color= new Color(40f / 255f, 180f / 255f, 99f / 255f); }
    }

    private void Update()
    {

        healthBar.fillAmount = health / maxHealth;
        Vector2 foregroundSize = healthBar.rectTransform.sizeDelta;
        foregroundSize.x = healthBarMax.rectTransform.sizeDelta.x* health / maxHealth;
        healthBar.rectTransform.sizeDelta = foregroundSize;
        Vector3 rotation = transform.position - mainCamera.transform.position;
        rotation.x = 0;
        transform.rotation = Quaternion.LookRotation(rotation);
    }
    public void SetMaxHealth(int healthInput)
    {
        health = healthInput;
        maxHealth = healthInput;
    }
    public void TakeDamage(int damage)
    {  
        health=Math.Max(0,health - damage);
        if (health <= 0) {
        healthController.Death();
        }
    }
}
