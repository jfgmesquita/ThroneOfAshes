using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireForce = 700.0f;
    public float magicCost = 5f;
    private PlayerUI playerUI;

    void Start()
    {
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

        if (Input.GetButtonDown("Fire2") && playerUI != null && playerUI.magic >= magicCost)
        {
            ShootFireball();
            playerUI.SetMagic(playerUI.magic - magicCost, playerUI.maxMagic);
        }
    }

    void ShootFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        if (fireball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(firePoint.forward * fireForce);
        }
    }
}
