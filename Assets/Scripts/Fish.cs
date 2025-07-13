using UnityEngine;

public class Fish : MonoBehaviour
{
    public float swimRadius = 5f;                // Max distance from start point
    public float maxSwimSpeed = 2f;              // Max movement speed
    public float acceleration = 1f;               // How fast fish accelerates/decelerates
    public float directionChangeInterval = 3f;  // How often the fish picks a new target
    public float slowingDistance = 1.5f;         // Distance at which fish starts slowing down
    public float turnSpeed = 1.5f;                // How fast the fish turns (radians per second)

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float changeDirectionTimer;

    private float currentSpeed = 0f;
    private Vector3 currentDirection;

    void Start()
    {
        this.transform.position = GetRandomPointInBoundary();
        startPosition = transform.position;
        PickNewTarget();
        currentDirection = transform.forward;

        
    }

    void Update()
    {
        changeDirectionTimer -= Time.deltaTime;

        if (changeDirectionTimer <= 0f)
        {
            PickNewTarget();
        }

        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance < slowingDistance)
        {
            PickNewTarget();
        }

        // Determine desired speed - slow down when close to target
        float desiredSpeed = maxSwimSpeed;
        if (distance < slowingDistance)
        {
            desiredSpeed = Mathf.Lerp(0, maxSwimSpeed, distance / slowingDistance);
        }

        // Smoothly accelerate/decelerate to desired speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);

        if (distance > 0.01f)
        {
            Vector3 desiredDirection = toTarget.normalized;

            // Smoothly rotate currentDirection toward desiredDirection over time using Vector3.RotateTowards
            currentDirection = Vector3.RotateTowards(currentDirection, desiredDirection, turnSpeed * Time.deltaTime, 0f);
            currentDirection.Normalize();

            // Move forward in currentDirection
            transform.position += currentDirection * currentSpeed * Time.deltaTime;

            // Rotate the fish to face the currentDirection
            if (currentDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Slow down smoothly if at target
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
        }
    }

    public float minTargetDistance = 2f; // new!
    public float minY = -1f; // minimum Y offset relative to startPosition
    public float maxY = 1f;
    public Vector3 boundaryMin = new Vector3(-10f, -2f, -10f);
    public Vector3 boundaryMax = new Vector3(10f, 2f, 10f);
    void PickNewTarget()
    {
        Vector3 randomOffset;
        int attempts = 0;
        do
        {
            randomOffset = Random.insideUnitSphere * swimRadius;

            // Clamp Y between minY and maxY (if you have these variables)
            randomOffset.y = Mathf.Clamp(randomOffset.y, minY, maxY);

            Vector3 candidate = startPosition + randomOffset;

            // Clamp candidate position to boundaries
            candidate.x = Mathf.Clamp(candidate.x, boundaryMin.x, boundaryMax.x);
            candidate.y = Mathf.Clamp(candidate.y, boundaryMin.y, boundaryMax.y);
            candidate.z = Mathf.Clamp(candidate.z, boundaryMin.z, boundaryMax.z);

            randomOffset = candidate - startPosition; // Adjust offset after clamping

            attempts++;
            if (attempts > 10)
                break;

        } while (randomOffset.magnitude < minTargetDistance);

        targetPosition = startPosition + randomOffset;
        changeDirectionTimer = directionChangeInterval;
    }


    Vector3 GetRandomPointInBoundary()
    {
        return new Vector3(Random.Range(boundaryMin.x, boundaryMax.x),
                           Random.Range(boundaryMin.y, boundaryMax.y),
                           Random.Range(boundaryMin.z, boundaryMax.z));
    }
}