using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseFunctions : MonoBehaviour
{
    public void IncreasePlayerHP(int amount)
    {
        // Assuming you have a way to access the player's stats
        // For example, if you have a GameManager or similar
        PlayerStats playerStats = FindObjectOfType<PlayerController>().playerStats;
        if (playerStats != null)
        {
            playerStats.hp += amount;
            Debug.Log($"Player HP increased by {amount}. New HP: {playerStats.hp}");
        }
    }
}