using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    public float detectionRange = 10f;
    public int maxHealth = 3;
    private int currentHealth;
    public float knockbackForce = 5f;
    private Transform player;
    private bool isPaused = false;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player != null && !isPaused)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StunPlayer(other.gameObject));
        }
    }

    private IEnumerator StunPlayer(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false; // Desactiva el control del jugador
            yield return new WaitForSeconds(1f);
            playerController.enabled = true; // Reactiva el control del jugador
        }
    }

    public void TakeDamage(int damage, Vector3 attackSource)
    {
        currentHealth -= damage;
        Debug.Log("Enemigo recibió daño. Vida actual: " + currentHealth);

        Vector3 knockbackDirection = (transform.position - attackSource).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemigo ha muerto.");
        Destroy(gameObject);
    }
}
