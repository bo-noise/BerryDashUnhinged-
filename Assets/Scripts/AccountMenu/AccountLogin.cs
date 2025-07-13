using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountLogin : MonoBehaviour
{
    public TMP_Text loginPanelStatusText;
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public Button loginBackButton;
    public Button loginSubmitButton;

    void Awake()
    {
        loginBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        loginSubmitButton.onClick.AddListener(() => SubmitLogin());
    }

    void OnEnable()
    {
        loginUsernameInput.text = "";
        loginPasswordInput.text = "";
        loginPanelStatusText.text = "";
    }

    async void SubmitLogin()
    {
        if (loginUsernameInput.text == string.Empty || loginPasswordInput.text == string.Empty)
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "All input fields must be filled", Color.red);
            return;
        }
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", loginUsernameInput.text);
        dataForm.AddField("password", loginPasswordInput.text);
        dataForm.AddField("currentHighScore", BazookaManager.Instance.GetGameStoreHighScore().ToString());
        dataForm.AddField("loginType", "0");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountSession((string)jsonResponse["data"]["session"]);
                BazookaManager.Instance.SetAccountName((string)jsonResponse["data"]["username"]);
                BazookaManager.Instance.SetAccountID(BigInteger.Parse((string)jsonResponse["data"]["userid"]));
                BazookaManager.Instance.SetGameStoreHighScore(BigInteger.Parse((string)jsonResponse["data"]["highscore"]));
                BazookaManager.Instance.SetBirdIcon((int)jsonResponse["data"]["icon"]);
                BazookaManager.Instance.SetBirdOverlay((int)jsonResponse["data"]["overlay"]);
                BazookaManager.Instance.SetGameStoreTotalNormalBerries(BigInteger.Parse((string)jsonResponse["data"]["totalNormalBerries"]));
                BazookaManager.Instance.SetGameStoreTotalPoisonBerries(BigInteger.Parse((string)jsonResponse["data"]["totalPoisonBerries"]));
                BazookaManager.Instance.SetGameStoreTotalSlowBerries(BigInteger.Parse((string)jsonResponse["data"]["totalSlowBerries"]));
                BazookaManager.Instance.SetGameStoreTotalUltraBerries(BigInteger.Parse((string)jsonResponse["data"]["totalUltraBerries"]));
                BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(BigInteger.Parse((string)jsonResponse["data"]["totalSpeedyBerries"]));
                BazookaManager.Instance.SetGameStoreTotalAttepts(BigInteger.Parse((string)jsonResponse["data"]["totalAttempts"]));
                BazookaManager.Instance.SetColorSettingIcon(JArray.Parse(jsonResponse["data"]["birdColor"].ToString()));
                BazookaManager.Instance.SetColorSettingOverlay(JArray.Parse(jsonResponse["data"]["overlayColor"].ToString()));
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(loginPanelStatusText, "", Color.red);
            }
            else
            {
                AccountHandler.UpdateStatusText(loginPanelStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
    }
}