using UnityEngine;

public class FishSchool : MonoBehaviour
{
    public GameObject[] fishGroup;        // Assign fish prefabs/objects in the inspector
    public float moveSpeed = 2f;          // Speed the school moves
    public float resetDistance = 20f;     // Distance from start after which fish reset

    private Vector3[] startPositions;     // Starting positions of each fish
    public string stateName = "Swim"; // Name of the animation state (case-sensitive)
    public float animationLength = 1f; // Duration of the animation in seconds (can get from Animator if needed)

    void Start()
    {
        for (int i = 0; i < fishGroup.Length; i++)
        {
            Animator animator = fishGroup[i].GetComponent<Animator>();
            if (animator != null)
            {
                float offset = Random.Range(0f, animationLength);
                animator.Play(stateName, 0, offset / animationLength);
            }
        }
    
        // Store initial positions of all fish
        startPositions = new Vector3[fishGroup.Length];
        for (int i = 0; i < fishGroup.Length; i++)
        {
            startPositions[i] = fishGroup[i].transform.position;
        }
    }

    void Update()
    {
        for (int i = 0; i < fishGroup.Length; i++)
        {
            GameObject fish = fishGroup[i];

            // Move fish in the direction it is facing (local forward)
            fish.transform.position += fish.transform.forward * moveSpeed * Time.deltaTime;

            // Check if fish has traveled too far from its start position
            float distance = Vector3.Distance(fish.transform.position, startPositions[i]);
            if (distance > resetDistance)
            {
                fish.transform.position = startPositions[i];
            }

            // Optional: If you want the fish to slowly rotate (e.g., for animation purposes),
            // you could add random turning here.
        }
    }


}