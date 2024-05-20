using Pathfinding;
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
        Vector3 direction = (path.steeringTarget - transform.position).normalized;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveTowardsTarget()
    {
        this.path.destination = this.playerTransform.position; 
        //transform.position = Vector3.MoveTowards(transform.position, this.playerTransform.position, movementSpeed * Time.deltaTime);
    }
}
