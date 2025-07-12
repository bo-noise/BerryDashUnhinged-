using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuBgColorPanel : MonoBehaviour
{
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;
    public ColorPickerUI colorPickerUI;
    public GameObject manualModeUI;
    public GameObject settingsUI;
    public TMP_InputField hexValue;
    public Button resetButton;
    public Button switchModeButton;
    public Button previewButton;

    void Awake()
    {
        var backgroundColor = BazookaManager.Instance.GetColorSettingBackground();
        rSlider.value = (int)backgroundColor[0];
        gSlider.value = (int)backgroundColor[1];
        bSlider.value = (int)backgroundColor[2];

        SyncAll();

        rSlider.onValueChanged.AddListener(_ => SyncAll());
        gSlider.onValueChanged.AddListener(_ => SyncAll());
        bSlider.onValueChanged.AddListener(_ => SyncAll());

        hexValue.onValueChanged.AddListener(value =>
        {
            var hex = value.StartsWith("#") ? value : "#" + value;
            if (hex.Length == 7 && ColorUtility.TryParseHtmlString(hex, out var col))
            {
                rSlider.SetValueWithoutNotify(col.r * 255f);
                gSlider.SetValueWithoutNotify(col.g * 255f);
                bSlider.SetValueWithoutNotify(col.b * 255f);
                SyncAll();
            }
        });

        resetButton.onClick.AddListener(() =>
        {
            rSlider.value = gSlider.value = bSlider.value = 58;
        });

        switchModeButton.onClick.AddListener(() =>
        {
            bool enableManual = !manualModeUI.activeSelf;
            manualModeUI.SetActive(enableManual);
            colorPickerUI.gameObject.SetActive(!enableManual);
        });

        previewButton.onClick.AddListener(() =>
        {
            settingsUI.SetActive(!settingsUI.activeSelf);
            Camera.main.backgroundColor = !settingsUI.activeSelf ? new Color(rSlider.value / 255f, gSlider.value / 255f, bSlider.value / 255f) : new Color(24 / 255f, 24 / 255f, 24 / 255f);
            previewButton.transform.GetChild(0).GetComponent<TMP_Text>().text = settingsUI.activeSelf ? "Preview On" : "Preview Off";
        });

        colorPickerUI.OnColorChanged += color =>
        {
            rSlider.SetValueWithoutNotify(color.r * 255);
            gSlider.SetValueWithoutNotify(color.g * 255);
            bSlider.SetValueWithoutNotify(color.b * 255);
            SyncAll(fromPicker: true);
        };
    }

    void SyncAll(bool fromPicker = false)
    {
        var col = new Color(rSlider.value / 255f, gSlider.value / 255f, bSlider.value / 255f);

        if (!fromPicker) colorPickerUI.SetSelectedColor(rSlider.value, gSlider.value, bSlider.value);
        if (!settingsUI.activeSelf) Camera.main.backgroundColor = col;

        hexValue.SetTextWithoutNotify("#" + ColorUtility.ToHtmlStringRGB(col));
        BazookaManager.Instance.SetColorSettingBackground(new JArray(
            (int)rSlider.value,
            (int)gSlider.value,
            (int)bSlider.value
        ));
    }
}
