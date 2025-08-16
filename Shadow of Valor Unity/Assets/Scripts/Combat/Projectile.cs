using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float life;
    private const float MaxLife = 4f;

    private CircleCollider2D col;
    private SpriteRenderer sr;
    private ProjectilePool pool;
    private Character owner;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        sr = GetComponent<SpriteRenderer>();
    }

    public void Launch(Vector2 start, Vector2 dir, float spd, float radius, Character src, ProjectilePool p)
    {
        transform.position = start;
        direction = dir.normalized;
        speed = spd;
        life = 0f;
        owner = src;
        pool = p;

        col.radius = radius;

        gameObject.layer = (owner.gameObject.layer == LayerMask.NameToLayer("Player1"))
            ? LayerMask.NameToLayer("Proj_P1")
            : LayerMask.NameToLayer("Proj_P2");
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        life += Time.deltaTime;
        if (life > MaxLife) pool.Despawn(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var target = other.GetComponentInParent<Character>();
        if (target != null && target != owner)
        {
            // Later: target.ApplyDamage(10);
            pool.Despawn(this);
        }
    }
}
