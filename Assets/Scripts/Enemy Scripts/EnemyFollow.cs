using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : EnemyGeneral
{

    public float rotationSpeed = 5f; // Speed of rotation towards the target
    public float movementSpeed = 2f; // Speed of movement towards the target
    protected override void DoWhenSeen()
    {
        this.LookAtTarget();
        this.MoveTowardsTarget();
    }

    private void LookAtTarget()
    {
        Vector3 direction = (this.playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, this.transform.up);
        lookRotation.y = 0;
        lookRotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, this.playerTransform.position, movementSpeed * Time.deltaTime);
    }
}
