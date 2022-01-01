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

    public BodyPart hitPart;
}
public struct HitInfo
{
    public string AttackerName;
    public string AttackerWeapon;
}
// Describes a physical affliction applied to an entity
public struct AfflictionInstance
{
    public AfflictionType Type;
    public float Size;
    public float Severity;
}
// TODO: this is almost uneeeded as it is a direct 1-1 from damage type
// TODO: added complexity could create a use for this. eg. some weapons create serrated cuts while dealing slash damage
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

    public static AfflictionInstance CalculateDamage(HitInstance hit)
    {
        // TODO: implement size of affliction, currently only calculates severity
        float size = 0.0f;
        float severity = 0.0f;
        AfflictionType type = AfflictionType.Cut;

        float currentHighest = 0.0f;
        foreach (HitSegment dmg in hit.segments)
        {
            // increase injury severity by the damage of each segment multiplied by the hit part's susceptibility
            severity += dmg.value * InjuryTable[hit.hitPart].Table[dmg.type];


            // The type of affliction is based on the largest damage segment in the hit
            // A sword will cut and crush but only inflicts a cut
            // TODO: this could be changed, to apply both afflictions, for example, swords can bruise
            if (dmg.value > currentHighest)
            {
                type = dmg.type switch
                {
                    DamageType.Slashing => AfflictionType.Cut,
                    DamageType.Piercing => AfflictionType.Puncture,
                    DamageType.Crushing => AfflictionType.Break,
                    DamageType.Burning => AfflictionType.Burn,
                    _ => type,
                };

                currentHighest = dmg.value;
            }
        }

        AfflictionInstance affliction = new AfflictionInstance() { Severity = severity, Size = size, Type = type };

        return affliction;
    }
}