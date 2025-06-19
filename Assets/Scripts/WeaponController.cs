using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject sword;
    bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    public AudioClip SwordSlash;
    public bool isAttacking = false;

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuGUI.IsPaused) return;
        GameOverMenu gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        Victory victoryMenu = FindFirstObjectByType<Victory>();
        if ((gameOverMenu != null && gameOverMenu.IsMenuActive()) ||
            (victoryMenu != null && victoryMenu.IsMenuActive()))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (CanAttack)
            {
                SwordAttack();
            }
        }
    }

    void SwordAttack()
    {
        isAttacking = true;
        CanAttack = false;
        Animator anim = sword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(SwordSlash);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }
}
