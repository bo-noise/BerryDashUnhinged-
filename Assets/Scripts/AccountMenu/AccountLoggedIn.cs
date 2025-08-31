using System;
using System.Text;
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
            BazookaManager.Instance.ResetSave();
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
        loggedInChangeUsernameButton.interactable = false;
        loggedInChangePasswordButton.interactable = false;
        loggedInSaveButton.interactable = false;
        loggedInLoadButton.interactable = false;
        loggedInRefreshLoginButton.interactable = false;
        loggedInLogoutButton.interactable = false;
        loggedInBackButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("saveData", Convert.ToBase64String(Encoding.UTF8.GetBytes(BazookaManager.Instance.saveFile.ToString(Formatting.None))));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "saveAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            loggedInChangeUsernameButton.interactable = true;
            loggedInChangePasswordButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInLoadButton.interactable = true;
            loggedInRefreshLoginButton.interactable = true;
            loggedInLogoutButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            Tools.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
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
        loggedInChangeUsernameButton.interactable = true;
        loggedInChangePasswordButton.interactable = true;
        loggedInSaveButton.interactable = true;
        loggedInLoadButton.interactable = true;
        loggedInRefreshLoginButton.interactable = true;
        loggedInLogoutButton.interactable = true;
        loggedInBackButton.interactable = true;
    }

    async void LoadAccount()
    {
        loggedInChangeUsernameButton.interactable = false;
        loggedInChangePasswordButton.interactable = false;
        loggedInSaveButton.interactable = false;
        loggedInLoadButton.interactable = false;
        loggedInRefreshLoginButton.interactable = false;
        loggedInLogoutButton.interactable = false;
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
            loggedInChangeUsernameButton.interactable = true;
            loggedInChangePasswordButton.interactable = true;
            loggedInSaveButton.interactable = true;
            loggedInLoadButton.interactable = true;
            loggedInRefreshLoginButton.interactable = true;
            loggedInLogoutButton.interactable = true;
            loggedInBackButton.interactable = true;
            Tools.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            Tools.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(loggedInText, "Can't send requests on self-built instance", Color.red);
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.saveFile = JObject.FromObject(jsonResponse["data"]);
                if (!Application.isMobilePlatform)
                {
                    var width = Display.main.systemWidth;
                    var height = Display.main.systemHeight;
                    Screen.SetResolution(width, height, BazookaManager.Instance.GetSettingFullScreen());
                    QualitySettings.vSyncCount = BazookaManager.Instance.GetSettingVsync() ? 1 : -1;
                }
                else
                {
                    Application.targetFrameRate = 360;
                    QualitySettings.vSyncCount = 0;
                }
                MenuMusic.Instance.GetComponent<AudioSource>().volume = BazookaManager.Instance.GetSettingMusicVolume();
                Tools.UpdateStatusText(loggedInText, "Loaded account data", Color.green);
            }
            else
            {
                Tools.UpdateStatusText(loggedInText, (string)jsonResponse["message"], Color.red);
            }
        }
        loggedInChangeUsernameButton.interactable = true;
        loggedInChangePasswordButton.interactable = true;
        loggedInSaveButton.interactable = true;
        loggedInLoadButton.interactable = true;
        loggedInRefreshLoginButton.interactable = true;
        loggedInLogoutButton.interactable = true;
        loggedInBackButton.interactable = true;
    }
}