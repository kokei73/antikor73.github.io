using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on Character.");
        }
    }

    public void PlayAnimation(string animationTrigger)
    {
        if (animator != null)
        {
            // First, reset standard triggers to avoid overlapping issues
            ResetAllTriggers();

            // Set the new trigger
            animator.SetTrigger(animationTrigger);
            Debug.Log($"Animation triggered: {animationTrigger}");
        }
    }

    private void ResetAllTriggers()
    {
        // Add all triggers defined in your Animator Controller here to reset them
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Turn");
        animator.ResetTrigger("BendOver");
        animator.ResetTrigger("LieDown");
        animator.ResetTrigger("Sit");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Wave");
        animator.ResetTrigger("TurnBack");
        animator.ResetTrigger("BendBack");
        animator.ResetTrigger("Kneel");
    }
}
