using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Points of Interest")]
    public Transform sofaPoint;
    public Transform windowPoint;
    public Transform tvPoint;
    public Transform defaultPoint;

    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMoving)
        {
            // Set animator speed based on NavMeshAgent velocity
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);

            // Check if reached destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    isMoving = false;
                    animator.SetFloat("Speed", 0f);
                    Debug.Log("CharacterMovement: Reached destination.");
                }
            }
        }
    }

    public bool MoveToLocation(string locationName)
    {
        Transform target = null;
        string loc = locationName.ToLower();

        if (loc.Contains("диван") || loc.Contains("sofa") || loc.Contains("couch")) target = sofaPoint;
        else if (loc.Contains("окн") || loc.Contains("window")) target = windowPoint;
        else if (loc.Contains("телевизор") || loc.Contains("tv")) target = tvPoint;
        else if (loc.Contains("центр") || loc.Contains("center") || loc.Contains("обратно") || loc.Contains("ковер")) target = defaultPoint;

        if (target != null)
        {
            agent.SetDestination(target.position);
            isMoving = true;
            Debug.Log($"CharacterMovement: Moving to {target.name}");
            return true;
        }

        Debug.LogWarning($"CharacterMovement: Location '{locationName}' not found or no Transform assigned.");
        return false;
    }
}
