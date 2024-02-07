using UnityEngine;

[System.Serializable]
public class PlayerStats : BaseStats
{
    public int strength;
    public int constitution;
    public float stamina;
    public float endurance;

    private float maxStamina = 100;
    private float staminaDrainRate = 12f; // Stamina drain per second while sprinting
    private float staminaRegenRate = 100f / 60f; // Regenerate to full in 60 seconds
    private float minStaminaToSprint = 10f; // Minimum stamina required to sprint
    private float staminaCooldown = 3.0f; // Time in seconds before stamina starts regenerating
    private float staminaThreshold = 10f; // Minimum stamina required to sprint
    private float lastTimeSprinted; // Time when the player last sprinted

    public PlayerStats(string name, int attackDamage, int hp, float speed,
                       int strength, int constitution, int stamina)
        : base(name, attackDamage, hp, speed)
    {
        this.strength = strength;
        this.constitution = constitution;
        this.stamina = stamina; // Initial stamina value
        this.endurance = 100; // Initialize endurance to full or a desired value
    }
    public bool CanSprint()
    {
        return stamina > 10; // Simplified condition for testing
    }



    public void UpdateStamina(bool isSprinting)
    {
        if (isSprinting && CanSprint())
        {
            stamina -= staminaDrainRate * Time.deltaTime; // Faster stamina consumption
            lastTimeSprinted = Time.time;
        }
        else if (Time.time - lastTimeSprinted > staminaCooldown)
        {
            stamina += staminaRegenRate * Time.deltaTime; // Regenerate stamina
        }
        stamina = Mathf.Clamp(stamina, 0, maxStamina); // Keep within bounds
    }
}
