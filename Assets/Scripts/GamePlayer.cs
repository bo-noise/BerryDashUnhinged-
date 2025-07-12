using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer instance;
    private readonly float spawnRate = 1f;
    private float nextSpawnTime;
    private BigInteger score;
    private BigInteger attempts;
    private BigInteger highscore;
    private BigInteger totalNormalBerries;
    private BigInteger totalPoisonBerries;
    private BigInteger totalSlowBerries;
    private BigInteger totalUltraBerries;
    private BigInteger totalSpeedyBerries;
    private BigInteger totalAttempts;
    private float boostLeft;
    private float slownessLeft;
    private float speedyLeft;
    private float screenWidth;
    private bool isGrounded;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject pausePanel;
    public Rigidbody2D rb;
    public AudioSource backgroundMusic;
    public TMP_Text fpsCounter;
    private float nextUpdate;
    private float fps;
    public SpriteRenderer overlayRender;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject jumpArrow;
    public GameObject restartButton;
    public GameObject backButton;
    public float lastMoveTime;

    void Start()
    {
        var backgroundColor = BazookaManager.Instance.GetColorSettingBackground();
        var birdColor = BazookaManager.Instance.GetColorSettingIcon();
        var overlayColor = BazookaManager.Instance.GetColorSettingOverlay();
        try
        {
            Camera.main.backgroundColor = new Color(
                int.Parse(backgroundColor[0].ToString()) / 255f,
                int.Parse(backgroundColor[1].ToString()) / 255f,
                int.Parse(backgroundColor[2].ToString()) / 255f
            );
            bird.GetComponent<SpriteRenderer>().color = new Color(
                int.Parse(birdColor[0].ToString()) / 255f,
                int.Parse(birdColor[1].ToString()) / 255f,
                int.Parse(birdColor[2].ToString()) / 255f
            );
            bird.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(
                int.Parse(overlayColor[0].ToString()) / 255f,
                int.Parse(overlayColor[1].ToString()) / 255f,
                int.Parse(overlayColor[2].ToString()) / 255f
            );
        }
        catch
        {
            Debug.LogError("Invalid BackgroundColor format");
        }

        lastMoveTime = Time.time;
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        instance = this;
        highscore = BazookaManager.Instance.GetGameStoreHighScore();
        totalNormalBerries = BazookaManager.Instance.GetGameStoreTotalNormalBerries();
        totalPoisonBerries = BazookaManager.Instance.GetGameStoreTotalPoisonBerries();
        totalSlowBerries = BazookaManager.Instance.GetGameStoreTotalSlowBerries();
        totalUltraBerries = BazookaManager.Instance.GetGameStoreTotalUltraBerries();
        totalSpeedyBerries = BazookaManager.Instance.GetGameStoreTotalSpeedyBerries();
        totalAttempts = BazookaManager.Instance.GetGameStoreTotalAttepts();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SpriteRenderer component = bird.GetComponent<SpriteRenderer>();
        int num = BazookaManager.Instance.GetBirdIcon();
        int num2 = BazookaManager.Instance.GetBirdOverlay();
        if (num == 1)
        {
            component.sprite = Tools.GetIconForUser(BazookaManager.Instance.GetAccountID() ?? 0);
        }
        else
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + num);
        }
        if (num2 == 8)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.37f, 0.32f, 0f);
        }
        else if (num2 == 11)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localScale = new UnityEngine.Vector3(1.1f, 1.1f, 1.1f); //yea i didnt feel like doing it for all lmao
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3141809f, 0.4324968f, 0f);
        }
        else if (num2 == 13)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3559977f, 0.3179995f, 0f);
        }
        else
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
        }
        if (component.sprite == null)
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_1");
            BazookaManager.Instance.SetBirdIcon(1);
        }
        if (overlayRender.sprite == null && num2 != 0)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_1");
            BazookaManager.Instance.SetBirdOverlay(1);
        }
        backgroundMusic.volume = BazookaManager.Instance.GetSettingMusicVolume();
        screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        if (Application.isMobilePlatform)
        {
            leftArrow = new("LeftArrow");
            rightArrow = new("RightArrow");
            jumpArrow = new("JumpArrow");
            restartButton = new("RestartButton");
            backButton = new("BackButton");
            leftArrow.AddComponent<SpriteRenderer>();
            rightArrow.AddComponent<SpriteRenderer>();
            jumpArrow.AddComponent<SpriteRenderer>();
            restartButton.AddComponent<SpriteRenderer>();
            backButton.AddComponent<SpriteRenderer>();

            var leftArrowSprite = leftArrow.GetComponent<SpriteRenderer>();
            var rightArrowSprite = rightArrow.GetComponent<SpriteRenderer>();
            var jumpArrowSprite = jumpArrow.GetComponent<SpriteRenderer>();
            var restartButtonSprite = restartButton.GetComponent<SpriteRenderer>();
            var backButtonSprite = backButton.GetComponent<SpriteRenderer>();

            leftArrowSprite.sprite = Resources.Load<Sprite>("Arrows/Arrow");
            leftArrowSprite.sortingOrder = 1000;
            rightArrowSprite.sprite = Resources.Load<Sprite>("Arrows/Arrow");
            rightArrowSprite.sortingOrder = 1000;
            jumpArrowSprite.sprite = Resources.Load<Sprite>("Arrows/Arrow");
            jumpArrowSprite.sortingOrder = 1000;
            restartButtonSprite.sprite = Resources.Load<Sprite>("Arrows/Restart");
            restartButtonSprite.sortingOrder = 1000;
            backButtonSprite.sprite = Resources.Load<Sprite>("Arrows/Back");
            backButtonSprite.sortingOrder = 1000;

            leftArrow.transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, 90f);
            rightArrow.transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, -90f);

            leftArrow.transform.position = new UnityEngine.Vector3(-screenWidth / 2.5f, -3.8f, 0f);
            rightArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -3.8f, 0f);
            jumpArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -1f, 0f);
            restartButton.transform.position = new UnityEngine.Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
            backButton.transform.position = new UnityEngine.Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);

            leftArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            rightArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            jumpArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            restartButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            backButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
        }
        UpdateStats(0, 1);
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doJump = false;
        bool doRestart = false;
        bool doBack = false;
        float movespeed = baseSpeed;
        if (boostLeft > 0f || speedyLeft > 0f)
        {
            movespeed = baseSpeed * 1.39f;
        }
        else if (slownessLeft > 0f)
        {
            movespeed = baseSpeed * 0.56f;
        }
        CheckIfGrounded();
        bool controllerLeft = Gamepad.current != null && (Gamepad.current.leftStick.left.isPressed || Gamepad.current.dpad.left.isPressed || Gamepad.current.rightStick.left.isPressed);
        bool controllerRight = Gamepad.current != null && (Gamepad.current.leftStick.right.isPressed || Gamepad.current.dpad.right.isPressed || Gamepad.current.rightStick.right.isPressed);
        bool controllerJump = Gamepad.current != null && (Gamepad.current.leftStick.up.isPressed || Gamepad.current.leftStick.down.isPressed || Gamepad.current.dpad.up.isPressed || Gamepad.current.dpad.down.isPressed || Gamepad.current.rightStick.up.isPressed || Gamepad.current.rightStick.down.isPressed);
        if (!Application.isMobilePlatform)
        {
            if (controllerLeft || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.jKey.isPressed)
            {
                doMoveLeft = true;
            }
            if (controllerRight || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed || Keyboard.current.lKey.isPressed)
            {
                doMoveRight = true;
            }
            if (controllerJump || Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.kKey.isPressed || Keyboard.current.iKey.isPressed || Mouse.current.leftButton.isPressed || (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed))
            {
                doJump = true;
            }
            if (Keyboard.current.rKey.isPressed)
            {
                doRestart = true;
            }
        }
        else
        {
            var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            for (int i = 0; i < touches.Count; i++)
            {
                var pos = touches[i].screenPosition;
                UnityEngine.Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, 0f));
                clickPosition.z = 0f;
                if (leftArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                {
                    doMoveLeft = true;
                }
                if (rightArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                {
                    doMoveRight = true;
                }
                if (jumpArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                {
                    doJump = true;
                }
                if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                {
                    doRestart = true;
                }
                if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                {
                    doBack = true;
                }
            }
        }
        if (doMoveLeft && !doMoveRight)
        {
            lastMoveTime = Time.time;
            bird.transform.position += new UnityEngine.Vector3(-movespeed, 0f, 0f);
            ClampPosition(bird);
            bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        }
        if (doMoveRight && !doMoveLeft)
        {
            lastMoveTime = Time.time;
            bird.transform.position += new UnityEngine.Vector3(movespeed, 0f, 0f);
            ClampPosition(bird);
            bird.transform.localScale = new UnityEngine.Vector3(-1.35f, 1.35f, 1.35f);
        }
        if (doJump && isGrounded)
        {
            lastMoveTime = Time.time;
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * BazookaManager.Instance.GetSettingSFXVolume());
            if (boostLeft > 0f || speedyLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 6f;
            }
            else
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 9f;
            }
        }
        if (doBack)
        {
            TogglePause();
        }
        if (doRestart)
        {
            if (score != 0) Respawn();
        }
    }

    void ClampPosition(GameObject obj)
    {
        var cam = Camera.main;
        var pos = obj.transform.position;
        var bounds = obj.GetComponent<Renderer>().bounds.extents;

        float zDist = Mathf.Abs(cam.transform.position.z - pos.z);

        UnityEngine.Vector3 min = cam.ViewportToWorldPoint(new UnityEngine.Vector3(0, 0, zDist));
        UnityEngine.Vector3 max = cam.ViewportToWorldPoint(new UnityEngine.Vector3(1, 1, zDist));

        pos.x = Mathf.Clamp(pos.x, min.x + bounds.x, max.x - bounds.x);
        pos.y = Mathf.Clamp(pos.y, min.y + bounds.y, max.y - bounds.y);

        obj.transform.position = pos;
    }

    void FixedUpdate()
    {
        SpawnBerries();
        if (!pausePanel.activeSelf)
        {
            MoveBird();
            if (boostLeft > 0f)
            {
                boostLeft -= Time.deltaTime;
                boostText.text = "Boost expires in " + $"{boostLeft:0.0}" + "s";
            }
            else if (slownessLeft > 0f)
            {
                slownessLeft -= Time.deltaTime;
                boostText.text = "Slowness expires in " + $"{slownessLeft:0.0}" + "s";
            }
            else if (speedyLeft > 0f)
            {
                speedyLeft -= Time.deltaTime;
                boostText.text = "Speed expires in " + $"{speedyLeft:0.0}" + "s";
            }
            else
            {
                boostText.text = "";
            }
        }
    }

    void SpawnBerries()
    {
        if (Time.time < nextSpawnTime)
        {
            return;
        }
        if (speedyLeft > 0)
        {
            nextSpawnTime = Time.time + 1f / (spawnRate * 1.875f);
        }
        else
        {
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
        float spawnProbability = Random.value;
        if (!pausePanel.activeSelf)
        {
            GameObject newBerry;
            SpriteRenderer spriteRenderer;
            if (spawnProbability <= 0.6f)
            {
                newBerry = new GameObject("Berry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/Berry");
                newBerry.tag = "Berry";
            }
            else if (spawnProbability <= 0.7f)
            {
                newBerry = new GameObject("PoisonBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PoisonBerry");
                newBerry.tag = "PoisonBerry";
            }
            else if (spawnProbability <= 0.8f)
            {
                newBerry = new GameObject("SlowBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SlowBerry");
                newBerry.tag = "SlowBerry";
            }
            else if (spawnProbability <= 0.9f)
            {
                newBerry = new GameObject("UltraBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/UltraBerry");
                newBerry.tag = "UltraBerry";
            }
            else
            {
                newBerry = new GameObject("SpeedyBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SpeedyBerry");
                newBerry.tag = "SpeedyBerry";
            }
            spriteRenderer.sortingOrder = -5;

            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float spawnPositionX = Random.Range(-screenWidth / 2.17f, screenWidth / 2.17f);
            newBerry.transform.position = new UnityEngine.Vector3(spawnPositionX, Camera.main.orthographicSize + 1f, 0f);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = new UnityEngine.Vector2(0f, -4f);
        }
    }

    void Update()
    {
        if (BazookaManager.Instance.GetSettingShowFPS() && Time.time > nextUpdate)
        {
            fps = 1f / Time.deltaTime;
            fpsCounter.text = "FPS: " + Mathf.Round(fps);
            nextUpdate = Time.time + 0.25f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(bird);
            if (Application.isMobilePlatform)
            {
                leftArrow.transform.position = new UnityEngine.Vector3(-screenWidth / 2.5f, -3.8f, 0f);
                rightArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -3.8f, 0f);
                jumpArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -1f, 0f);
                restartButton.transform.position = new UnityEngine.Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
                backButton.transform.position = new UnityEngine.Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);

                leftArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                rightArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                jumpArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                restartButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                backButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            }
        }
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");
        GameObject[] speedyberries = GameObject.FindGameObjectsWithTag("SpeedyBerry");
        if (!pausePanel.activeSelf)
        {
            if (Time.time - lastMoveTime > 20)
            {
                lastMoveTime = float.MaxValue;
                EnablePause();
            }
            CheckIfGrounded();
            GameObject[] array5 = berries;
            foreach (GameObject berry in array5)
            {
                if (berry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(berry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, berry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(berry);
                    totalNormalBerries++;
                    UpdateStats(1, 0);
                }
                if (speedyLeft > 0)
                {
                    berry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    berry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            array5 = poisonberries;
            foreach (GameObject gameObject7 in array5)
            {
                if (gameObject7.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject7);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject7.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Respawn();
                    totalPoisonBerries++;
                    UpdateStats(0, 0);
                }
                if (speedyLeft > 0)
                {
                    gameObject7.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    gameObject7.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            array5 = ultraberries;
            foreach (GameObject gameObject8 in array5)
            {
                if (gameObject8.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject8);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject8.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(gameObject8);
                    totalUltraBerries++;
                    speedyLeft = 0f;
                    if (slownessLeft > 0f)
                    {
                        slownessLeft = 0f;
                        UpdateStats(1, 0);
                    }
                    else
                    {
                        boostLeft += 10f;
                        UpdateStats(5, 0);
                    }
                }
                if (speedyLeft > 0)
                {
                    gameObject8.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    gameObject8.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            array5 = slownessberries;
            foreach (GameObject gameObject9 in array5)
            {
                if (gameObject9.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject9);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject9.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(gameObject9);
                    boostLeft = 0f;
                    slownessLeft = 10f;
                    speedyLeft = 0f;
                    totalSlowBerries++;
                    if (score > 0)
                    {
                        UpdateStats(-1, 0);
                    }
                }
                if (speedyLeft > 0)
                {
                    gameObject9.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    gameObject9.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            array5 = speedyberries;
            foreach (GameObject gameObject10 in array5)
            {
                if (gameObject10.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject10);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject10.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SpeedyPowerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(gameObject10);
                    boostLeft = 0f;
                    slownessLeft = 0f;
                    speedyLeft = 10f;
                    totalSpeedyBerries++;
                    UpdateStats(10, 0);
                }
                if (speedyLeft > 0)
                {
                    gameObject10.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    gameObject10.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
        }
        else
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = UnityEngine.Vector2.zero;
            GameObject[] array5 = berries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = poisonberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = ultraberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = slownessberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = speedyberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
        }
        if (!Application.isMobilePlatform && (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame))))
        {
            TogglePause();
        }
    }

    void Respawn()
    {
        bird.transform.position = new UnityEngine.Vector3(0f, -4.3f, 0f);
        bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        rb.gravityScale = 0f;
        rb.linearVelocity = UnityEngine.Vector2.zero;
        score = 0;
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        UpdateStats(0, 1);
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");
        GameObject[] speedyberries = GameObject.FindGameObjectsWithTag("SpeedyBerry");

        foreach (GameObject b in berries)
        {
            Destroy(b);
        }
        foreach (GameObject pb in poisonberries)
        {
            Destroy(pb);
        }
        foreach (GameObject ub in ultraberries)
        {
            Destroy(ub);
        }
        foreach (GameObject sb in slownessberries)
        {
            Destroy(sb);
        }
        foreach (GameObject syb in speedyberries)
        {
            Destroy(syb);
        }
    }

    void UpdateStats(BigInteger scoreAddAmount, BigInteger attemptAddAmount)
    {
        score += scoreAddAmount;
        totalAttempts += attemptAddAmount;
        attempts += attemptAddAmount;
        if (score > highscore)
        {
            highscore = score;
        }
        BazookaManager.Instance.SetGameStoreHighScore(highscore);
        BazookaManager.Instance.SetGameStoreTotalNormalBerries(totalNormalBerries);
        BazookaManager.Instance.SetGameStoreTotalPoisonBerries(totalPoisonBerries);
        BazookaManager.Instance.SetGameStoreTotalSlowBerries(totalSlowBerries);
        BazookaManager.Instance.SetGameStoreTotalUltraBerries(totalUltraBerries);
        BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(totalSpeedyBerries);
        BazookaManager.Instance.SetGameStoreTotalAttepts(totalAttempts);
        scoreText.text = $"Score: {Tools.FormatWithCommas(score)} \\u2022 Attempts: {Tools.FormatWithCommas(attempts)}";
        highScoreText.text = $"High Score: {Tools.FormatWithCommas(highscore)} \\u2022 Total Attempts: {Tools.FormatWithCommas(totalAttempts)}";
        if (restartButton != null) restartButton.GetComponent<Renderer>().material.color = score == 0 ? Color.gray : Color.white;
    }

    void CheckIfGrounded()
    {
        isGrounded = bird.transform.position.y <= -4.1299996f;

        rb.gravityScale = isGrounded ? 0f : 1.5f;

        if (bird.transform.position.y < -4.1359f)
        {
            bird.transform.position = new UnityEngine.Vector2(bird.transform.position.x, -4.1359f);
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, 0f);
        }
        if (jumpArrow != null && jumpArrow.GetComponent<Renderer>() != null)
        {
            jumpArrow.GetComponent<Renderer>().material.color = isGrounded ? Color.white : Color.red;
        }
    }

    public void TogglePause()
    {
        if (pausePanel.activeSelf)
        {
            DisablePause();
        }
        else
        {
            EnablePause();
        }
    }

    public void EnablePause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        backgroundMusic.Pause();
        pausePanel.SetActive(true);
    }

    public void DisablePause()
    {
        lastMoveTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.Play();
        pausePanel.SetActive(false);
        if (GamePlayerPauseMenu.Instance.editingUI == true) GamePlayerPauseMenu.Instance.ToggleEditingUI();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) EnablePause();
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}