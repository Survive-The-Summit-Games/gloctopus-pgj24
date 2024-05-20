using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum moveType { 
    moving, cooldown 
}

public class EnemyBolt : EnemyGeneral
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 20.0f;
    public float timeToMove = 3.0f;

    protected override void DoWhenSeen()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform not assigned to EnemyBolt!");
            return;
        }

        if (this.method_trigger == methods.sight)
        {
            MoveTowardsTarget();
        } else if (this.method_trigger == methods.sense) 
        {
            LookAtTarget();
        }
    }
    private void LookAtTarget()
    {
        Vector3 direction = (this.playerTransform.position - transform.position).normalized;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, this.playerTransform.position, moveSpeed * Time.deltaTime);
    }
}
