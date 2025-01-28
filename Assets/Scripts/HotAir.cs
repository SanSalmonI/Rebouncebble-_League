using UnityEngine;
using System.Collections;

public class HotAir : MonoBehaviour
{
    [SerializeField] private GameObject hotAirObject1;
    [SerializeField] private GameObject particles1;
    [SerializeField] private GameObject hotAirObject2;
    [SerializeField] private GameObject particles2;
    [SerializeField] private GameObject hotAirObject3;
    [SerializeField] private GameObject particles3;

    [SerializeField] private float spawnDuration = 2f; // Time each object remains active
    [SerializeField] private float cooldown = 3f; // Time between activations
    private float timer = 0f; // Tracks elapsed time
    private int currentObjectIndex = 0; // Tracks which object is being activated
    private bool isSpawned = false; // Tracks whether the current object is active

    private GameObject[] hotAirObjects;
    private GameObject[] particleSystems;

    private void Start()
    {

        hotAirObjects = new GameObject[] { hotAirObject1, hotAirObject2, hotAirObject3 };
        particleSystems = new GameObject[] { particles1, particles2, particles3 };

        foreach (var obj in hotAirObjects)
        {
            obj.SetActive(false);
        }
        foreach (var particles in particleSystems)
        {
            particles.SetActive(false);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (isSpawned && timer >= spawnDuration)
        {
            // Deactivate the current object and particles
            DeactivateCurrent();
        }
        else if (!isSpawned && timer >= cooldown)
        {
            // Activate the next object and particles
            ActivateNext();
        }
    }

    private void ActivateNext()
    {
        // Activate the next object and particles
        hotAirObjects[currentObjectIndex].SetActive(true);
        particleSystems[currentObjectIndex].SetActive(true);

        // Update state
        isSpawned = true;
        timer = 0f;
    }

    private void DeactivateCurrent()
    {
        // Deactivate the current object and particles
        hotAirObjects[currentObjectIndex].SetActive(false);
        particleSystems[currentObjectIndex].SetActive(false);

        // Move to the next object
        currentObjectIndex = (currentObjectIndex + 1) % hotAirObjects.Length;

        // Update state
        isSpawned = false;
        timer = 0f;
    }

}
