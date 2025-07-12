using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconsMenuOverlayColorPanel : MonoBehaviour
{
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;
    public ColorPickerUI colorPickerUI;
    public GameObject manualModeUI;
    public TMP_InputField hexValue;
    public Image previewImage;
    public Button resetButton;
    public Button switchModeButton;

    void Awake()
    {
        var overlayColor = BazookaManager.Instance.GetColorSettingOverlay();
        rSlider.value = (int)overlayColor[0];
        gSlider.value = (int)overlayColor[1];
        bSlider.value = (int)overlayColor[2];

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
            rSlider.value = gSlider.value = bSlider.value = 255;
        });

        switchModeButton.onClick.AddListener(() =>
        {
            bool enableManual = !manualModeUI.activeSelf;
            manualModeUI.SetActive(enableManual);
            colorPickerUI.gameObject.SetActive(!enableManual);
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

        previewImage.color = col;
        hexValue.SetTextWithoutNotify("#" + ColorUtility.ToHtmlStringRGB(col));
        BazookaManager.Instance.SetColorSettingOverlay(new JArray(
            (int)rSlider.value,
            (int)gSlider.value,
            (int)bSlider.value
        ));
    }
}
