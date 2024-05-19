using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public abstract class EnemyGeneral : MonoBehaviour
{
    public float fieldOfViewAngle = 90f; // Field of view in degrees
    public float viewDistance = 5f; // Distance the enemy can see
    public LayerMask targetMask; // Layer mask to detect the player

    protected Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Gloctopus").transform;
    }

    void Update()
    {
        DetectPlayer();
    }

    private void OnDrawGizmos()
    {

        Vector2 rightBoundary = Quaternion.Euler(0, 0, fieldOfViewAngle / 2) * transform.right;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -fieldOfViewAngle / 2) * transform.right;

        Debug.DrawRay(transform.position, rightBoundary * viewDistance, Color.yellow);
        Debug.DrawRay(transform.position, leftBoundary * viewDistance, Color.yellow);
    }

    void DetectPlayer()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Draw the FOV boundaries

        if (distanceToPlayer <= viewDistance)
        {
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

            if (angleToPlayer <= fieldOfViewAngle / 2f)
            {
                // Draw a ray towards the player
                Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.red);

                // Check if there's a clear line of sight to the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, targetMask);

                if (hit.collider != null && hit.collider.CompareTag("Gloctopus"))
                {
                    this.DoWhenSeen();
                }
            }
        }
    }

    protected abstract void DoWhenSeen();
}
