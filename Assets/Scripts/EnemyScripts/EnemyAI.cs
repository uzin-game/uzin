using UnityEngine;
using Unity.Netcode;
using PlayerScripts;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyAI : NetworkBehaviour
{
    public float roamSpeed = 1.5f;
    public float attackSpeed = 5f;
    public float roamRadius = 5f;
    public float minTimeToAttack = 30f;
    public float maxTimeToAttack = 50f;
    public float damageAmount = 5f;

    private enum State
    {
        Roam,
        Attack
    }

    private State currentState = State.Roam;
    private Vector3 roamTarget;
    private Transform player;
    private Rigidbody2D rb;
    
    //Hover animation variable
    public float hoverAmplitude = 0.2f;   // height of the hover
    public float hoverFrequency = 2f; 
    private Vector3 basePosition; // Position from movement logic
    public bool IsFrozen = false;


    void Start()
    {
        if (!IsServer)
        {
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<PlayerNetwork>().transform;

        basePosition = transform.position;

        SetNextRoamTarget();
        ScheduleNextAttack();
    }

    void FixedUpdate()
    {
        if (!IsServer || IsFrozen)
        {
            return;
        }
        
        if (currentState == State.Roam)
        {
            DoRoam();
        }
        else
        {
            DoAttack();
        }
        
        // Hover effect added to base position
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        Vector3 finalPosition = basePosition + new Vector3(0, hoverOffset, 0);

        rb.MovePosition(finalPosition);
    }

    void DoRoam()
    {       
        basePosition = Vector3.MoveTowards(basePosition, roamTarget, roamSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(basePosition, roamTarget) < 0.1f)
        {
            SetNextRoamTarget();
        }
    }

    void DoAttack()
    {       
        if (player == null) return;

        basePosition = Vector3.MoveTowards(basePosition, player.position, attackSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(basePosition, player.position) < 0.5f)
        {
            currentState = State.Roam;
            SetNextRoamTarget();
            ScheduleNextAttack();
        }
    }

    void SetNextRoamTarget()
    {
        Vector2 offset = Random.insideUnitCircle * roamRadius;
        roamTarget = basePosition + (Vector3)offset;
        
    }

    void ScheduleNextAttack()
    {
        float delay = Random.Range(minTimeToAttack, maxTimeToAttack);

        Invoke(nameof(StartAttack), delay);
    }

    void StartAttack()
    {
        currentState = State.Attack;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || currentState != State.Attack)
        {
            return;
        }

        var netHealth = other.GetComponent<NetworkHealth>();

        if (netHealth != null)
        {
            netHealth.ApplyDamageServerRpc(damageAmount);
            currentState = State.Roam;
            SetNextRoamTarget();
            ScheduleNextAttack();
        }
    }
}
