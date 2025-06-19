using System;
using UnityEngine;

public class RunePickup : MonoBehaviour
{
    public int runeValue = 10;
    public AudioClip pickRune;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var ui = FindFirstObjectByType<PlayerUI>();
            if (ui != null)
            {
                ui.SetRuneCount(ui.GetRuneCount() + runeValue);
            }
            if (pickRune != null)
            {
                AudioSource.PlayClipAtPoint(pickRune, transform.position, 0.7f);
            }
            Destroy(gameObject);
        }
    }
}
