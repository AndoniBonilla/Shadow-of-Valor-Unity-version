using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public Projectile projectilePrefab;
    public int preload = 12;

    private readonly Queue<Projectile> pool = new Queue<Projectile>();

    private void Start()
    {
        for (int i = 0; i < preload; i++) CreateOne();
    }

    private Projectile CreateOne()
    {
        Projectile p = Instantiate(projectilePrefab, transform);
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
        return p;
    }

    public void Spawn(Vector2 pos, Vector2 dir, float speed, float radius, Character owner)
    {
        if (pool.Count == 0) CreateOne();
        Projectile p = pool.Dequeue();
        p.Launch(pos, dir, speed, radius, owner, this);
        p.gameObject.SetActive(true);
    }

    public void Despawn(Projectile p)
    {
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}
