using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private bool showMenu = false;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;
    private Texture2D goldTex, darkTex;
    public AudioClip hoverSound;
    private AudioSource audioSource;
    private bool hoveredReturn = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        goldTex = new Texture2D(1, 1);
        goldTex.SetPixel(0, 0, new Color(0.95f, 0.82f, 0.32f));
        goldTex.Apply();

        darkTex = new Texture2D(1, 1);
        darkTex.SetPixel(0, 0, new Color(0.12f, 0.12f, 0.12f, 0.98f));
        darkTex.Apply();
    }

    public void Show()
    {
        showMenu = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public bool IsMenuActive()
    {
        return showMenu;
    }

    void OnGUI()
    {
        if (!showMenu) return;

        float menuWidth = Screen.width * 0.5f;
        float menuHeight = Screen.height * 0.5f;
        float menuX = (Screen.width - menuWidth) / 2f;
        float menuY = (Screen.height - menuHeight) / 2f;
        Rect menuRect = new(menuX, menuY, menuWidth, menuHeight);

        // Background
        GUI.color = new Color(0.07f, 0.07f, 0.07f, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Border
        float border = 6f;
        GUI.DrawTexture(new Rect(menuX - border, menuY - border, menuWidth + border * 2, border), goldTex);
        GUI.DrawTexture(new Rect(menuX - border, menuY + menuHeight, menuWidth + border * 2, border), goldTex);
        GUI.DrawTexture(new Rect(menuX - border, menuY, border, menuHeight), goldTex);
        GUI.DrawTexture(new Rect(menuX + menuWidth, menuY, border, menuHeight), goldTex);

        // Panel
        GUI.DrawTexture(menuRect, darkTex);

        // Styles
        int titleFontSize = Mathf.RoundToInt(Screen.height * 0.09f);
        int buttonFontSize = Mathf.RoundToInt(Screen.height * 0.055f);
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        titleStyle.normal.textColor = new Color(0.95f, 0.82f, 0.32f);

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = buttonFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        buttonStyle.normal.textColor = new Color(0.95f, 0.82f, 0.32f);

        // Title
        GUI.Label(new Rect(menuX, menuY + 30, menuWidth, titleFontSize + 10), "Game Over", titleStyle);

        // Button
        float btnWidth = menuWidth * 0.6f;
        float btnHeight = menuHeight * 0.18f;
        float btnX = menuX + (menuWidth - btnWidth) / 2f;
        float btnY = menuY + menuHeight * 0.6f;
        Rect btnRect = new(btnX, btnY, btnWidth, btnHeight);
        HandleHover(ref hoveredReturn, btnRect);

        if (GUI.Button(btnRect, "Return to Menu", buttonStyle))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Main Menu");
        }
    }

    void HandleHover(ref bool wasHovered, Rect rect)
    {
        bool isHovered = rect.Contains(Event.current.mousePosition);
        if (isHovered && !wasHovered && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, 0.3f);
        }
        wasHovered = isHovered;
    }
}