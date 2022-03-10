using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerActions : MonoBehaviour
{
    // A reference to the player controller script
    private PlayerController controller;
    // A reference to the player's transform
    private Transform playerTransform;
    // A reference to the player's rigidbody
    public Rigidbody playerRigidbody;

    private void Start()
    {
        // Get references to various components
        controller = GetComponent<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = this.transform;
    }

    public void Idle()
    {

    }

    public PlayerState IdlingTransitions()
    {
        PlayerState nextState = PlayerState.idling;
        // If the player is moving, go to the running state
        if (PlayerMovementCheck())
        {
            nextState = PlayerState.running;
        }
        // If the jump flag has been set, go to the jumping state
        if (controller.jumpFlag)
        {
            nextState = PlayerState.jumping;
        }
        
        return nextState;
    }

    public void Run()
    {
        // Get a new movement vector based on the player inputs
        Vector3 movementVector = GetMovementVector(1f);

        // Update the player rigidbody with the new movement vector
        playerRigidbody.velocity = movementVector;
    }

    public PlayerState RunningTransitions()
    {
        PlayerState nextState = PlayerState.running;
        // If the player is not moving, go to the idling state
        if (!PlayerMovementCheck())
        {
            nextState = PlayerState.idling;
        }
        // If the jump flag has been set, go to the jumping state
        if (controller.jumpFlag)
        {
            nextState = PlayerState.jumping;
        }

        return nextState;
    }

    private bool hasJumped = false;
    public void Jump()
    {
        // Add force in the y-direction to jump. The hasJumped bool is used to ensure the force only gets added once per jump
        if (!hasJumped)
        {
            // Add 
            playerRigidbody.AddForce(new Vector3(0f, 2000f, 0f));
            hasJumped = true;
        }

        // Get a new movement vector based on the player inputs. Movement speed affected by a movement penalty
        Vector3 movementVector = GetMovementVector(controller.airborneMovementPenalty);

        // Update the player rigidbody with the new movement vector
        playerRigidbody.velocity = movementVector;

        // If the jumpFlag has been released early, cut the upward velocity for a shorter jump
        if (!controller.jumpFlag)
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y * controller.jumpReleaseMultiplier, playerRigidbody.velocity.z);
        }

    }

    public PlayerState JumpingTransitions()
    {
        PlayerState nextState = PlayerState.jumping;
        // If y velocity is negative, go to the falling state
        if (playerRigidbody.velocity.y < 0)
        {
            nextState = PlayerState.falling;
        }

        // Clear flags before transitioning state
        if (nextState != PlayerState.jumping)
        {
            hasJumped = false;
            controller.jumpFlag = false;
        }

        return nextState;
    }

    public void Fall()
    {
        // Get a new movement vector based on the player inputs. Movement speed affected by a movement penalty
        Vector3 movementVector = GetMovementVector(controller.airborneMovementPenalty);

        // Update the player rigidbody with the new movement vector
        playerRigidbody.velocity = movementVector;
    }

    public PlayerState FallingTransitions()
    {
        PlayerState nextState = PlayerState.falling;
        // If the player touches the ground, go to the idling state (or running state if moving)
        if (controller.GroundCheck())
        {
            if (PlayerMovementCheck()) { nextState = PlayerState.running; }
            else { nextState = PlayerState.idling; }
        }
        return nextState;
    }

    // A function to convert player inputs into a movement vector in the x and z axes
    private Vector3 GetMovementVector(float movementMultiplier)
    {
        Vector3 movementVector = Vector3.zero;

        // If holding the forward input, move the player in the newly rotated forwards direction
        if (controller.verticalInput >= 0) { movementVector = playerTransform.forward * controller.verticalInput; }
        // If holding the backward input, move backwards with speed affected by a strafing penalty
        else { movementVector = playerTransform.forward * controller.verticalInput * controller.strafingPenalty; }
        // Add on the left/right movement which is always affected by the strafing penalty
        movementVector += playerTransform.right * controller.horizontalInput * controller.strafingPenalty;

        // Scale the vector by a multiplier. Used to reduce the movement speed while airborne
        movementVector *= movementMultiplier;

        // Leave the y-component of the velocity unchanged, only move in x and z
        movementVector.y = playerRigidbody.velocity.y;

        return movementVector;
    }

    // Check whether the player is current moving in the x, z plane
    private bool PlayerMovementCheck()
    {
        // Return true if the magnitude of either movement value if non-zero
        if (Mathf.Abs(controller.horizontalInput) >= 0.1 || Mathf.Abs(controller.verticalInput) >= 0.01)
        {
            return true;
        }
        // Return false otherwise
        return false;
    }
}
