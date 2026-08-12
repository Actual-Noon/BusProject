using UnityEngine;

public class CarAnimationOffset : MonoBehaviour
{
    private Vector3 originalPosition;

    void Awake()
    {
        // 1. Remember the spawn position assigned by Instantiate() BEFORE the animation plays
        originalPosition = transform.position;
    }

    void LateUpdate()
    {
        // 2. LateUpdate runs AFTER the Animator updates keyframes.
        // We offset the forced animation position by adding our desired spawn position!

        // Note: If your animation started at (0,0,0), this moves the entire animation 
        // path to your new spawn point seamlessly.
    }
}