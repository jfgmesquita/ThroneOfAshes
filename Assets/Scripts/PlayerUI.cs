using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    float health = 100f;
    float maxHealth = 100f;
    public float magic = 50f;
    public float maxMagic = 50f;
    public float magicRegenRate = 1f;
    float hookchainCooldown = 0f;
    float hookchainMaxCooldown = 3f;
    int runeCount = 0;
    public float intoxication = 0f;
    public float maxIntoxication = 120f; // 2 min
    public int liberatedSouls = 0;
    public int totalEnemies = 10;
    private Texture2D redTex, blueTex, grayTex, goldTex, brownTex, darkTex, greenTex;
    public AudioClip deathSound;
    AudioSource audioSource;
    public GameOverMenu gameOverMenu;
    public Victory victoryMenu;
    public Texture2D vignetteTexture;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        redTex = MakeTex(Color.red);
        blueTex = MakeTex(new Color(0.2f, 0.4f, 1f)); // deep blue
        grayTex = MakeTex(new Color(0.2f, 0.2f, 0.2f)); // dark gray
        goldTex = MakeTex(new Color(0.7f, 0.5f, 0.1f)); // gold
        brownTex = MakeTex(new Color(0.3f, 0.15f, 0.05f)); // wood brown
        darkTex = MakeTex(new Color(0.1f, 0.05f, 0.01f)); // dark border
        greenTex = MakeTex(new Color(0.18f, 0.35f, 0.13f)); // green

        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    Texture2D MakeTex(Color col)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, col);
        t.Apply();
        return t;
    }

    void Update()
    {
        if (magic < maxMagic)
        {
            magic += magicRegenRate * Time.deltaTime;
            if (magic > maxMagic)
                magic = maxMagic;
        }

        if ((victoryMenu != null && victoryMenu.IsMenuActive()) || (gameOverMenu != null && gameOverMenu.IsMenuActive()))
        {
            return;
        }

        if (liberatedSouls >= totalEnemies && intoxication < maxIntoxication)
        {
            ShowVictory();
            return;
        }

        if (liberatedSouls < totalEnemies)
        {
            intoxication += Time.deltaTime;
            if (intoxication >= maxIntoxication)
            {
                intoxication = maxIntoxication;
                ShowGameOver();
            }
        }
    }

    public void AddHealth(int amount)
    {
        SetHealth(health + amount, maxHealth);
    }

    public void SetHealth(float value, float max)
    {
        health = Mathf.Clamp(value, 0, max);
        maxHealth = max;
    }

    public void SetMagic(float value, float max)
    {
        magic = Mathf.Clamp(value, 0, max);
        maxMagic = max;
    }

    public void SetHookchainCooldown(float value, float max)
    {
        hookchainCooldown = value;
        hookchainMaxCooldown = max;
    }

    public void SetRuneCount(int count)
    {
        runeCount = count;
    }

    public int GetRuneCount()
    {
        return runeCount;
    }

    public void AddLiberatedSoul()
    {
        liberatedSouls++;
    }

    public void ShowGameOver()
    {
        var playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.StopHeartbeat();

        if (gameOverMenu != null && !gameOverMenu.IsMenuActive())
        {
            audioSource?.PlayOneShot(deathSound);
            gameOverMenu.Show();
            Time.timeScale = 0f;
        }
    }

    public void ShowVictory()
    {
        if (victoryMenu != null && !victoryMenu.IsMenuActive())
        {
            victoryMenu.Show();
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        // Vignette
        float intoxThreshold = maxIntoxication * 0.7f;
        float intoxPercent = Mathf.Clamp01(intoxication / intoxThreshold);

        float vignetteAlpha = Mathf.Lerp(0.2f, 0.7f, intoxPercent);

        GUI.color = new Color(0, 0, 0, vignetteAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), vignetteTexture);
        GUI.color = Color.white;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float barWidth = screenWidth * 0.22f;
        float barHeight = screenHeight * 0.035f;
        float padding = screenHeight * 0.015f;
        float barSpacing = barHeight * 1.2f;

        // Intoxication Bar (top center)
        float intoxBarWidth = screenWidth * 0.35f;
        float intoxBarHeight = screenHeight * 0.045f;
        float intoxBarX = (screenWidth - intoxBarWidth) / 2f;
        float intoxBarY = padding * 0.5f;
        Rect intoxRect = new(intoxBarX, intoxBarY, intoxBarWidth, intoxBarHeight);

        GUI.DrawTexture(new Rect(intoxRect.x - 3, intoxRect.y - 3, intoxRect.width + 6, intoxRect.height + 6), goldTex);
        GUI.DrawTexture(intoxRect, greenTex);
        float intoxFill = Mathf.Clamp01(intoxication / maxIntoxication);
        if (intoxFill > 0f)
        {
            GUI.DrawTexture(
                new Rect(intoxRect.x, intoxRect.y, intoxRect.width * intoxFill, intoxRect.height),
                darkTex
            );
        }

        var intoxStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(intoxBarHeight * 0.7f),
            normal = { textColor = new Color(0.95f, 0.85f, 0.6f) }
        };
        GUI.Label(intoxRect, "Intoxication", intoxStyle);

        // Liberated Souls (top right)
        float soulsWidth = screenWidth * 0.18f;
        float soulsHeight = screenHeight * 0.045f;
        float soulsX = screenWidth - soulsWidth - padding;
        float soulsY = intoxBarY;
        Rect soulsRect = new(soulsX, soulsY, soulsWidth, soulsHeight);

        GUI.DrawTexture(new Rect(soulsRect.x - 3, soulsRect.y - 3, soulsRect.width + 6, soulsRect.height + 6), darkTex);
        GUI.DrawTexture(new Rect(soulsRect.x - 2, soulsRect.y - 2, soulsRect.width + 4, soulsRect.height + 4), goldTex);
        GUI.DrawTexture(soulsRect, brownTex);

        var soulStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(soulsHeight * 0.7f),
            normal = { textColor = new Color(0.95f, 0.82f, 0.32f) }
        };
        GUI.Label(soulsRect, $"Souls: {liberatedSouls}/{totalEnemies}", soulStyle);

        // Health Bar
        Rect healthRect = new(
            padding,
            screenHeight - barHeight * 2 - padding * 2 - barSpacing,
            barWidth,
            barHeight
        );
        DrawMedievalBar(healthRect, health / maxHealth, redTex, "Health", goldTex, darkTex);

        // Mana Bar
        Rect manaRect = new(
            padding,
            screenHeight - barHeight - padding - barSpacing / 2,
            barWidth,
            barHeight
        );
        DrawMedievalBar(manaRect, magic / maxMagic, blueTex, "Mana", brownTex, darkTex);

        // Chain Cooldown Bar
        Rect chainRect = new(
            screenWidth - barWidth - padding,
            screenHeight - barHeight - padding,
            barWidth,
            barHeight
        );
        float fill = Mathf.Clamp01(hookchainCooldown / hookchainMaxCooldown);
        DrawChainCooldownBar(chainRect, fill, hookchainMaxCooldown - hookchainCooldown);

        // Rune Count
        float runeBarWidth = screenWidth * 0.13f;
        float runeBarHeight = screenHeight * 0.045f;
        float runePadding = padding * 1.5f;
        Rect runeBarRect = new(
            screenWidth - runeBarWidth - runePadding,
            runePadding + soulsHeight + padding * 0.5f,
            runeBarWidth,
            runeBarHeight
        );
        DrawMedievalBar(runeBarRect, 1f, goldTex, $"Runes: {runeCount}", goldTex, darkTex);
    }

    void DrawMedievalBar(Rect rect, float fill, Texture2D fillTex, string label, Texture2D borderTex, Texture2D shadowTex)
    {
        GUI.DrawTexture(new Rect(rect.x - 3, rect.y - 3, rect.width + 6, rect.height + 6), shadowTex);
        GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), borderTex);
        GUI.DrawTexture(rect, grayTex);
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * fill, rect.height), fillTex);

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(rect.height * 0.7f),
            normal = { textColor = new Color(0.95f, 0.85f, 0.6f) }
        };
        GUI.Label(rect, label, style);
    }

    void DrawChainCooldownBar(Rect rect, float fill, float secondsLeft)
    {
        fill = Mathf.Clamp01(fill);

        GUI.DrawTexture(new Rect(rect.x - 3, rect.y - 3, rect.width + 6, rect.height + 6), darkTex);
        GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), goldTex);
        GUI.DrawTexture(rect, grayTex);
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * fill, rect.height), blueTex);

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = Mathf.RoundToInt(rect.height * 0.7f),
            richText = true,
            normal = { textColor = fill < 1f ? new Color(0.7f, 0.85f, 1f) : new Color(1f, 0.95f, 0.6f) }
        };
        string text = fill >= 1f
            ? "<b><color=#FFD700>Chain Ready!</color></b>"
            : $"<b>Wait {Mathf.CeilToInt(secondsLeft)}s...</b>";
        GUI.Label(rect, text, style);
    }
}
