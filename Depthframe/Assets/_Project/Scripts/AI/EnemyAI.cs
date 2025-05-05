using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    private int currentPatrolIndex = 0;

    [Header("Detection Settings")]
    public float viewRadius = 5f;
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float maxChaseDistance = 10f;
    public float loseInterestTime = 3f;

    [Header("Investigation Settings")]
    public float investigationDuration = 5f;
    public float investigationSpeed = 2.5f;

    private Transform player;
    private Vector3 lastKnownPosition;
    private float loseInterestTimer;
    private AIState currentState;
    private Vector3 investigationPoint;
    private float investigationTimer;

    private enum AIState
    {
        Patrol,
        Investigate,
        Chase,
        ReturnToPatrol
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = AIState.Patrol;
    }

    private void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                CheckForDetection();
                break;

            case AIState.Investigate:
                Investigate();
                CheckForDetection();
                break;

            case AIState.Chase:
                ChaseTarget();
                break;

            case AIState.ReturnToPatrol:
                ReturnToPatrol();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 targetPosition = patrolPoints[currentPatrolIndex].position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // Update rotation to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void CheckForDetection()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        foreach (Collider2D target in targetsInViewRadius)
        {
            Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
            float angleToTarget = Vector2.Angle(transform.right, directionToTarget);
            
            Debug.DrawLine(transform.position, target.transform.position, Color.yellow, 0.1f);
            Debug.Log($"Angle to target: {angleToTarget}, View Angle: {viewAngle/2}");

            if (angleToTarget < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);
                
                if (!hit)
                {
                    PlayerStealth playerStealth = target.GetComponent<PlayerStealth>();
                    if (playerStealth != null)
                    {
                        playerStealth.OnSpottedInDarkness();
                    }
                    
                    lastKnownPosition = target.transform.position;
                    currentState = AIState.Chase;
                    return;
                }
            }
        }
    }

    private void ChaseTarget()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        
        // Update rotation to face player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (distanceToPlayer > maxChaseDistance)
        {
            loseInterestTimer += Time.deltaTime;
            if (loseInterestTimer >= loseInterestTime)
            {
                currentState = AIState.ReturnToPatrol;
                loseInterestTimer = 0;
            }
        }
        else
        {
            loseInterestTimer = 0;
            transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }
    }

    private void ReturnToPatrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 nearestPatrolPoint = FindNearestPatrolPoint();
        Vector3 direction = (nearestPatrolPoint - transform.position).normalized;
        
        // Update rotation to face return direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = Vector3.MoveTowards(transform.position, nearestPatrolPoint, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nearestPatrolPoint) < 0.1f)
        {
            currentState = AIState.Patrol;
        }
    }

    private Vector3 FindNearestPatrolPoint()
    {
        Vector3 nearestPoint = patrolPoints[0].position;
        float nearestDistance = Vector3.Distance(transform.position, nearestPoint);

        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < nearestDistance)
            {
                nearestPoint = patrolPoints[i].position;
                nearestDistance = distance;
                currentPatrolIndex = i;
            }
        }

        return nearestPoint;
    }

    private void Investigate()
    {
        Vector3 direction = (investigationPoint - transform.position).normalized;
        
        // Update rotation to face investigation point
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = Vector3.MoveTowards(transform.position, investigationPoint, investigationSpeed * Time.deltaTime);

        investigationTimer -= Time.deltaTime;
        if (investigationTimer <= 0)
        {
            currentState = AIState.ReturnToPatrol;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw view radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Draw view angle
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    private Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}

/*To use this script:

1. Create an empty GameObject in your scene and name it "Enemy"
2. Attach this script to the Enemy GameObject
3. Set up the following in the Inspector:
   - Add patrol points (empty GameObjects) to the Patrol Points array
   - Configure the view radius and angle
   - Set up the appropriate layers for target and obstacle masks
   - Adjust speeds and timers as needed
The script includes visual debugging (Gizmos) to help you see the enemy's field of vision in the Scene view.

Remember to:

- Tag your player GameObject with the "Player" tag
- Set up appropriate layers for your player and obstacles
- Create patrol points in your scene and assign them to the enemy
You can trigger investigations by calling the InvestigatePoint(Vector3 point) method from other scripts (e.g., when a noise is made or a light is turned on).*/