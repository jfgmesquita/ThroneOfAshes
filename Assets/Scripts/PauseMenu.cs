using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGUI : MonoBehaviour
{
    public AudioClip hoverSound;
    private AudioSource audioSource;
    private bool isPaused = false;
    private bool hoveredContinue = false;
    private bool hoveredExit = false;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;
    private GUIStyle infoStyle;
    private Texture2D goldTexture;
    private Texture2D darkTexture;
    public static bool IsPaused = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        goldTexture = new Texture2D(1, 1);
        goldTexture.SetPixel(0, 0, new Color(0.95f, 0.82f, 0.32f));
        goldTexture.Apply();

        darkTexture = new Texture2D(1, 1);
        darkTexture.SetPixel(0, 0, new Color(0.12f, 0.12f, 0.12f, 0.98f));
        darkTexture.Apply();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            IsPaused = isPaused;
            Time.timeScale = isPaused ? 0 : 1;

            if (isPaused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void OnGUI()
    {
        if (!isPaused) return;

        Color prevColor = GUI.color;
        GUI.color = new Color(0.07f, 0.07f, 0.07f, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = prevColor;

        float menuWidth = Screen.width * 0.5f;
        float menuHeight = Screen.height * 0.92f;
        float menuX = (Screen.width - menuWidth) / 2f;
        float menuY = (Screen.height - menuHeight) / 2f;
        Rect menuRect = new(menuX, menuY, menuWidth, menuHeight);

        float borderThickness = 6f;
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY - borderThickness, menuWidth + borderThickness * 2, borderThickness), goldTexture);
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY + menuHeight, menuWidth + borderThickness * 2, borderThickness), goldTexture);
        GUI.DrawTexture(new Rect(menuX - borderThickness, menuY, borderThickness, menuHeight), goldTexture);
        GUI.DrawTexture(new Rect(menuX + menuWidth, menuY, borderThickness, menuHeight), goldTexture);

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

        GUI.Label(new Rect(menuX, y, menuWidth, titleFontSize + 10), "Game Paused", titleStyle);

        y += titleFontSize + menuHeight * 0.06f;

        float lineHeight = 4f;
        GUI.DrawTexture(new Rect(centerX, y, buttonWidth, lineHeight), goldTexture);

        y += lineHeight + menuHeight * 0.04f;

        Rect continueBtnRect = new(centerX, y, buttonWidth, buttonHeight);
        if (GUI.Button(continueBtnRect, "Continue", buttonStyle))
        {
            isPaused = false;
            IsPaused = false;
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        HandleHover(ref hoveredContinue, continueBtnRect);

        y += buttonHeight + menuHeight * 0.03f;

        Rect exitBtnRect = new(centerX, y, buttonWidth, buttonHeight * 0.8f);
        if (GUI.Button(exitBtnRect, "Exit to Main Menu", buttonStyle))
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Main Menu");
        }
        HandleHover(ref hoveredExit, exitBtnRect);

        y += buttonHeight * 0.8f + menuHeight * 0.04f;

        GUI.DrawTexture(new Rect(centerX, y, buttonWidth, lineHeight), goldTexture);

        y += lineHeight + menuHeight * 0.04f;

        GUI.Label(
            new Rect(menuX, y, menuWidth, menuHeight * 0.3f),
            "<b>Ashen Knight Controls</b>\n\n" +
            "WASD – Move\n" +
            "Space – Jump\n" +
            "Fire1 – Swing sword\n" +
            "Fire2 – Cast fireball\n" +
            "E – Unleash the chain hook\n" +
            "ESC - Pause the journey",
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