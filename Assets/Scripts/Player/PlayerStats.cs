using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Slashing,
    Crushing,
    Piercing,
    Burning
}
// A single portion of damage dealt in an instance. An instance can have multiple types
// For example, an axe deals some slashing damage, and some crushing damage.
public struct DamageSegment
{
    public DamageType type;
    public float value;
}
public struct DamageInstance
{
    public List<DamageSegment> segments;
}
public struct DamageInfo
{
    public string AttackerName;
    public string AttackerWeapon;
}
public class PlayerStats : MonoBehaviour
{
    // Base movement speed values (units/second)
    [SerializeField]
    private float moveWalkSpeed = 4f, moveSprintSpeed = 6f, moveStrafeSpeed = 2f;
    public float MoveWalkSpeed { get => moveWalkSpeed; private set { if (value < 0) value = 0; moveWalkSpeed = value; } }
    public float MoveSprintSpeed { get => moveSprintSpeed; private set { if (value < 0) value = 0; moveSprintSpeed = value; } }
    public float MoveStrafeSpeed { get => moveStrafeSpeed; private set { if (value < 0) value = 0; moveStrafeSpeed = value; } }

    public float GetModifiedForwardMoveSpeed(float forwardInput, bool sprinting)
    {
        if (forwardInput < 0)
            return forwardInput * MoveStrafeMultiplier * MoveStrafeSpeed;
        else if (forwardInput > 0)
        {
            if (sprinting)
                return forwardInput * MoveSprintMultiplier * MoveSprintSpeed;
            else
                return forwardInput * MoveWalkMultiplier * MoveWalkSpeed;
        }
        return 0;
    }

    // Multipliers for movement speeds. Adjusted by slow effects, boots, etc
    [SerializeField]
    private float moveWalkMultiplier = 1f, moveSprintMultiplier = 1f, moveStrafeMultiplier = 1f;
    public float MoveWalkMultiplier { get => moveWalkSpeed; private set { if (value < 0) value = 0; moveWalkMultiplier = value; } }
    public float MoveStrafeMultiplier { get => moveStrafeSpeed; private set { if (value < 0) value = 0; moveStrafeMultiplier = value; } }
    public float MoveSprintMultiplier { get => moveSprintSpeed; private set { if (value < 0) value = 0; moveSprintMultiplier = value; } }

    [SerializeField]
    private float maxHP = 20, currentHP = 20;
    public float MaxHP { get => maxHP; private set { if (value < 0) value = 0; maxHP = value; } }
    public float CurrentHP { get => currentHP; private set { if (value < 0) value = 0; if (value > MaxHP) value = MaxHP; currentHP = value; } }

    [SerializeField]
    private float jumpForce = 5f;
    public float JumpForce { get => jumpForce; private set { if (value < 0) value = 0; jumpForce = value; } }

    // Modifies the player's current health based on damage value
    public void ApplyDamage(int damage)
    {
        // Damage cannot be negative, as it would heal
        if (damage < 0)
            return;

        currentHP -= damage;
    }

    // Modifies the player's current health based on damage value
    public void ApplyHeal(int heal)
    {
        // Heal cannot be negative, as it would reduce HP
        if (heal < 0)
            return;

        currentHP += heal;
    }
}
