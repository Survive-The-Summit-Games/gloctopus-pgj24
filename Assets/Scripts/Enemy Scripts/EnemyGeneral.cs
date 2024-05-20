using Pathfinding;
using UnityEngine;

public enum methods { 
    sight, sense, nothing
}


[RequireComponent(typeof(Collider2D))]
public abstract class EnemyGeneral : MonoBehaviour
{
    public float fieldOfViewAngle = 90f; // Field of view in degrees
    public float viewDistance = 5f; // Distance the enemy can see

    public float sense_distance = 5f; // Distance the enemy can sense the player
    public LayerMask targetMask; // Layer mask to detect the player

    protected Transform playerTransform;
    protected GloctopusHealth playerHealth;
    protected AIPath path;

    protected float time_since_action = 0.0f;

    protected methods method_trigger = methods.nothing;

    void Start()
    {
        path = GetComponent<AIPath>();
        playerTransform = GameObject.FindGameObjectWithTag("Gloctopus").transform;
        playerHealth = GameObject.FindGameObjectWithTag("Gloctopus").GetComponent<GloctopusHealth>();
    }

    protected virtual void Update()
    {
        DetectPlayer();
        time_since_action += Time.deltaTime;
    }

    public void DrawCircle(Vector3 center, float radius, Color color, int segments = 36)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float startAngle = i * angleStep;
            float endAngle = (i + 1) * angleStep;

            Vector3 startPoint = GetPointOnCircle(center, radius, startAngle);
            Vector3 endPoint = GetPointOnCircle(center, radius, endAngle);

            Debug.DrawLine(startPoint, endPoint, color);
        }
    }

    private static Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float x = center.x + Mathf.Cos(radian) * radius;
        float y = center.y + Mathf.Sin(radian) * radius;
        return new Vector3(x, y, center.z);
    }

    private void OnDrawGizmos()
    {

        Vector2 rightBoundary = Quaternion.Euler(0, 0, fieldOfViewAngle / 2) * transform.right;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -fieldOfViewAngle / 2) * transform.right;

        Debug.DrawRay(transform.position, rightBoundary * viewDistance, Color.yellow);
        Debug.DrawRay(transform.position, leftBoundary * viewDistance, Color.yellow);

        this.DrawCircle(this.transform.position, this.sense_distance, Color.yellow);
    }

    void DetectPlayer()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= viewDistance && this.method_trigger == methods.nothing)
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
                    method_trigger = methods.sight;
                    this.DoWhenSeen();
                }
            }
        }

        if (distanceToPlayer <= this.sense_distance && this.method_trigger == methods.nothing)
        {
            method_trigger = methods.sense;
            this.DoWhenSeen();
        }

        method_trigger = methods.nothing;
    }

    protected abstract void DoWhenSeen();
}
