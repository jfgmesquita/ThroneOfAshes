using UnityEngine;
using System.Collections;

public class ChainAttack : MonoBehaviour
{
    public Transform chainOrigin; 
    public LineRenderer lineRenderer; 
    public float range = 20f;
    public float pullSpeed = 10f;
    private Transform target;
    private bool pulling = false;
    public float cooldownTime = 3f;
    private bool onCooldown = false;
    public AudioClip chainSound;
    private AudioSource audioSource;
    private PlayerUI playerUI;
    private float cooldownTimer = 0f;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chainSound;
        audioSource.volume = 0.1f;
        playerUI = FindFirstObjectByType<PlayerUI>();
    }

    void Update()
    {
        if (PauseMenuGUI.IsPaused) return;
        GameOverMenu gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        Victory victoryMenu = FindFirstObjectByType<Victory>();
        if ((gameOverMenu != null && gameOverMenu.IsMenuActive()) ||
            (victoryMenu != null && victoryMenu.IsMenuActive()))
            return;

        if (Input.GetKeyDown(KeyCode.E) && !pulling && !onCooldown)
        {
            if (Physics.Raycast(chainOrigin.position, chainOrigin.forward, out RaycastHit hit, range))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (chainSound != null && audioSource != null)
                        audioSource.PlayOneShot(chainSound);

                    target = hit.collider.transform;
                    StartCoroutine(PullEnemy());
                    StartCoroutine(Cooldown());
                }
            }
        }

        if (pulling && target != null)
        {
            lineRenderer.SetPosition(0, chainOrigin.position);
            lineRenderer.SetPosition(1, target.position);
        }

        if (pulling && target != null)
        {
            lineRenderer.SetPosition(0, chainOrigin.position);
            lineRenderer.SetPosition(1, target.position);

            float length = Vector3.Distance(chainOrigin.position, target.position);
            float tilingFactor = 2f;
            Vector2 textureScale = new(length * tilingFactor, 1);

            lineRenderer.material.SetTextureScale("_MainTex", textureScale);
        }

        if (onCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (playerUI != null)
                playerUI.SetHookchainCooldown(Mathf.Min(cooldownTimer, cooldownTime), cooldownTime);

            if (cooldownTimer >= cooldownTime)
            {
                onCooldown = false;
                cooldownTimer = cooldownTime;
                if (playerUI != null)
                    playerUI.SetHookchainCooldown(cooldownTime, cooldownTime);
            }
        }
        else
        {
            if (playerUI != null)
                playerUI.SetHookchainCooldown(cooldownTime, cooldownTime);
        }
    }

    IEnumerator PullEnemy()
    {
        pulling = true;
        lineRenderer.enabled = true;
        audioSource.Play();

        while (Vector3.Distance(target.position, transform.position) > 1.5f)
        {
            target.position = Vector3.MoveTowards(
                target.position,
                transform.position,
                pullSpeed * Time.deltaTime
            );
            yield return null;
        }

        lineRenderer.enabled = false;
        pulling = false;
        target = null;
    }

    IEnumerator Cooldown()
    {
        onCooldown = true;
        cooldownTimer = 0f;
        while (cooldownTimer < cooldownTime)
        {
            yield return null;
        }
        onCooldown = false;
    }
}
