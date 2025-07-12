using System.Numerics;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountLoggedIn : MonoBehaviour
{
    public TMP_Text loggedInText;
    public AccountLoggedOut accountLoggedOut;
    public Button loggedInChangeUsernameButton;
    public Button loggedInChangePasswordButton;
    public Button loggedInSaveButton;
    public Button loggedInLoadButton;
    public Button loggedInRefreshLoginButton;
    public Button loggedInLogoutButton;
    public Button loggedInBackButton;

    void Awake()
    {
        loggedInChangeUsernameButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(4));
        loggedInChangePasswordButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(5));
        loggedInSaveButton.onClick.AddListener(() => SaveAccount());
        loggedInLoadButton.onClick.AddListener(() => LoadAccount());
        loggedInRefreshLoginButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(6));
        loggedInLogoutButton.onClick.AddListener(() =>
        {
            accountLoggedOut.clearValues = true;
            AccountHandler.instance.SwitchPanel(1);
        });
        loggedInBackButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
    }

    void OnEnable()
    {
        loggedInSaveButton.interactable = true;
        loggedInLoadButton.interactable = true;
        loggedInText.text = "Logged in as: " + BazookaManager.Instance.GetAccountName();
    }

    async void SaveAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("userName", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("gameSession", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("highScore", BazookaManager.Instance.GetGameStoreHighScore().ToString());
        dataForm.AddField("icon", BazookaManager.Instance.GetBirdIcon().ToString());
        dataForm.AddField("overlay", BazookaManager.Instance.GetBirdOverlay().ToString());
        dataForm.AddField("totalNormalBerries", BazookaManager.Instance.GetGameStoreTotalNormalBerries().ToString());
        dataForm.AddField("totalPoisonBerries", BazookaManager.Instance.GetGameStoreTotalPoisonBerries().ToString());
        dataForm.AddField("totalSlowBerries", BazookaManager.Instance.GetGameStoreTotalSlowBerries().ToString());
        dataForm.AddField("totalUltraBerries", BazookaManager.Instance.GetGameStoreTotalUltraBerries().ToString());
        dataForm.AddField("totalSpeedyBerries", BazookaManager.Instance.GetGameStoreTotalSpeedyBerries().ToString());
        dataForm.AddField("totalAttempts", BazookaManager.Instance.GetGameStoreTotalAttepts().ToString());
        dataForm.AddField("birdR", BazookaManager.Instance.GetColorSettingIcon()[0].ToString());
        dataForm.AddField("birdG", BazookaManager.Instance.GetColorSettingIcon()[1].ToString());
        dataForm.AddField("birdB", BazookaManager.Instance.GetColorSettingIcon()[2].ToString());
        dataForm.AddField("overlayR", BazookaManager.Instance.GetColorSettingOverlay()[0].ToString());
        dataForm.AddField("overlayG", BazookaManager.Instance.GetColorSettingOverlay()[1].ToString());
        dataForm.AddField("overlayB", BazookaManager.Instance.GetColorSettingOverlay()[2].ToString());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "saveAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "-999":
                AccountHandler.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
                break;
            case "-996":
                AccountHandler.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
                break;
            case "1":
                AccountHandler.UpdateStatusText(loggedInText, "Synced account", Color.green);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Failed to find info about your user (refresh login?)", Color.red);
                break;
            default:
                AccountHandler.UpdateStatusText(loggedInText, "Unknown server response", Color.red);
                break;
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
    }

    async void LoadAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("userName", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("gameSession", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loadAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "-999":
                AccountHandler.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
                break;
            case "-996":
                AccountHandler.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Failed to find info about your user (refresh login?)", Color.red);
                break;
            default:
                var split = response.Split(":");
                if (split[0] == "1")
                {
                    BazookaManager.Instance.SetGameStoreHighScore(BigInteger.Parse(split[1]));
                    BazookaManager.Instance.SetBirdIcon(int.Parse(split[2]));
                    BazookaManager.Instance.SetBirdOverlay(int.Parse(split[3]));
                    BazookaManager.Instance.SetGameStoreTotalNormalBerries(BigInteger.Parse(split[4]));
                    BazookaManager.Instance.SetGameStoreTotalPoisonBerries(BigInteger.Parse(split[5]));
                    BazookaManager.Instance.SetGameStoreTotalSlowBerries(BigInteger.Parse(split[6]));
                    BazookaManager.Instance.SetGameStoreTotalUltraBerries(BigInteger.Parse(split[7]));
                    BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(BigInteger.Parse(split[8]));
                    BazookaManager.Instance.SetGameStoreTotalAttepts(BigInteger.Parse(split[9]));
                    BazookaManager.Instance.SetColorSettingIcon(new JArray(
                        int.Parse(split[10]),
                        int.Parse(split[11]),
                        int.Parse(split[12])
                    ));
                    BazookaManager.Instance.SetColorSettingOverlay(new JArray(
                        int.Parse(split[13]),
                        int.Parse(split[14]),
                        int.Parse(split[15])
                    ));
                    AccountHandler.UpdateStatusText(loggedInText, "Loaded account data", Color.green);
                }
                else
                {
                    AccountHandler.UpdateStatusText(loggedInText, "Unknown server response", Color.red);
                }
                break;
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
    }
}