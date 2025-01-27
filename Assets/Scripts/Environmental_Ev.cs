using UnityEngine;

public class Environmental_Ev : MonoBehaviour
{
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    [SerializeField] private GameObject eventObject;
    private GameObject currentInstance; 
    [SerializeField]private float spawnDuration; 
    [SerializeField]private float cooldown; 
    private float timer = 0f; 
    private bool isSpawned = false; 

    void Update()
    {
        timer += Time.deltaTime;

        if (isSpawned && timer >= spawnDuration)
        {
            // Destroy the current object after its duration
            Destroy(currentInstance);
            isSpawned = false;
            timer = 0f; // Reset the timer for cooldown
        }
        else if (!isSpawned && timer >= cooldown)
        {
            // Spawn a new object after the cooldown
            Vector3 randomPos = RandomPositionCircle();
            currentInstance = Instantiate(eventObject, randomPos, Quaternion.identity);
            isSpawned = true;
            timer = 0f; // Reset the timer for the spawn duration
        }
    }

    // Random position within the bounds of the square
    private Vector3 RandomPositionSquare()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);
        return new Vector3(x, y, z);
    }

    // Random position within the bounds of the circle
    private Vector3 RandomPositionCircle()
    {
        // Define the center and radius of the circle
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        float radius = Mathf.Min(maxX - minX, maxZ - minZ) / 2; // Set radius based on bounds

        // Random angle and radius
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        // Get random position within the circle on the XZ plane
        float x = center.x + distance * Mathf.Cos(angle);
        float z = center.z + distance * Mathf.Sin(angle);
        float y = Random.Range(minY, maxY); // Random Y within the given bounds

        return new Vector3(x, y, z);
    }
}
