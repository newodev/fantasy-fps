using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SusceptibilityConfigLoader
{
    // Ordered list of the damage types loaded in the SusceptibilityConfig
    public static List<DamageType> ConfigHeaderOrder = new List<DamageType>();
    public static void LoadSusceptibilityConfig(List<string> lines)
    {
        // Load the header to define the damage types
        LoadSusceptibilityConfigHeader(lines[0]);
        // Remove the header line, as it was computed in the above function call
        lines.RemoveAt(0);

        foreach (string line in lines)
        {
            // Get the body part using the first word of each line
            string firstWord = line.Substring(0, line.IndexOf(",")).ToUpper();
            BodyPart p = LoadSusceptibilityConfigPart(firstWord);

            // Create susceptibility info structure with the line data
            SusceptibilityInfo susInfo = LoadSusceptibilityConfigLine(line);

            // Add line to table
            DamageCalculations.SusceptibilityTable.Add(p, susInfo);
        }
    }

    public static BodyPart LoadSusceptibilityConfigPart(string word)
    {
        BodyPart p = word switch
        {
            "HEAD" => BodyPart.Head,
            "CHEST" => BodyPart.Chest,
            "UARM" => BodyPart.UpperArm,
            "LARM" => BodyPart.LowerArm,
            "ULEG" => BodyPart.UpperLeg,
            "LLEG" => BodyPart.LowerLeg,
            _ => BodyPart.ConfigError
        };

        if (p == BodyPart.ConfigError)
            Debug.LogError($"ConfigError: Susceptibility config row {word} doesn't match a defined BodyPart");

        return p;
    }
    public static void LoadSusceptibilityConfigHeader(string line)
    {
        List<string> columns = SplitAndTrimColumns(line);
        // Remove first column, as it defines parts
        columns.RemoveAt(0);
        foreach (string column in columns)
        {
            DamageType t = column switch
            {
                "SLASH" => DamageType.Slashing,
                "CRUSH" => DamageType.Crushing,
                "PIERCE" => DamageType.Piercing,
                "BURN" => DamageType.Burning,
                _ => DamageType.Error
            };

            if (t == DamageType.Error)
                Debug.LogError($"ConfigError: Susceptibility config header {column} doesn't match a defined DamageType");

            ConfigHeaderOrder.Add(t);
        }
    }
    public static SusceptibilityInfo LoadSusceptibilityConfigLine(string line)
    {
        List<string> columns = SplitAndTrimColumns(line);
        // Remove first column, as it defines parts, which is handled by the LoadSusceptibilityConfigTable function
        columns.RemoveAt(0);

        List<float> susceptibilityVals = new List<float>();
        foreach (string column in columns)
        {
            float columnVal = LoadSusceptibilityConfigVal(column);
            susceptibilityVals.Add(columnVal);
        }
        return ConstructSusceptibilityInfo(susceptibilityVals);
    }

    public static float LoadSusceptibilityConfigVal(string column)
    {
        float columnVal;
        if (float.TryParse(column, out columnVal))
        {
            return columnVal;
        }
        else
        {
            Debug.LogError($"ConfigError: Susceptibility config element {column} cannot be converted to float");
            return 1.0f;
        }
    }

    public static List<string> SplitAndTrimColumns(string line)
    {
        List<string> columns = new List<string>(line.Split(','));
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i] = columns[i].Trim();
        }
        return columns;
    }

    public static SusceptibilityInfo ConstructSusceptibilityInfo(List<float> vals)
    {
        Dictionary<DamageType, float> table = new Dictionary<DamageType, float>();
        for (int i = 0; i < ConfigHeaderOrder.Count; i++)
        {
            if (i - 1 > vals.Count)
            {
                Debug.LogError($"ConfigError: Incorrect number of values in susceptibility table line {vals}");
                return new SusceptibilityInfo();
            }
            table.Add(ConfigHeaderOrder[i], vals[i]);
        }
        return new SusceptibilityInfo() { Table = table };
    }
}
