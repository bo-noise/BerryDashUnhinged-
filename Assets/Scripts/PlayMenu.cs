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
            float remainingDifference = difference;
            int remainingCount = divideBy;
            int sumAssigned = 0;

            if (normalBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = normalBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                normalBerryChance.text = displayVal.ToString().Replace("%", "");
                normalBerryChance.stringPosition = normalBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (poisonBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = poisonBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                poisonBerryChance.text = displayVal.ToString().Replace("%", "");
                poisonBerryChance.stringPosition = poisonBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (slowBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = slowBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                slowBerryChance.text = displayVal.ToString().Replace("%", "");
                slowBerryChance.stringPosition = slowBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (ultraBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = ultraBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                ultraBerryChance.text = displayVal.ToString().Replace("%", "");
                ultraBerryChance.stringPosition = ultraBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (speedyBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = speedyBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                speedyBerryChance.text = displayVal.ToString().Replace("%", "");
                speedyBerryChance.stringPosition = speedyBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (randomBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = randomBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                randomBerryChance.text = displayVal.ToString().Replace("%", "");
                randomBerryChance.stringPosition = randomBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (antiBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = antiBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                antiBerryChance.text = displayVal.ToString().Replace("%", "");
                antiBerryChance.stringPosition = antiBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
            }

            if (nothingBerry > 0)
            {
                float adjust = remainingCount > 0 ? (remainingDifference / remainingCount) : 0f;
                float newVal = nothingBerry - adjust;
                int displayVal;
                if (remainingCount == 1)
                    displayVal = 100 - sumAssigned;
                else
                    displayVal = (int)System.Math.Round(newVal);
                nothingBerryChance.text = displayVal.ToString().Replace("%", "");
                nothingBerryChance.stringPosition = nothingBerryChance.text.Length;
                remainingDifference -= adjust;
                remainingCount--;
                sumAssigned += displayVal;
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