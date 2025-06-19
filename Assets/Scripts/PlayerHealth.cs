using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    int currentHealth;
    [Header("Audio Clips")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    AudioSource audioSource;
    PlayerUI playerUI;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerUI = FindFirstObjectByType<PlayerUI>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (playerUI != null)
            playerUI.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        currentHealth -= amount;
        if (playerUI != null)
            playerUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (TryGetComponent<PlayerMovement>(out var playerMovement))
        {
            playerMovement.StopHeartbeat();
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            StartCoroutine(ShowGameOverAfterSound(deathSound.length));
        }
        else
        {
            ShowGameOver();
        }
    }

    IEnumerator ShowGameOverAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowGameOver();
    }

    void ShowGameOver()
    {
        var gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        if (gameOverMenu != null)
        {
            gameOverMenu.Show();
        }
    }
}
