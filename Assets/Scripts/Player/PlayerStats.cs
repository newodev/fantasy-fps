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
public struct HitSegment
{
    public DamageType type;
    public float value;
}
public struct HitInstance
{
    // Some info about the damage dealer
    public HitInfo info;
    // The different types of damage that make up this instance
    public List<HitSegment> segments;
}
public struct HitInfo
{
    public string AttackerName;
    public string AttackerWeapon;
}
// Describes a physical affliction applied to an entity
public struct AfflictionInstance
{

}
public enum AfflictionType
{
    Cut,
    Break,
    Puncture,
    Burn
}

public enum BodyPart
{
    Head,
    Chest,
    UpperArm,
    LowerArm,
    UpperLeg,
    LowerLeg
}

// Describes how different affliction types affect a body part
public struct SusceptibilityInfo
{
    public Dictionary<DamageType, float> Table;
}
// EXAMPLE DAMAGE CONFIG TO IMPLEMENT
/*
 * PART,    SLASH, CRUSH, PIERCE, BURN, BLEED
 * HEAD,    0.7,   1.0,   0.6,    0.8,  0.5
 * CHEST,   0.6,   0.8,   1.0,    0.6,  0.8
 * UARM,    0.3,   0.4,   0.5,    0.4,  0.6
 * LARM,    0.3,   0.3,   0.4,    0.4,  0.6
 * ULEG,    0.5,   0.5,   0.5,    0.4,  0.7
 * LLEG,    0.4,   0.4,   0.5,    0.3,  0.7
*/
public static class DamageCalculations
{
    // TODO: for online balancing, these can be adjusted to be loaded at runtime from a database on the server. (instead of readonly)
    public static readonly SusceptibilityInfo HeadInfo = ConstructSusceptibilityTable(0.7f, 1.0f, 0.6f, 0.8f);
    public static readonly SusceptibilityInfo ChestInfo = ConstructSusceptibilityTable(0.6f, 0.8f, 1.0f, 0.6f);
    // TODO: maybe complete rest... or just make a config loader
    public static SusceptibilityInfo ConstructSusceptibilityTable(float s, float c, float p, float b)
    {
        return new SusceptibilityInfo() { Table = new Dictionary<DamageType, float>() { { DamageType.Slashing, s }, { DamageType.Crushing, c }, { DamageType.Piercing, p }, { DamageType.Burning, b } } };
    }

    public static Dictionary<BodyPart, SusceptibilityInfo> InjuryTable = new Dictionary<BodyPart, SusceptibilityInfo>()
    { {BodyPart.Head, HeadInfo}, {BodyPart.Chest, ChestInfo } };
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
