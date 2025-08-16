using UnityEngine;

/// <summary>
/// Player character logic: movement, facing, jumping, attacks, shield, and projectiles.
/// Mirrors your Java Character class but adapted to Unity MonoBehaviour.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    // ---------------------------
    // Inspector‑tunable stats
    // ---------------------------

    [Header("Identity")]
    [SerializeField] private string characterName = "Aqua";
    [SerializeField] private Color tintColor = Color.white;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Vitals")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxShield = 100;

    [Header("Projectile (Charged)")]
    [SerializeField] private float minProjectileSpeed = 10f;
    [SerializeField] private float maxProjectileSpeed = 26f;
    [SerializeField] private float minProjectileRadius = 0.25f;
    [SerializeField] private float maxProjectileRadius = 0.60f;
    [SerializeField] private float fullChargeSeconds = 1.20f;

    [Header("Scene References")]
    [SerializeField] public Transform projectileSpawn;      // assign a child transform
    [SerializeField] public ProjectilePool projectilePool;   // drop your pool object here
    [SerializeField] public AbilitySystem abilitySystem;     // same GO as Character or child

    // ---------------------------
    // Runtime state
    // ---------------------------

    private Rigidbody2D body;
    private SpriteRenderer sr;

    private int health;
    private int shield;

    private bool facingRight = true;
    private bool grounded = true; // simple flag; replace with real ground check later

    // Charge shot state
    private bool isCharging = false;
    private float chargeTimer = 0f;

    // Convenience: cache layers
    private int layerP1;
    private int layerP2;

    // ---------------------------
    // Unity lifecycle
    // ---------------------------

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        layerP1 = LayerMask.NameToLayer("Player1");
        layerP2 = LayerMask.NameToLayer("Player2");
    }

    private void Start()
    {
        InitFromStats();
    }

    private void Update()
    {
        HandleMovement();
        HandleAttacks();
        HandleChargeTimer();
    }

    // ---------------------------
    // Initialization / Reset
    // ---------------------------

    /// <summary>
    /// Initialize vitals and visuals from inspector stats.
    /// </summary>
    public void InitFromStats()
    {
        health = maxHealth;
        shield = maxShield;

        if (sr != null)
        {
            sr.color = tintColor;
        }
    }

    // ---------------------------
    // Movement
    // ---------------------------

    private void HandleMovement()
    {
        float x = 0f;

        // Two‑player split keyboard by layer (P1: A/D/W, P2: Arrows)
        if (gameObject.layer == layerP1)
        {
            x = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);

            if (Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
        }
        else // Player2
        {
            x = (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }

        // Apply horizontal velocity
        body.linearVelocity = new Vector2(x * moveSpeed, body.linearVelocity.y);

        // Face movement direction
        if (x > 0f && !facingRight) Flip();
        if (x < 0f && facingRight) Flip();
    }

    private void Jump()
    {
        if (!grounded) return; // replace with real ground check later
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        grounded = false; // naive; set true again when you detect ground
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    // ---------------------------
    // Attacks / Charge / Specials
    // ---------------------------

    private void HandleAttacks()
    {
        // Shield (hold) – maps to your AbilityManager shield drain/regen
        bool shieldHeld =
            (gameObject.layer == layerP1 && Input.GetKey(KeyCode.I)) ||
            (gameObject.layer == layerP2 && Input.GetKey(KeyCode.Keypad9));

        if (abilitySystem != null)
        {
            abilitySystem.SetShield(shieldHeld);
        }

        // Light fire (instant)
        bool lightPressed =
            (gameObject.layer == layerP1 && Input.GetKeyDown(KeyCode.J)) ||
            (gameObject.layer == layerP2 && Input.GetKeyDown(KeyCode.Keypad1));

        if (lightPressed)
        {
            // Light tap = small, fast projectile
            LaunchProjectile(16f, 0.35f);
        }

        // Charged fire (hold to charge, release to fire)
        bool chargePressed =
            (gameObject.layer == layerP1 && Input.GetKeyDown(KeyCode.K)) ||
            (gameObject.layer == layerP2 && Input.GetKeyDown(KeyCode.Keypad2));

        bool chargeHeld =
            (gameObject.layer == layerP1 && Input.GetKey(KeyCode.K)) ||
            (gameObject.layer == layerP2 && Input.GetKey(KeyCode.Keypad2));

        bool chargeReleased =
            (gameObject.layer == layerP1 && Input.GetKeyUp(KeyCode.K)) ||
            (gameObject.layer == layerP2 && Input.GetKeyUp(KeyCode.Keypad2));

        if (chargePressed)
        {
            StartCharge();
        }
        else if (chargeHeld)
        {
            // timer advances in Update via HandleChargeTimer()
        }
        else if (chargeReleased)
        {
            FireChargedProjectile();
        }

        // Special (your active ability)
        bool specialPressed =
            (gameObject.layer == layerP1 && Input.GetKeyDown(KeyCode.L)) ||
            (gameObject.layer == layerP2 && Input.GetKeyDown(KeyCode.Keypad3));

        if (specialPressed && abilitySystem != null)
        {
            abilitySystem.TryActivateSpecial();
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0f;
    }

    private void HandleChargeTimer()
    {
        if (!isCharging) return;

        chargeTimer += Time.deltaTime;

        // Optional: clamp silently; actual clamping is in LaunchChargedProjectile()
        if (chargeTimer > fullChargeSeconds)
        {
            chargeTimer = fullChargeSeconds;
        }
    }

    private void FireChargedProjectile()
    {
        if (!isCharging) return;

        LaunchChargedProjectile(chargeTimer);
        isCharging = false;
        chargeTimer = 0f;
    }

    /// <summary>
    /// Launches a basic projectile with explicit speed & radius.
    /// </summary>
    private void LaunchProjectile(float speed, float radius)
    {
        if (projectilePool == null || projectileSpawn == null) return;

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        projectilePool.Spawn(projectileSpawn.position, dir, speed, radius, this);
    }

    /// <summary>
    /// Maps chargeSeconds → speed & radius using min/max ranges.
    /// </summary>
    public void LaunchChargedProjectile(float chargeSeconds)
    {
        float t = Mathf.Clamp01(chargeSeconds / Mathf.Max(0.01f, fullChargeSeconds));
        float speed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, t);
        float radius = Mathf.Lerp(minProjectileRadius, maxProjectileRadius, t);

        LaunchProjectile(speed, radius);
    }

    // ---------------------------
    // Health / Shield
    // ---------------------------

    /// <summary>
    /// Applies incoming damage, draining shield first, then health.
    /// </summary>
    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;

        if (shield > 0)
        {
            int used = Mathf.Min(shield, amount);
            shield -= used;
            amount -= used;
        }

        if (amount > 0)
        {
            health = Mathf.Max(0, health - amount);
            if (health == 0)
            {
                OnKO();
            }
        }
    }

    /// <summary>
    /// Modifies shield amount (used by AbilitySystem for regen/deplete).
    /// </summary>
    public void ModifyShield(float delta)
    {
        int d = Mathf.RoundToInt(delta);
        shield = Mathf.Clamp(shield + d, 0, maxShield);
    }

   private void OnKO()
{
    var rm = FindFirstObjectByType<RoundManager>();
    if (rm != null)
    {
        rm.OnKO(this);
    }
    else
    {
        Debug.LogWarning("RoundManager not found in scene.");
    }
}


    // ---------------------------
    // Accessors (useful for UI)
    // ---------------------------

    public string GetCharacterName()
    {
        return characterName;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetShield()
    {
        return shield;
    }
}
