using UnityEngine;

public class PlantSeed : MonoBehaviour
{
    public GameObject seedPrefab; // Prefab of the seed to instantiate
    public float initialHeight = 10f; // Initial height above the target position
    public float gravity = -9.81f; // Gravity acceleration in m/s^2
    private GameObject seedInstance;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float velocity = 0f;
    private bool isMoving = false;

    public void Initialize(GameObject seedPrefab, Vector3 targetPosition)
    {
        this.seedPrefab = seedPrefab;
        // Calculate start position
        startPosition = targetPosition + new Vector3(0, initialHeight, 0);
        endPosition = targetPosition;

        // Instantiate the seed at the start position
        seedInstance = Instantiate(seedPrefab, startPosition, Quaternion.identity);

        // Start moving the seed
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            float deltaTime = Time.deltaTime;

            // Update velocity based on gravity
            velocity += gravity * deltaTime;

            // Update position based on velocity
            Vector3 newPosition = seedInstance.transform.position + new Vector3(0, velocity * deltaTime, 0);

            // Check if the seed has reached or passed the end position
            if (newPosition.y <= endPosition.y)
            {
                newPosition.y = endPosition.y;
                isMoving = false;
                Destroy(gameObject); // Destroy the GameObject to clean up
                Destroy(seedInstance);
            }

            // Update seed position
            seedInstance.transform.position = newPosition;
        }
    }
}
