using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  A script to control the player character. Utilises a finite state machine
 */

// An enum containing the possible player movement states
public enum PlayerState
{
    idling,
    running,
    jumping,
    falling
}

[RequireComponent(typeof(PlayerActions))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    // Variables to hold the current state of the player and the next state to transition to
    [SerializeField] private PlayerState currentState = PlayerState.idling;
    //Fields which hold the different UI menu screens
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject winPanel;
    private PlayerState nextState = PlayerState.idling;

    // A reference to the player camera transform
    public Transform cameraTransform;
    // A reference to the player's transform
    public Transform playerTransform;
    // A reference to the player's collider
    public Collider playerCollider;
    // A reference to the player actions script holding the movement actions that can be taken by the player
    private PlayerActions actions;
    // A reference to the gunsabre controller script
    public GunSabreController gunSabreController;

    // Player health management variables
    public float maximumHealth = 100f;
    public float health = 100f;

    // Player movement parameters
    public float movementSpeed = 100f;
    public float sprintMultiplier = 2f;
    public float strafingPenalty = 0.8f;
    public float airborneMovementPenalty = 0.8f;
    public float jumpForce = 200f;
    public float jumpReleaseMultiplier = 0.6f;
    public float delayedJumpTime = 0.2f;

    // Player turning parameters
    public float lookSensitivity = 100f;
    public float maximumCameraPitchAngle = 80f;

    // Variables to hold player input
    [HideInInspector] public float horizontalInput = 0f;
    [HideInInspector] public float verticalInput = 0f;
    public bool jumpFlag = false;
    private bool attackFlag = false;
    private bool interactFlag = false;
    private bool stanceSwitchFlag = false;
    private float rotationYaw = 0f;
    private float rotationPitch = 0f;

    // Timer variables for attacking/stance switching cooldowns
    private float attackTimer = 0f;
    public float maximumAttackTimer = 1f;
    private float stanceSwitchTimer = 0f;
    public float maximumStanceSwitchTimer = 1f;

    // Variables used for check whether the player is grounded
    public float groundCheckRadius = 1f;
    public LayerMask groundMask;

    public float interactDistance = 3f;
    public LayerMask interactableLayermask;

    //Animator for player death
    public Animator Death;

    public RectTransform healthBar;
    public RectTransform chargeBar;

    public GameObject deathScreen;

    void Start()
    {
        // Lock the cursor to the screen for turning with the mouse
        Cursor.lockState = CursorLockMode.Locked;

        // Get references to various components
        playerTransform = this.transform;
        playerCollider = GetComponent<Collider>();
        actions = GetComponent<PlayerActions>();
    }

    void Update()
    {
        // Get the player inputs using the Unity Input Manager
        GetInputs();
        // Turn the player to look at the mouse
        TurnPlayer();
        // Update the FSM of the player. This FSM handles all player movement
        UpdateStates();
        // Handle the control of the gunsabre (attacking and stance switching)
        GunSabre();
        //Check for Death Condition
        Die();
        // Check for player interaction
        if (interactFlag)
        {
            interactFlag = false;
            actions.Interact();
        }
        // Update the user interface
        UpdateUI();
               
        if(Input.GetKeyDown(KeyCode.P))
        {
            pauseGame();
            Debug.Log("Paused");
        }
        if(Input.GetKeyDown(KeyCode.H))
        {
            hideAllUIPanels();
            resumeTime();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            PlayerRespawn();
        }

        
    }

    // Get the player inputs using the Unity Input Manager
    private void GetInputs()
    {
        // Movement inputs, multiplied by Time.deltaTime to standardise against frame rate changes
        horizontalInput = Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime;
        verticalInput = Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime;

        // If the jump key has been pressed
        if (Input.GetButtonDown("Jump"))
        {
            // Only allow jumping if the player is currently on the ground
            if (GroundCheck())
            {
                jumpFlag = true;
            }
            // If the player is not grounded, check again after a delay in case the player jumps while just off the ground
            else
            {
                Invoke("DelayedJumpCheck", delayedJumpTime);
            }
        }

        // If the player is jumping and releases the key early, end the jump
        if (Input.GetButtonUp("Jump") && jumpFlag)
        {
            jumpFlag = false;
        }

        // If the attack key has been pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // Only attack if the cooldown has expired
            if (attackTimer <= 0)
            {
                // Set the attack flag
                attackFlag = true;
                // Begin the cooldown timer
                attackTimer = maximumAttackTimer;
            }
        }
        // Continue the cooldown timer every cycle, regardless of input (only if the timer hasn't expired)
        if (attackTimer > 0) { attackTimer -= Time.deltaTime; }


        // If the stance switch key has been pressed
        if (Input.GetButtonDown("Fire2"))
        {
            // Only switch if the cooldown has expired
            if (stanceSwitchTimer <= 0)
            {
                // Set the attack flag
                stanceSwitchFlag = true;
                // Begin the cooldown timer
                stanceSwitchTimer = maximumStanceSwitchTimer;
            }
        }
        // Continue the cooldown timer every cycle, regardless of input (only if the timer hasn't expired)
        if (stanceSwitchTimer > 0) { stanceSwitchTimer -= Time.deltaTime; }


        // Get the change in mouse movement since last update
        rotationYaw += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        rotationPitch -= Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        if (Input.GetButtonDown("Interact"))
        {
            interactFlag = true;
        }
    }

    // Called from the main jump check after a delay. Used to allow the user to jump if they hit the button while just off the ground
    private void DelayedJumpCheck()
    {
        // Recheck for player grounding
        if (GroundCheck())
        {
            jumpFlag = true;
        }
    }

    // Rotate the player accoridng to the changes in mouse movement. The player is rotated around the y-axis (yaw), only the camera is rotated around the x-axis (pitch)
    private void TurnPlayer()
    {
        // Rotate the player around the yaw axis
        playerTransform.localRotation = Quaternion.Euler(0, rotationYaw, 0);
        // Clamp the pitch to some maximum angle
        rotationPitch = Mathf.Clamp(rotationPitch, -maximumCameraPitchAngle, maximumCameraPitchAngle);
        // Rotate the camera around the pitch axis
        cameraTransform.localRotation = Quaternion.Euler(rotationPitch, 0, 0);
    }

    // Update the FSM of the player. This FSM handles all player movement
    private void UpdateStates()
    {
        switch (currentState)
        {
            case PlayerState.idling:
                actions.Idle();
                nextState = actions.IdlingTransitions();
                break;
            case PlayerState.running:
                actions.Run();
                nextState = actions.RunningTransitions();
                break;
            case PlayerState.jumping:
                actions.Jump();
                nextState = actions.JumpingTransitions();
                break;
            case PlayerState.falling:
                actions.Fall();
                nextState = actions.FallingTransitions();
                break;
            default:
                Debug.Log("Error in PlayerController.cs: UpdateStates(). State '" + currentState + "' not recognised");
                nextState = PlayerState.idling;
                break;
        }
        // If a state transition has occurred, update the current state
        if (currentState != nextState) currentState = nextState;
    }

    // Handle the control of the gunsabre (attacking and stance switching)
    void GunSabre()
    {
        if (stanceSwitchFlag)
        {
            gunSabreController.ChangeStance();
            stanceSwitchFlag = false;
        }
        if (attackFlag)
        {
            gunSabreController.Attack();
            attackFlag = false;
        }
    }

    // Check whether the player is grounded. Done with a spherecast pointing downwards
    public bool GroundCheck()
    {
        // Perform the ground check from the player's position, but offset in y by half the height of the player's collider
        Vector3 groundCheckPosition = new Vector3(playerTransform.position.x, (playerTransform.position.y - playerCollider.bounds.extents.y), playerTransform.position.z);
        // Use the CheckSphere() function to determine whether there is ground under the player
        if (Physics.CheckSphere(groundCheckPosition, groundCheckRadius, groundMask))
        {
            // If ground is detected, return true
            return true;
        }
        // If no ground is detected return false
        return false;
    }

    // A function to apply damage to the player from external scripts
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Ouch");
    }

    // A function to kill the player if their health drops below 0 (or if a key is pressed for debugging purposes)
    public void Die()
    {

        if (health <= 0)
        {
            Debug.Log("Died");
            Death.SetTrigger("DeathTrigger");

            GameObject.Find("BossHealthCanvas").SetActive(false);
        }
    }

    // Update the player health and charge UI bars according to the value of corresponding variable
    public void UpdateUI()
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
        chargeBar.sizeDelta = new Vector2(gunSabreController.charge * 10f, chargeBar.sizeDelta.y);
    }

    // A funciton used ot pause the game
    public void pauseGame()
    {   
        //freeze time ingame
        Time.timeScale = 0f;
        //unlock cursor to the window
        Cursor.lockState = CursorLockMode.None;
        pausePanel.SetActive(true);
    }

    // A function to hide some UI elements (pause menu, etc.) when resuming the game
    public void hideAllUIPanels()
    {   
        //sets all UI canvas panels to false to hide them while playing the game
        Cursor.lockState = CursorLockMode.Locked;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Death.ResetTrigger("DeathTrigger");
        //winPanel.SetActive(false);
    }
    
    // Functions to stop and start time
    public void pauseTime()
    {   
        //freeze time ingame
        Time.timeScale = 0f;
    }
    public void resumeTime()
    {   
        //unfreeze time in game
        Time.timeScale = 1f;
        Debug.Log("resume pressed");
    }

    // A function to exit the game
    public void quitGame()
    {   
        //used for UI quit button
        Application.Quit();
    }

    public void PlayerRespawn()
    {   
        //Grabs information of the player spawn point location
        GameObject.Find("Player_Scene1").transform.position = GameObject.Find("Player_Scene1").GetComponent<CheckpointSystem>().spawnPoint;
        //give the player full health and 5 shots in the gun sabre
        GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 100;
        GameObject.Find("Player_Scene1").GetComponent<PlayerController>().gunSabreController.charge = 5;
        hideAllUIPanels();
        Death.SetTrigger("Respawn");
        Invoke("delayDeathResetTransition", 0.1f);
    }

    private void delayDeathResetTransition() {
        Death.SetTrigger("FinishedRespawning");
    }
}

