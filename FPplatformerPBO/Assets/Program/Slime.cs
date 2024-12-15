using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50; // Health maksimum
    private int currentHealth; // Health saat ini

    [Header("UI Elements")]
    public Slider healthBar; // HealthBar UI

    private Animator animator; // Animator Slime
    private bool isDead = false; // Status mati

    private void Start()
    {
        // Inisialisasi Health
        currentHealth = maxHealth;

        // Atur HealthBar jika tersedia
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Ambil komponen Animator
        animator = GetComponent<Animator>();

        // Mainkan animasi idle saat awal
        if (animator != null)
        {
            animator.Play("slime-idle");
        }
    }

    /// <summary>
    /// Dipanggil saat Slime menerima damage.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return; // Jika sudah mati, abaikan serangan

        // Kurangi health
        currentHealth -= damage;

        // Update HealthBar
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        Debug.Log(gameObject.name + " took " + damage + " damage!");

        // Mainkan animasi Hurt
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        // Cek jika health <= 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Fungsi saat Slime mati.
    /// </summary>
    private void Die()
    {
        if (isDead) return; // Jika sudah mati, abaikan
        isDead = true;

        Debug.Log(gameObject.name + " died!");

        // Mainkan animasi Die
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Matikan Collider agar tidak bisa diserang lagi
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Hancurkan GameObject setelah animasi mati selesai
        Destroy(gameObject, 2f);
    }
}