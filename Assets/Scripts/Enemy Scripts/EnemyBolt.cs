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
        Vector3 direction = (this.playerTransform.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(direction, this.transform.up);
        lookRotation.y = 0;
        lookRotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, this.playerTransform.position, moveSpeed * Time.deltaTime);
    }
}
