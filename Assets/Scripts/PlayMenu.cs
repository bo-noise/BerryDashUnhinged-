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
        customPlayButton.interactable = false;
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
            customPlayButton.interactable = true;
            validateTotalText.gameObject.SetActive(false);
        }
        else
        {
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
        customBackButton.interactable = true;
        ValidateTotal();
    }
}