using UnityEngine;

public class FishSchool : MonoBehaviour
{
    public GameObject[] fishGroup;       
    public float moveSpeed = 2f;         
    public float resetDistance = 20f;    

    private Vector3 startPosition;  
    public string stateName = "Swim"; 
    public float animationLength = 1f; 

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

        startPosition = this.transform.position;

    }

    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, startPosition);
        if (distance > resetDistance)
        {
            transform.position = startPosition;
        }
    }


}