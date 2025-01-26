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
            Vector3 randomPos = RandomPosition();
            currentInstance = Instantiate(eventObject, randomPos, Quaternion.identity);
            isSpawned = true;
            timer = 0f; // Reset the timer for the spawn duration
        }
    }

    private Vector3 RandomPosition()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);
        return new Vector3(x, y, z);
    }
}
