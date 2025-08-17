using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class HeroController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;

    private Animator animator;
    private SpriteRenderer sr;

    private bool isGrounded; // set this via your ground check

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // TEMP input for testing
        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;

        transform.Translate(new Vector2(x, 0f) * moveSpeed * Time.deltaTime);

        // Flip left/right
        if (Mathf.Abs(x) > 0.01f)
        {
            sr.flipX = x < 0f;
        }

        // Feed Animator parameters
        animator.SetFloat("speed", Mathf.Abs(x));
        animator.SetBool("isGrounded", isGrounded);

        // Attack tests
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("attackLight");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("attackHeavy");
        }
    }
}
