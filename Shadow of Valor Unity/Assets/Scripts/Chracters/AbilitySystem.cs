using UnityEngine;

[RequireComponent(typeof(Character))]
public class AbilitySystem : MonoBehaviour
{
    [Header("Shield Rates (per second)")]
    public float shieldRegenPerSecond = 0.35f;
    public float shieldUsePerSecond = 0.58f;

    private Character owner;
    private bool shieldActive;

    private void Awake()
    {
        owner = GetComponent<Character>();
    }

    private void Update()
    {
        if (owner == null) return;

        float delta = (shieldActive ? -shieldUsePerSecond : shieldRegenPerSecond) * Time.deltaTime;
        owner.ModifyShield(delta);
    }

    public void SetShield(bool active)
    {
        shieldActive = active;
    }

    public void TryActivateSpecial()
    {
        // TODO: your special logic (cooldowns/meter/etc.)
        // For now, no-op so Character.cs compiles and runs.
    }
}
