using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject customMenu;
    [SerializeField] private Button customButton;
    [SerializeField] private Button customBackButton;
    [SerializeField] private Button customPlayButton;
    [SerializeField] private Button customNormalizeButton;

    [SerializeField] private TMP_InputField normalBerryChance;
    [SerializeField] private TMP_InputField poisonBerryChance;
    [SerializeField] private TMP_InputField slowBerryChance;
    [SerializeField] private TMP_InputField ultraBerryChance;
    [SerializeField] private TMP_InputField speedyBerryChance;
    [SerializeField] private TMP_InputField randomBerryChance;
    [SerializeField] private TMP_InputField antiBerryChance;
    [SerializeField] private TMP_InputField nothingBerryChance;

    [SerializeField] private TMP_Text validateTotalText;

    private readonly float defaultNormalBerryChance = 47.5f;
    private readonly float defaultPoisonBerryChance = 12.5f;
    private readonly float defaultSlowBerryChance = 10f;
    private readonly float defaultUltraBerryChance = 10f;
    private readonly float defaultSpeedyBerryChance = 10f;
    private readonly float defaultRandomBerryChance = 5f;
    private readonly float defaultAntiBerryChance = 5f;
    private readonly float defaultNothingBerryChance = 0f;

    void Awake()
    {
        customButton.onClick.AddListener(() =>
        {
            selectionMenu.SetActive(false);
            customMenu.SetActive(true);
        });
        customBackButton.onClick.AddListener(() =>
        {
            customMenu.SetActive(false);
            selectionMenu.SetActive(true);

            normalBerryChance.text = defaultNormalBerryChance.ToString();
            poisonBerryChance.text = defaultPoisonBerryChance.ToString();
            slowBerryChance.text = defaultSlowBerryChance.ToString();
            ultraBerryChance.text = defaultUltraBerryChance.ToString();
            speedyBerryChance.text = defaultSpeedyBerryChance.ToString();
            randomBerryChance.text = defaultRandomBerryChance.ToString();
            antiBerryChance.text = defaultAntiBerryChance.ToString();
            nothingBerryChance.text = defaultNothingBerryChance.ToString();
            ValidateTotal();
        });
        customNormalizeButton.onClick.AddListener(() =>
        {
            float normalBerry = GetValueFrom(normalBerryChance);
            float poisonBerry = GetValueFrom(poisonBerryChance);
            float slowBerry = GetValueFrom(slowBerryChance);
            float ultraBerry = GetValueFrom(ultraBerryChance);
            float speedyBerry = GetValueFrom(speedyBerryChance);
            float randomBerry = GetValueFrom(randomBerryChance);
            float antiBerry = GetValueFrom(antiBerryChance);
            float nothingBerry = GetValueFrom(nothingBerryChance);

            int divideBy = 0;

            divideBy += normalBerry > 0 ? 1 : 0;
            divideBy += poisonBerry > 0 ? 1 : 0;
            divideBy += slowBerry > 0 ? 1 : 0;
            divideBy += ultraBerry > 0 ? 1 : 0;
            divideBy += speedyBerry > 0 ? 1 : 0;
            divideBy += randomBerry > 0 ? 1 : 0;
            divideBy += antiBerry > 0 ? 1 : 0;
            divideBy += nothingBerry > 0 ? 1 : 0;

            float addedChances = normalBerry + poisonBerry + slowBerry + ultraBerry + speedyBerry + randomBerry + antiBerry + nothingBerry;
            float difference = addedChances - 100f;

            if (normalBerry > 0)
            {
                normalBerryChance.text = (normalBerry - (difference / divideBy)).ToString() + "%";
                normalBerryChance.stringPosition = normalBerryChance.text.Length;
            }

            if (poisonBerry > 0)
            {
                poisonBerryChance.text = (poisonBerry - (difference / divideBy)).ToString() + "%";
                poisonBerryChance.stringPosition = poisonBerryChance.text.Length;
            }

            if (slowBerry > 0)
            {
                slowBerryChance.text = (slowBerry - (difference / divideBy)).ToString() + "%";
                slowBerryChance.stringPosition = slowBerryChance.text.Length;
            }

            if (ultraBerry > 0)
            {
                ultraBerryChance.text = (ultraBerry - (difference / divideBy)).ToString() + "%";
                ultraBerryChance.stringPosition = ultraBerryChance.text.Length;
            }

            if (speedyBerry > 0)
            {
                speedyBerryChance.text = (speedyBerry - (difference / divideBy)).ToString() + "%";
                speedyBerryChance.stringPosition = speedyBerryChance.text.Length;
            }

            if (randomBerry > 0)
            {
                randomBerryChance.text = (randomBerry - (difference / divideBy)).ToString() + "%";
                randomBerryChance.stringPosition = randomBerryChance.text.Length;
            }

            if (antiBerry > 0)
            {
                antiBerryChance.text = (antiBerry - (difference / divideBy)).ToString() + "%";
                antiBerryChance.stringPosition = antiBerryChance.text.Length;
            }

            if (nothingBerry > 0)
            {
                nothingBerryChance.text = (nothingBerry - (difference / divideBy)).ToString() + "%";
                nothingBerryChance.stringPosition = nothingBerryChance.text.Length;
            }

            normalBerry = GetValueFrom(normalBerryChance);
            poisonBerry = GetValueFrom(poisonBerryChance);
            slowBerry = GetValueFrom(slowBerryChance);
            ultraBerry = GetValueFrom(ultraBerryChance);
            speedyBerry = GetValueFrom(speedyBerryChance);
            randomBerry = GetValueFrom(randomBerryChance);
            antiBerry = GetValueFrom(antiBerryChance);
            nothingBerry = GetValueFrom(nothingBerryChance);

            if (normalBerry > 0)
            {
                normalBerry = (float)Math.Floor(normalBerry);
            }

            if (poisonBerry > 0)
            {
                poisonBerry = (float)Math.Floor(poisonBerry);
            }

            if (slowBerry > 0)
            {
                slowBerry = (float)Math.Floor(slowBerry);
            }

            if (ultraBerry > 0)
            {
                ultraBerry = (float)Math.Floor(ultraBerry);
            }

            if (speedyBerry > 0)
            {
                speedyBerry = (float)Math.Floor(speedyBerry);
            }

            if (randomBerry > 0)
            {
                randomBerry = (float)Math.Floor(randomBerry);
            }

            if (antiBerry > 0)
            {
                antiBerry = (float)Math.Floor(antiBerry);
            }

            if (nothingBerry > 0)
            {
                nothingBerry = (float)Math.Floor(nothingBerry);
            }

            float addedChances2 = normalBerry + poisonBerry + slowBerry + ultraBerry + speedyBerry + randomBerry + antiBerry + nothingBerry;
            float difference2 = addedChances2 - 100f;
            bool fixedValues = false;

            Debug.Log(difference2);
            Debug.Log(normalBerry - difference2);
            Debug.Log(poisonBerry - difference2);

            if (normalBerry > 0)
            {
                if (!fixedValues)
                {
                    normalBerryChance.text = (normalBerry - difference2).ToString() + "%";
                    normalBerryChance.stringPosition = normalBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    normalBerryChance.text = normalBerry.ToString() + "%";
                    normalBerryChance.stringPosition = normalBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (poisonBerry > 0)
            {
                if (!fixedValues)
                {
                    poisonBerryChance.text = (poisonBerry - difference2).ToString() + "%";
                    poisonBerryChance.stringPosition = poisonBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    poisonBerryChance.text = poisonBerry.ToString() + "%";
                    poisonBerryChance.stringPosition = poisonBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (slowBerry > 0)
            {
                if (!fixedValues)
                {
                    slowBerryChance.text = (slowBerry - difference2).ToString() + "%";
                    slowBerryChance.stringPosition = slowBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    slowBerryChance.text = slowBerry.ToString() + "%";
                    slowBerryChance.stringPosition = slowBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (ultraBerry > 0)
            {
                if (!fixedValues)
                {
                    ultraBerryChance.text = (ultraBerry - difference2).ToString() + "%";
                    ultraBerryChance.stringPosition = ultraBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    ultraBerryChance.text = ultraBerry.ToString() + "%";
                    ultraBerryChance.stringPosition = ultraBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (speedyBerry > 0)
            {
                if (!fixedValues)
                {
                    speedyBerryChance.text = (speedyBerry - difference2).ToString() + "%";
                    speedyBerryChance.stringPosition = speedyBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    speedyBerryChance.text = speedyBerry.ToString() + "%";
                    speedyBerryChance.stringPosition = speedyBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (randomBerry > 0)
            {
                if (!fixedValues)
                {
                    randomBerryChance.text = (randomBerry - difference2).ToString() + "%";
                    randomBerryChance.stringPosition = randomBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    randomBerryChance.text = randomBerry.ToString() + "%";
                    randomBerryChance.stringPosition = randomBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (antiBerry > 0)
            {
                if (!fixedValues)
                {
                    antiBerryChance.text = (antiBerry - difference2).ToString() + "%";
                    antiBerryChance.stringPosition = antiBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    antiBerryChance.text = antiBerry.ToString() + "%";
                    antiBerryChance.stringPosition = antiBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            if (nothingBerry > 0)
            {
                if (!fixedValues)
                {
                    nothingBerryChance.text = (nothingBerry - difference2).ToString() + "%";
                    nothingBerryChance.stringPosition = nothingBerryChance.text.Length;
                    fixedValues = true;
                }
                else
                {
                    nothingBerryChance.text = nothingBerry.ToString() + "%";
                    nothingBerryChance.stringPosition = nothingBerryChance.text.Length;
                    fixedValues = true;
                }
            }

            ValidateTotal();
        });
        customPlayButton.onClick.AddListener(async () =>
        {
            GameObject obj = new("CustomGameTempData");
            obj.AddComponent<CustomGameTempData>();
            CustomGameTempData customGameTempData = obj.GetComponent<CustomGameTempData>();
            customGameTempData.normalBerryChance = GetValueFrom(normalBerryChance);
            customGameTempData.poisonBerryChance = GetValueFrom(poisonBerryChance);
            customGameTempData.slowBerryChance = GetValueFrom(slowBerryChance);
            customGameTempData.ultraBerryChance = GetValueFrom(ultraBerryChance);
            customGameTempData.speedyBerryChance = GetValueFrom(speedyBerryChance);
            customGameTempData.randomBerryChance = GetValueFrom(randomBerryChance);
            customGameTempData.antiBerryChance = GetValueFrom(antiBerryChance);
            customGameTempData.nothingBerryChance = GetValueFrom(nothingBerryChance);
            await SceneManager.LoadSceneAsync("CustomGamePlayer");
        });

        normalBerryChance.onSelect.AddListener((value) => OnSelect(value, normalBerryChance));
        normalBerryChance.onDeselect.AddListener((value) => OnDeselect(value, normalBerryChance));
        poisonBerryChance.onSelect.AddListener((value) => OnSelect(value, poisonBerryChance));
        poisonBerryChance.onDeselect.AddListener((value) => OnDeselect(value, poisonBerryChance));
        slowBerryChance.onSelect.AddListener((value) => OnSelect(value, slowBerryChance));
        slowBerryChance.onDeselect.AddListener((value) => OnDeselect(value, slowBerryChance));
        ultraBerryChance.onSelect.AddListener((value) => OnSelect(value, ultraBerryChance));
        ultraBerryChance.onDeselect.AddListener((value) => OnDeselect(value, ultraBerryChance));
        speedyBerryChance.onSelect.AddListener((value) => OnSelect(value, speedyBerryChance));
        speedyBerryChance.onDeselect.AddListener((value) => OnDeselect(value, speedyBerryChance));
        randomBerryChance.onSelect.AddListener((value) => OnSelect(value, randomBerryChance));
        randomBerryChance.onDeselect.AddListener((value) => OnDeselect(value, randomBerryChance));
        antiBerryChance.onSelect.AddListener((value) => OnSelect(value, antiBerryChance));
        antiBerryChance.onDeselect.AddListener((value) => OnDeselect(value, antiBerryChance));
        nothingBerryChance.onSelect.AddListener((value) => OnSelect(value, nothingBerryChance));
        nothingBerryChance.onDeselect.AddListener((value) => OnDeselect(value, nothingBerryChance));
    }

    void ValidateTotal()
    {
        customBackButton.interactable = false;
        customPlayButton.interactable = false;
        customNormalizeButton.interactable = false;
        float total = 0f;
        total += GetValueFrom(normalBerryChance);
        total += GetValueFrom(poisonBerryChance);
        total += GetValueFrom(slowBerryChance);
        total += GetValueFrom(ultraBerryChance);
        total += GetValueFrom(speedyBerryChance);
        total += GetValueFrom(randomBerryChance);
        total += GetValueFrom(antiBerryChance);
        total += GetValueFrom(nothingBerryChance);
        if (total == 100f)
        {
            customBackButton.interactable = true;
            customPlayButton.interactable = true;
            validateTotalText.gameObject.SetActive(false);
        }
        else
        {
            customNormalizeButton.interactable = true;
            validateTotalText.text = "Total must add up to 100%!";
            validateTotalText.gameObject.SetActive(true);
        }
    }

    private float GetValueFrom(TMP_InputField inputField)
    {
        return GetValueFroText(inputField.text);
    }

    private float GetValueFroText(string text)
    {
        try
        {
            return float.Parse(text.Replace("%", "").Trim());
        }
        catch
        {
            return 0f;
        }
    }

    void OnSelect(string value, TMP_InputField inputField)
    {
        validateTotalText.gameObject.SetActive(false);
        customBackButton.interactable = false;
        customPlayButton.interactable = false;
        customNormalizeButton.interactable = false;
        inputField.text = value.Replace("%", "");
        inputField.stringPosition = inputField.text.Length;
    }

    void OnDeselect(string value, TMP_InputField inputField)
    {
        if (float.TryParse(value, out var value2) && value2 < 0f)
        {
            value = "0";
        }
        inputField.text = value + "%";
        ValidateTotal();
    }
}