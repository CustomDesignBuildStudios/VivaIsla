using UnityEngine;


public class WaterFloat : MonoBehaviour
{
    public float floatStrength = 0.5f;    
    public float floatSpeed = 1f;       
    
    public float tiltStrength = 5f;   
    
    public float tiltSpeed = 1f;       

    private Vector3 startPos;
    private float randomOffset;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        randomOffset = Random.Range(0f,2f *Mathf.PI);
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed + randomOffset)*floatStrength;

        float tiltX = Mathf.Sin(Time.time * tiltSpeed +randomOffset) * tiltStrength;
        float tiltZ = Mathf.Cos(Time.time * tiltSpeed + randomOffset)*tiltStrength;

        transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);
        transform.localRotation = Quaternion.Euler(startRot.eulerAngles.x + tiltX, startRot.eulerAngles.y, startRot.eulerAngles.z + tiltZ);
    }
}