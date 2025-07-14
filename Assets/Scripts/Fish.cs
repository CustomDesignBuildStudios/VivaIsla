using UnityEngine;

public class Fish : MonoBehaviour
{
    public float swimRadius=5f;        
    public float maxSwimSpeed=2f; 
    public float acceleration=1f;    
    public float directionChangeInterval = 3f; 
    public float slowingDistance = 1.5f;
    public float turnSpeed = 1.5f; 

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float changeDirectionTimer;

    private float currentSpeed = 0f;
    private Vector3 currentDirection;

    void Start()
    {
        this.transform.position = GetRandomPointInBox();
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

        float desiredSpeed = maxSwimSpeed;
        if (distance < slowingDistance)
        {
            desiredSpeed = Mathf.Lerp(0, maxSwimSpeed, distance / slowingDistance);
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);

        if (distance > 0.01f)
        {
            Vector3 desiredDirection = toTarget.normalized;

            currentDirection = Vector3.RotateTowards(currentDirection, desiredDirection, turnSpeed * Time.deltaTime, 0f);
            currentDirection.Normalize();

            transform.position += currentDirection * currentSpeed * Time.deltaTime;

            if (currentDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
        }
    }

    public float minTargetDistance = 2f; 
    public float minY = -1f; 
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

            randomOffset.y = Mathf.Clamp(randomOffset.y, minY, maxY);

            Vector3 candidate = startPosition + randomOffset;

            candidate.x = Mathf.Clamp(candidate.x, boundaryMin.x, boundaryMax.x);
            candidate.y = Mathf.Clamp(candidate.y, boundaryMin.y, boundaryMax.y);
            candidate.z = Mathf.Clamp(candidate.z, boundaryMin.z, boundaryMax.z);

            randomOffset = candidate - startPosition; 

            attempts++;
            if (attempts > 10)
                break;

        } while (randomOffset.magnitude < minTargetDistance);

        targetPosition = startPosition + randomOffset;
        changeDirectionTimer = directionChangeInterval;
    }


    Vector3 GetRandomPointInBox()
    {
        return new Vector3(Random.Range(boundaryMin.x, boundaryMax.x),
                           Random.Range(boundaryMin.y, boundaryMax.y),
                           Random.Range(boundaryMin.z, boundaryMax.z));
    }
}