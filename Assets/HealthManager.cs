using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    [SerializeField]
    private GameObject deathParticles;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void OnDeath()
    {
        GameObject spawnedDeathParticles = Instantiate(deathParticles, transform);
        spawnedDeathParticles.transform.parent = null;
        spawnedDeathParticles.transform.localScale = Vector3.one * 15f;
        Destroy(spawnedDeathParticles, 5f);
        Destroy(gameObject);
    }

    public void ChangeHealth(int deltaHealh)
    {
        currentHealth = Mathf.Clamp(currentHealth + deltaHealh, 0, maxHealth);
        if (currentHealth == 0)
        {
            OnDeath();
        }
    }
}
