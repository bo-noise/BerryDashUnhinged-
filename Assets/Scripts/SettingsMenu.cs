using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle setting1toggle;
    public Toggle setting2toggle;
    public Toggle setting3toggle;
    public Toggle setting4toggle;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        if (!Application.isMobilePlatform)
        {
            setting1toggle.isOn = BazookaManager.Instance.GetSettingFullScreen() ?? true == true;
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.isOn = BazookaManager.Instance.GetSettingVsync() ?? true == true;
            setting4toggle.isOn = BazookaManager.Instance.GetSettingHideSocials() == true;
        }
        else
        {
            setting1toggle.interactable = false;
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.interactable = false;
            setting4toggle.isOn = BazookaManager.Instance.GetSettingHideSocials() == true;
        }
        setting1toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingFullScreen(value);
            Screen.fullScreen = value;
        });
        setting2toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingShowFPS(value);
        });
        setting3toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingVsync(value);
            QualitySettings.vSyncCount = value ? 1 : -1;
        });
        setting4toggle.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingHideSocials(value));
        musicSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingMusicVolume(value));
        sfxSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingSFXVolume(value));
    }
}