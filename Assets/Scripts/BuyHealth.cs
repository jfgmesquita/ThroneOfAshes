using UnityEngine;

public class PedestalBuyHealth : MonoBehaviour
{
    public int healthPrice = 25;
    public int healthAmount = 35;
    public AudioClip buySound;
    public AudioClip failSound;
    private bool playerNearby = false;
    private PlayerUI playerUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Sword"))
        {
            playerUI = FindFirstObjectByType<PlayerUI>();
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    void OnGUI()
    {
        if (playerNearby && playerUI != null)
        {
            string msg = $"Press <b>B</b> to buy <color=#F2D16B>{healthAmount} health</color>\nfor <color=#F2D16B>{healthPrice} runes</color>";

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.062f),
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                normal = { textColor = Color.white }
            };

            float areaWidth = Screen.width * 0.7f;
            float areaHeight = Screen.height * 0.20f;
            float areaX = (Screen.width - areaWidth) / 2f;
            float areaY = Screen.height * 0.23f;

            GUI.Label(new Rect(areaX, areaY, areaWidth, areaHeight), msg, style);
        }
    }

    void Update()
    {
        if (playerNearby && playerUI != null && Input.GetKeyDown(KeyCode.B))
        {
            if (playerUI.GetRuneCount() >= healthPrice)
            {
                playerUI.SetRuneCount(playerUI.GetRuneCount() - healthPrice);
                playerUI.AddHealth(healthAmount);
                if (buySound != null)
                    AudioSource.PlayClipAtPoint(buySound, transform.position, 0.8f);
            }
            else
            {
                if (failSound != null)
                    AudioSource.PlayClipAtPoint(failSound, transform.position, 0.8f);
            }
        }
    }
}