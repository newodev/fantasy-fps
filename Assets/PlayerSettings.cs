using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    private float sensitivity = 5f;
    public float Sensitivity { get => sensitivity; private set { sensitivity = Mathf.Clamp(value, 0f, 10f); } }
}
