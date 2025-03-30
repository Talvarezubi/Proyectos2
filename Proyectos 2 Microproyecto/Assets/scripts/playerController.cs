using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundDistance;
    public int maxHealth = 3;
    private int currentHealth;
    public float knockbackForce = 5f; // Fuerza del retroceso
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;
    
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;

        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDistance;
                transform.position = movePos;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0, y);
        rb.linearVelocity = moveDir * speed;

        if (x != 0 && x < 0)
        {
            sr.flipX = true;
        }
        else if (x != 0 && x > 0)
        {
            sr.flipX = false;
        }

        if (Input.GetMouseButtonDown(0)) // Ataque con clic izquierdo del mouse
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector3 attackDirection = sr.flipX ? -transform.right : transform.right;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRange, attackDirection, attackRange, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("enemy"))
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage, transform.position); // Pasamos la posición del jugador como origen del ataque
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Vector3 knockbackDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            TakeDamage(1); // Ajusta el daño según lo necesites
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Jugador recibió daño. Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Jugador ha muerto.");
        // Lógica adicional para la muerte del jugador
    }
}
