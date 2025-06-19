using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGUI : MonoBehaviour
{
    public AudioClip hoverSound;
    private AudioSource audioSource;
    private bool hoveredStart = false;
    private bool hoveredExit = false;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;
    private GUIStyle infoStyle;
    private Texture2D goldTexture;
    private Texture2D darkTexture;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        goldTexture = new Texture2D(1, 1);
        goldTexture.SetPixel(0, 0, new Color(0.95f, 0.82f, 0.32f));
        goldTexture.Apply();

        darkTexture = new Texture2D(1, 1);
        darkTexture.SetPixel(0, 0, new Color(0.12f, 0.12f, 0.12f, 0.98f));
        darkTexture.Apply();
    }

    void OnGUI()
    {
        Color previousGUIColor = GUI.color;
        GUI.color = new Color(0.07f, 0.07f, 0.07f, 1f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previousGUIColor;

        // Menu area
        float menuWidth = Screen.width * 0.5f;
        float menuHeight = Screen.height * 0.92f;
        float menuX = (Screen.width - menuWidth) / 2f;
        float menuY = (Screen.height - menuHeight) / 2f;
        Rect menuRect = new(menuX, menuY, menuWidth, menuHeight);

        // Draw gold border
        float borderThickness = 6f;
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY - borderThickness, menuWidth + borderThickness * 2, borderThickness), goldTexture);
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY + menuHeight, menuWidth + borderThickness * 2, borderThickness), goldTexture);
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY, borderThickness, menuHeight), goldTexture);
        GUI.DrawTexture(new Rect(menuX + menuWidth, menuY, borderThickness, menuHeight), goldTexture);

        // Draw menu background
        GUI.DrawTexture(menuRect, darkTexture);

        int titleFontSize = Mathf.RoundToInt(Screen.height * 0.09f);
        int buttonFontSize = Mathf.RoundToInt(Screen.height * 0.055f);
        int infoFontSize = Mathf.RoundToInt(Screen.height * 0.025f);

        Color gold = new(0.95f, 0.82f, 0.32f);

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        titleStyle.normal.textColor = gold;

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = buttonFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        buttonStyle.normal.textColor = gold;
        buttonStyle.hover.textColor = Color.white;

        infoStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = infoFontSize,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.UpperCenter,
            wordWrap = true
        };
        infoStyle.normal.textColor = gold;

        float buttonWidth = menuWidth * 0.7f;
        float buttonHeight = menuHeight * 0.12f;
        float centerX = menuX + (menuWidth - buttonWidth) / 2f;
        float y = menuY + menuHeight * 0.10f;

        // Title
        GUI.Label(new Rect(menuX, y, menuWidth, titleFontSize + 10), "Throne of Ashes", titleStyle);

        y += titleFontSize + menuHeight * 0.06f;

        // Decorative gold line
        float lineHeight = 4f;
        GUI.DrawTexture(new Rect(centerX, y, buttonWidth, lineHeight), goldTexture);

        y += lineHeight + menuHeight * 0.04f;

        // Start Button
        Rect startBtnRect = new(centerX, y, buttonWidth, buttonHeight);
        if (GUI.Button(startBtnRect, "Begin Thy Journey", buttonStyle))
        {
            PauseMenuGUI.IsPaused = false;
            SceneManager.LoadScene("Game");
        }
        HandleHover(ref hoveredStart, startBtnRect);

        y += buttonHeight + menuHeight * 0.03f;

        // Exit Button
        Rect exitBtnRect = new(centerX, y, buttonWidth, buttonHeight * 0.8f);
        if (GUI.Button(exitBtnRect, "Leave Dungeon", buttonStyle))
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        HandleHover(ref hoveredExit, exitBtnRect);

        y += buttonHeight * 0.8f + menuHeight * 0.04f;

        // Decorative gold line
        GUI.DrawTexture(new Rect(centerX, y, buttonWidth, lineHeight), goldTexture);

        y += lineHeight + menuHeight * 0.04f;

        // Controls
        GUI.Label(
            new Rect(menuX, y, menuWidth, menuHeight * 0.3f),
            "<b>Ashen Knight Controls</b>\n\n" +
            "WASD – Move\n" +
            "Space – Jump\n" +
            "Fire1 – Swing sword\n" +
            "Fire2 – Cast fireball\n" +
            "E – Unleash the chain hook\n" +
            "ESC - Pause the journey\n",
            infoStyle
        );
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