using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "ScriptableObject/EntityData")]
public class EntityDataScriptableObject : ScriptableObject
{
    [Header("Movement Stats")]
    public float JumpHeight = 2.5f;
    public float TimeToJumpApex = 0.3f;
    public float MoveSpeed = 10f;
    public float MaxClimbAngle = 80;
    public float MaxDescendAngle = 80;
    public float AccelerationTimeAirborne = 0.2f;
    public float AccelerationTimeGrounded = 0.1f;
    public bool GravityEnabledOnStart = true;
}
