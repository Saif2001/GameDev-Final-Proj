using UnityEngine;

// An enum containing the names of all possible power-up types. The enum is public and declared in the global scope so can be used in other scripts
public enum PowerUpType
{
    speedBoost,
    jumpBoost,
    restoreHealth
}

[RequireComponent(typeof(Collider))]
public class PowerUpController : MonoBehaviour
{
    public PlayerController playerController;

    // The type of this power-up, can be chosen in the inspector
    public PowerUpType powerUpType = PowerUpType.speedBoost;

    // Power-up parameters
    public float powerUpTime = 4f;

    public float speedMultiplier = 2f;
    public float jumpMultiplier = 1.5f;
    public float healthRestored = 25f;

    // Whenever a collider hits the power-up
    void OnTriggerEnter(Collider other)
    {
        // Only power-up the player
        if (other.tag == "Player")
        {
            // Grant a different effect depending on the selected powerUpType
            switch (powerUpType)
            {
                case PowerUpType.speedBoost:
                    SpeedBoost();
                    break;
                case PowerUpType.jumpBoost:
                    JumpBoost();
                    break;
                case PowerUpType.restoreHealth:
                    RestoreHealth();
                    break;
                default:
                    Debug.Log("Error in PowerUpController.cs : OnTriggerEnter(). Power-up type '" + powerUpType + "' not recognised");
                    break;
            }
            // Deactivate the game object to remove the power-up once used
            this.gameObject.SetActive(false);
        }
    }

    private void SpeedBoost()
    {
        // Apply the speed multiplier to the movement speed from the player controller script
        playerController.movementSpeed *= speedMultiplier;
        // Call the revertPowerUp() function after a delay to put player movement speed back to normal
        Invoke("RevertPowerUp", powerUpTime);
    }

    private void JumpBoost()
    {
        // Apply multiplier to player jump force, then revert after a delay
        playerController.jumpForce *= jumpMultiplier;
        Invoke("RevertPowerUp", powerUpTime);
    }

    private void RestoreHealth()
    {
        // Get the player's health from the player controller
        float health = playerController.health;
        // Add to the player's health
        health += healthRestored;
        // Clamp the player's health to its maximum value
        if (health > playerController.maximumHealth)
        {
            health = playerController.maximumHealth;
        }
        // Return the updated health value to the player controller
        playerController.health = health;
    }

    private void RevertPowerUp()
    {
        // Switch through current power-up type to apply the correct operation to revert
        switch (powerUpType)
        {
            case PowerUpType.speedBoost:
                playerController.movementSpeed /= speedMultiplier;
                break;
            case PowerUpType.jumpBoost:
                playerController.jumpForce /= jumpMultiplier;
                break;
            case PowerUpType.restoreHealth:
                playerController.health -= healthRestored;
                break;
            default:
                Debug.Log("Error in PowerUpController.cs : RevertPowerUp(). Power-up type '" + powerUpType + "' not recognised");
                break;
        }
    }
}
