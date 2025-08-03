using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public LayerMask enemyLayer;
    public GameObject p;
    public AudioClip clip;
    public AudioSource aSource;
    private SpriteRenderer sr;
    private Animator animator;
    public float attackAnimTime = 0.25f; // How long isAttacking stays true

    void Start()
    {
        sr = p.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector2 direction;

        if (clip != null)
        {
            aSource.PlayOneShot(clip);
        }
        // Set isAttacking true
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            StopAllCoroutines();
            StartCoroutine(ResetAttackFlag());
        }
        // Determine direction player is facing
        if (sr.flipX)
        {
            direction = Vector2.left;
        }
        else
        {
            direction = Vector2.right;
        }
        Vector2 origin = (Vector2)transform.position + direction * attackRange * 0.5f;

        // Perform a box cast
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Hit: {hit.gameObject.name}");
            hit.gameObject.SetActive(false); // Or Destroy(hit.gameObject)
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(attackAnimTime);
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }

    // Optional: visualize attack zone
    private void OnDrawGizmosSelected()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + direction * attackRange * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}