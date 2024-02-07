using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerStats playerStats;
    public Rigidbody2D rb;
    private Vector2 movement;
    private float originalSpeed;
    public Text staminaText; // Add this line
    public Text ammoTextUI; // Assign in the inspector
    public AudioClip[] grassFootstepSounds;
    public AudioClip[] concreteFootstepSounds;
    public AudioClip[] woodFootstepSounds;
    public AudioSource footstepSource; // AudioSource for footsteps
    private float stepRate = 0.5f; // Time between footstep sounds
    private float stepCoolDown; // Internal timer for footsteps
    private GunShooting currentGun;

    void Start()
    {
         currentGun = GetComponentInChildren<GunShooting>();
        
        if (currentGun != null)
        {
            currentGun.SetAmmoTextUI(ammoTextUI);
        }
        playerStats = new PlayerStats("Hero", playerStats.hp, playerStats.attackDamage, playerStats.speed, 15, 10, 100);
        originalSpeed = playerStats.speed; // Ensure this is a valid speed value
        Debug.Log("Original Speed: " + originalSpeed);
        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Handle player movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();
        HandleFootsteps();
        // Handle sprinting and stamina
        HandleSprinting();

        if (Input.GetKeyDown(KeyCode.L))
        {
            LogPlayerStats();
        }

        // Update UI
        UpdateStaminaUI(playerStats.stamina);
    }
    private void HandleFootsteps()
    {
        if (movement.magnitude > 0 && stepCoolDown <= 0f)
        {
            footstepSource.clip = GetFootstepSound();
            footstepSource.Play();
            stepCoolDown = stepRate;
        }

        if (stepCoolDown > 0)
        {
            stepCoolDown -= Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        // Move the player
        rb.velocity = movement * playerStats.speed; // Use playerStats.speed for movement
    }
    private AudioClip GetFootstepSound()
    {
        // LayerMask to ignore the player's layer
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask;

        // Cast a ray straight down from the player's position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layerMask);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.tag); // Should log the tag of the object hit, excluding the player
            switch (hit.collider.gameObject.tag)
            {
                case "Grass":
                    return grassFootstepSounds[Random.Range(0, grassFootstepSounds.Length)];
                case "Concrete":
                    return concreteFootstepSounds[Random.Range(0, concreteFootstepSounds.Length)];
                // More cases as needed
                case "Wood":
                    return woodFootstepSounds[Random.Range(0, concreteFootstepSounds.Length)];
                // More cases as needed
                default:
                    return woodFootstepSounds[Random.Range(0, grassFootstepSounds.Length)]; // Default sound
            }
        }
        return null;
    }

    private void HandleSprinting()
    {
        bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift);
        bool canSprint = playerStats.CanSprint();

        if (isTryingToSprint && canSprint)
        {
            playerStats.UpdateStamina(true);
            playerStats.speed = originalSpeed * 1.5f;
        }
        else
        {
            playerStats.UpdateStamina(false);
            playerStats.speed = originalSpeed;
        }
    }



    private void LogPlayerStats()
    {
        string statsMessage = $"Hello, my name is {playerStats.name}. " +
                              $"My stats are: Attack Damage: {playerStats.attackDamage}, " +
                              $"HP: {playerStats.hp}, Speed: {playerStats.speed}, " +
                              $"Strength: {playerStats.strength}, Constitution: {playerStats.constitution}, " +
                              $"Stamina: {playerStats.stamina}, Endurance: {playerStats.endurance}.";

        Debug.Log(statsMessage);
    }

    private void UpdateStaminaUI(float currentStamina)
    {
        float maxStamina = 100f; // Replace with your actual max stamina value
        staminaText.text = $"Stamina: {currentStamina:0} / {maxStamina}"; // Format as string with no decimal places
    }
}
