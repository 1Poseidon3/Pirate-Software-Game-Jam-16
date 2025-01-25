using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandler : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float knockbackResistance = 0.5f;
    [SerializeField] private float knockbackCooldown = 0.5f;

    private float damageInterval = 0f;
    private float damageFromPlayer;
    private float knockbackFromPlayer;
    private bool isInSwordCollider = false;
    private bool beingKnockedBack = false;
    private bool damageCoroutineRunning = false;
    private bool canBeKnockedBack = true;
    private Rigidbody rb;
    private Transform player;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        rb = GetComponent<Rigidbody>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is missing on this GameObject!");
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        if (player != null && !beingKnockedBack)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponDamage"))
        {
            if (!isInSwordCollider)
            {
                Debug.Log("Enemy entered the sword's hitbox!");

                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    damageFromPlayer = player.GetDamage();
                    damageInterval = player.GetAttackInterval();
                    knockbackFromPlayer = player.GetKnockback();

                    Vector3 knockbackDirection = (transform.position - player.transform.position).normalized;
                    knockbackDirection.y = 0;
                    StartCoroutine(ApplyKnockback(knockbackDirection, knockbackFromPlayer));
                }

                isInSwordCollider = true;

                if (!damageCoroutineRunning)
                {
                    StartCoroutine(DamageOverTime());
                }
            }
        }
        else if (other.CompareTag("PlayerHealthBase"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.DealDamageToPlayer(damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeaponDamage"))
        {
            Debug.Log("Enemy exited the sword's hitbox!");
            isInSwordCollider = false;
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    private IEnumerator DamageOverTime()
    {
        damageCoroutineRunning = true;

        while (isInSwordCollider)
        {
            TakeDamage(damageFromPlayer);

            yield return new WaitForSeconds(damageInterval);
        }

        damageCoroutineRunning = false;
    }

    private IEnumerator ApplyKnockback(Vector3 direction, float knockbackDistance)
    {
        if (!canBeKnockedBack) yield break;

        beingKnockedBack = true;
        canBeKnockedBack = false;
        navMeshAgent.enabled = false;

        float knockbackSpeed = 5f;
        float distanceMoved = 0f;

        knockbackDistance *= (1 - knockbackResistance);

        while (distanceMoved < knockbackDistance)
        {
            float moveStep = knockbackSpeed * Time.deltaTime;
            transform.position += direction * moveStep;
            distanceMoved += moveStep;
            yield return null;
        }

        navMeshAgent.enabled = true;
        beingKnockedBack = false;

        yield return new WaitForSeconds(knockbackCooldown);
        canBeKnockedBack = true;
    }


}
