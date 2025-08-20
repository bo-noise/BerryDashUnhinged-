using System.Numerics;
using Newtonsoft.Json;
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
        Tools.UpdateStatusText(loggedInText, "Logged in as: " + BazookaManager.Instance.GetAccountName(), Color.white);
    }

    async void SaveAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        loggedInBackButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("highScore", BazookaManager.Instance.GetGameStoreHighScore().ToString());
        dataForm.AddField("icon", BazookaManager.Instance.GetBirdIcon().ToString());
        dataForm.AddField("overlay", BazookaManager.Instance.GetBirdOverlay().ToString());
        dataForm.AddField("totalNormalBerries", BazookaManager.Instance.GetGameStoreTotalNormalBerries().ToString());
        dataForm.AddField("totalPoisonBerries", BazookaManager.Instance.GetGameStoreTotalPoisonBerries().ToString());
        dataForm.AddField("totalSlowBerries", BazookaManager.Instance.GetGameStoreTotalSlowBerries().ToString());
        dataForm.AddField("totalUltraBerries", BazookaManager.Instance.GetGameStoreTotalUltraBerries().ToString());
        dataForm.AddField("totalSpeedyBerries", BazookaManager.Instance.GetGameStoreTotalSpeedyBerries().ToString());
        dataForm.AddField("totalCoinBerries", BazookaManager.Instance.GetGameStoreTotalCoinBerries().ToString());
        dataForm.AddField("totalAttempts", BazookaManager.Instance.GetGameStoreTotalAttepts().ToString());
        dataForm.AddField("birdColor", BazookaManager.Instance.GetColorSettingIcon().ToString(Formatting.None));
        dataForm.AddField("overlayColor", BazookaManager.Instance.GetColorSettingOverlay().ToString(Formatting.None));
        dataForm.AddField("marketplaceData", JsonConvert.SerializeObject(BazookaManager.Instance.GetCustomBirdIconData()));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "saveAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                Tools.UpdateStatusText(loggedInText, "Synced account", Color.green);
            }
            else
            {
                Tools.UpdateStatusText(loggedInText, (string)jsonResponse["message"], Color.red);
            }
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
        loggedInBackButton.interactable = true;
    }

    async void LoadAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        loggedInBackButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loadAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            loggedInLoadButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetGameStoreHighScore(BigInteger.Parse((string)jsonResponse["highscore"]));
                BazookaManager.Instance.SetBirdIcon((int)jsonResponse["icon"]);
                BazookaManager.Instance.SetBirdOverlay((int)jsonResponse["overlay"]);
                BazookaManager.Instance.SetGameStoreTotalNormalBerries(BigInteger.Parse((string)jsonResponse["totalNormalBerries"]));
                BazookaManager.Instance.SetGameStoreTotalPoisonBerries(BigInteger.Parse((string)jsonResponse["totalPoisonBerries"]));
                BazookaManager.Instance.SetGameStoreTotalSlowBerries(BigInteger.Parse((string)jsonResponse["totalSlowBerries"]));
                BazookaManager.Instance.SetGameStoreTotalUltraBerries(BigInteger.Parse((string)jsonResponse["totalUltraBerries"]));
                BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(BigInteger.Parse((string)jsonResponse["totalSpeedyBerries"]));
                BazookaManager.Instance.SetGameStoreTotalCoinBerries(BigInteger.Parse((string)jsonResponse["totalCoinBerries"]));
                BazookaManager.Instance.SetGameStoreTotalAttepts(BigInteger.Parse((string)jsonResponse["totalAttempts"]));
                BazookaManager.Instance.SetColorSettingIcon(JArray.Parse(jsonResponse["birdColor"].ToString()));
                BazookaManager.Instance.SetColorSettingOverlay(JArray.Parse(jsonResponse["overlayColor"].ToString()));
                BazookaManager.Instance.SetCustomBirdIconData(JsonConvert.DeserializeObject<MarketplaceIconStorageType>(jsonResponse["marketplaceData"].ToString()));
                Tools.UpdateStatusText(loggedInText, "Loaded account data", Color.green);
            }
            else
            {
                Tools.UpdateStatusText(loggedInText, (string)jsonResponse["message"], Color.red);
            }
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
        loggedInBackButton.interactable = true;
    }
}