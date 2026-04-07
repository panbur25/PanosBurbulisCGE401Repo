using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 5, -10f); // -10 Z is standard for 2D

    [Header("Bounds (match your map boundaries)")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    void LateUpdate()
    {
        if (player == null) return;

        // Desired position
        Vector3 desiredPos = player.position + offset;

        // Clamp to map boundaries
        float clampedX = Mathf.Clamp(desiredPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPos.y, minBounds.y, maxBounds.y);
        Vector3 clampedPos = new Vector3(clampedX, clampedY, offset.z);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, clampedPos, smoothSpeed);
    }
}