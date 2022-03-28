using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A script to control the teleporter used to excape the first combat arena

public class Teleporter : MonoBehaviour
{
    public GameObject teleporterField;
    public Transform destination;
    public Animator teleporterEffectsAnimator;
    private Transform player;

    // Disable the field blocking teleporter access
    public void Unlock()
    {
        teleporterField.SetActive(false);
    }

    // Enable the field blocking teleporter access
    public void Lock()
    {
        teleporterField.SetActive(true);
    }
    
    //If the player enters the teleporter, triggering the collider, then teleport them
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject.transform;
            // Player teleporter animation
            teleporterEffectsAnimator.SetTrigger("Teleporting");
            // After a delay, teleport the player
            Invoke("TeleportPlayerToNewScene", 3f);
        }    
    }

    // If this teleporter function is used then the player will be teleported to the position of a public transform (in the same scene)
    private void TeleportPlayerToDestination()
    {
        player.position = destination.position;
    }

    // If this teleporter function is used then the player will be teleported to the next scene in the build order
    private void TeleportPlayerToNewScene()
    {
        // Increment the build order index then load the corresponding scene
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene - 1);
    }
}
