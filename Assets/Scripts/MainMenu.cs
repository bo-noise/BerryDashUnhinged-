using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button exitButton;
    public TMP_Text updateText;
    public Button updateButton;
    public GameObject selfBuiltPanel;
    public Button profileButton;
    public ProfileMenu profilePrefab;

    void Awake()
    {
        LatestVersionText.Instance.text = updateText;
        LatestVersionText.Instance.updateButton = updateButton;
        LatestVersionText.Instance.RefreshText();

        if (Application.isMobilePlatform || Application.isEditor)
        {
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
        if (SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY.Trim() == string.Empty || SensitiveInfo.SERVER_SEND_TRANSFER_KEY.Trim() == string.Empty) selfBuiltPanel.SetActive(true);
    }

    void Start()
    {
        if (BazookaManager.Instance.GetAccountID() != null)
        {
            profileButton.transform.GetChild(0).GetComponent<TMP_Text>().text = BazookaManager.Instance.GetAccountName();
            profileButton.onClick.AddListener(async () =>
            {
                var clone = Instantiate(profilePrefab.gameObject, profilePrefab.gameObject.transform.parent);
                clone.SetActive(true);
                var customIconData = BazookaManager.Instance.GetCustomBirdIconData();
                string customIcon = null;
                if (customIconData.Selected != null)
                {
                    foreach (var icon in customIconData.Data)
                    {
                        if (icon.UUID == customIconData.Selected)
                        {
                            customIcon = icon.Data;
                        }
                    }
                }
                var iconColor = BazookaManager.Instance.GetColorSettingIcon();
                var overlayColor = BazookaManager.Instance.GetColorSettingOverlay();
                await clone.GetComponent<ProfileMenu>().Init(
                    BazookaManager.Instance.GetGameStoreTotalNormalBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalPoisonBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalSlowBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalUltraBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalSpeedyBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalCoinBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalRandomBerries().ToString(),
                    BazookaManager.Instance.GetGameStoreTotalAntiBerries().ToString(),
                    customIconData.Balance.ToString(),
                    BazookaManager.Instance.GetAccountName().ToString(),
                    BazookaManager.Instance.GetAccountID() ?? 0,
                    BazookaManager.Instance.GetBirdIcon(),
                    BazookaManager.Instance.GetBirdOverlay(),
                    customIcon,
                    new Color((int)iconColor[0] / 255f, (int)iconColor[1] / 255f, (int)iconColor[2] / 255f),
                    new Color((int)overlayColor[0] / 255f, (int)overlayColor[1] / 255f, (int)overlayColor[2] / 255f)
                );
            });
            profileButton.gameObject.SetActive(true);
        }
    }
}