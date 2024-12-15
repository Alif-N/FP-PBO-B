using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInput : MonoBehaviour
{
    public BGMController bgmController;

    public float walkSpeed = 0.7f; // Kecepatan jalan default
    public float runSpeed = 1.5f;  // Kecepatan lari default
    public float sprintSpeed = 2.5f; // Kecepatan sprint default
    public float jumpForce = 10f; // Kekuatan lompatan default
    private float originalJumpForce; // Menyimpan jumpForce asli
    private float originalRunSpeed; // Menyimpan runSpeed asli

    public LayerMask groundLayer; // Layer untuk ground
    public Transform groundCheck; // Posisi cek ground
    public float groundCheckRadius = 0.2f; // Radius cek ground

    public AudioClip punch1; // Sound effect untuk attack 1
    public AudioClip punch2; // Sound effect untuk attack 2
    public AudioClip punch3; // Sound effect untuk attack 3
    private AudioSource audioSource; // Komponen AudioSource

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    private bool isWalking = false; // Apakah sedang berjalan
    private bool isJumping = false;
    private bool isAttacking = false; // Apakah sedang menyerang
    private int comboStep = 0; // Langkah combo attack (0 = player-att-1, 1 = player-att-2, 2 = player-att-3)
    private float comboResetTime = 1.0f; // Waktu maksimum sebelum combo di-reset
    private float lastAttackTime = 0f; // Waktu terakhir attack dilakukan

    // Power Up Variables
    public bool isPowerUpActive = false; // Apakah Power Up aktif
    private bool isCooldown = false; // Apakah sedang cooldown
    private float powerUpDuration = 100f; // Durasi Power Up dalam detik
    private float cooldownDuration = 10f; // Durasi cooldown Power Up dalam detik

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalJumpForce = jumpForce; // Simpan nilai awal jumpForce
        originalRunSpeed = runSpeed; // Simpan nilai awal runSpeed

        // Periksa jika BGMController sudah dihubungkan
        if (bgmController == null)
        {
            Debug.LogError("BGMController belum diatur! Drag GameObject BGM ke sini.");
        }
    }

    private void Update()
    {
        // Switch mode walk/run dengan CTRL
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isWalking = !isWalking; // Toggle mode
        }

        // Mengambil input dari keyboard (A dan D)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Mengatur kecepatan sesuai kondisi (Power Up atau Normal)
        float currentSpeed = isPowerUpActive ? sprintSpeed : (isWalking ? walkSpeed : runSpeed);

        // Mengatur animasi berjalan, lari, atau idle
        if (moveInput != 0 && IsGrounded() && !isAttacking)
        {
            if (isPowerUpActive)
            {
                animator.SetBool("isSprinting", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
            }
            else if (isWalking)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isSprinting", false);
            }
            else
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isSprinting", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isSprinting", false);
        }

        // Mengatur arah karakter
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Fungsi Jump (dengan GroundCheck)
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isAttacking)
        {
            Jump();
        }

        // Mengatur animasi lompat (jump) atau jatuh (fall)
        if (rb.linearVelocity.y > 0.1f) // Karakter sedang naik
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
        }
        else if (rb.linearVelocity.y < -0.1f) // Karakter sedang jatuh
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        else // Karakter di tanah
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }

        // Fungsi Attack dengan Combo
        if (Input.GetKeyDown(KeyCode.J)) // Tombol "J" untuk menyerang
        {
            if (IsGrounded() && !isAttacking)
            {
                Attack();
            }
        }

        // Reset combo jika waktu yang diberikan habis
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0; // Reset ke langkah pertama
        }

        // Aktivasi Power Up dengan tombol H
        if (Input.GetKeyDown(KeyCode.H) && !isPowerUpActive && !isCooldown)
        {
            StartCoroutine(ActivatePowerUp());
        }
    }

    private void FixedUpdate()
    {
        // Menggerakkan Player secara horizontal
        if (!isAttacking) // Tidak bergerak saat menyerang
        {
            float currentSpeed = isPowerUpActive ? sprintSpeed : (isWalking ? walkSpeed : runSpeed);
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Hentikan pergerakan horizontal saat menyerang
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    private bool IsGrounded()
    {
        // Cek apakah Player menyentuh ground
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Attack()
    {
        isAttacking = true;

        switch (comboStep)
        {
            case 0:
                animator.SetTrigger("Attack1");
                PlaySoundEffect(punch1);
                break;
            case 1:
                animator.SetTrigger("Attack2");
                PlaySoundEffect(punch2);
                break;
            case 2:
                animator.SetTrigger("Attack3");
                PlaySoundEffect(punch3);
                break;
        }

        comboStep = (comboStep + 1) % 3;
        lastAttackTime = Time.time;

        float attackDuration = 0.2f;
        Invoke("FinishAttack", attackDuration);
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
    }

    private IEnumerator ActivatePowerUp()
    {
        isPowerUpActive = true;
        runSpeed = sprintSpeed; // Defaultkan ke Sprint saat Power Up
        jumpForce = originalJumpForce * 1.4f; // Gandakan jumpForce dengan multiplier 1.4
        // Ganti BGM ke Power Up
        if (bgmController != null)
        {
            bgmController.PlayPowerUpBGM();
        }
        Debug.Log("Power Up Activated!");

        yield return new WaitForSeconds(powerUpDuration);

        isPowerUpActive = false;
        runSpeed = originalRunSpeed; // Kembalikan ke runSpeed asli
        jumpForce = originalJumpForce; // Reset jumpForce ke nilai awal
        if (bgmController != null)
        {
            bgmController.PlayDefaultBGM();
        }
        Debug.Log("Power Up Ended. Cooldown started...");

        isCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);

        isCooldown = false;
        Debug.Log("Power Up Ready!");
    }
}