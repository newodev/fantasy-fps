using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Base movement speed value. In units/second
    [SerializeField]
    private float moveSpeed = 5f;
    // Multiplier for the base movement speed.
    private float moveSpeedMultiplier = 1f;

    public float GetMovementSpeed()
    {
        return moveSpeed * moveSpeedMultiplier;
    }
    
    // 
    [SerializeField]
    private int maxHP = 20;
    [SerializeField]
    private int currentHP = 20;

    public int GetMaxHP()
    {
        return maxHP;
    }
    public int GetCurrentHP()
    {
        return currentHP;
    }
    public void ChangeMaxHP(int val)
    {
        maxHP += val;
    }

    // Modifies the player's current health based on damage value
    public void ApplyDamage(int damage)
    {
        // Damage cannot be negative, as it would heal
        if (damage < 0)
            return;

        currentHP -= damage;
        // Ensure HP can not go below 0
        if (currentHP < 0)
            currentHP = 0;
    }

    // Modifies the player's current health based on damage value
    public void ApplyHeal(int heal)
    {
        // Heal cannot be negative, as it would reduce HP
        if (heal < 0)
            return;

        currentHP += heal;
        // Ensure HP cannot exceed max
        if (currentHP > maxHP)
            currentHP = maxHP;
    }
}
