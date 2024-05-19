using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GloctopusHealth : MonoBehaviour
{
    public Slider healthSlider; // Reference to the UI Slider
    public float maxHealth = 100f; // Maximum health value
    private float currentHealth; // Current health value

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    // Method to lose health
    public void LoseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else { 
            // End the game here
        }
        UpdateHealthSlider();
    }

    // Method to gain health
    public void GainHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthSlider();
    }

    // Method to update the Slider's value
    private void UpdateHealthSlider()
    {
        Debug.Log("Player's health now: " + currentHealth);
        healthSlider.value = currentHealth / maxHealth;
    }
}
