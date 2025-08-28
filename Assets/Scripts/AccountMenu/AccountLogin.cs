using System.Numerics;
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
        loginBackButton.interactable = false;
        loginSubmitButton.interactable = false;
        if (loginUsernameInput.text == string.Empty || loginPasswordInput.text == string.Empty)
        {
            Tools.UpdateStatusText(loginPanelStatusText, "All input fields must be filled", Color.red);
            return;
        }
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", loginUsernameInput.text);
        dataForm.AddField("password", loginPasswordInput.text);
        dataForm.AddField("currentHighScore", BazookaManager.Instance.GetGameStoreHighScore().ToString());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            loginBackButton.interactable = true;
            loginSubmitButton.interactable = true;
            Tools.UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            Tools.UpdateStatusText(loginPanelStatusText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(loginPanelStatusText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(loginPanelStatusText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(loginPanelStatusText, "Can't send requests on self-built instance", Color.red);
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountSession((string)jsonResponse["data"]["session"]);
                BazookaManager.Instance.SetAccountName((string)jsonResponse["data"]["username"]);
                BazookaManager.Instance.SetAccountID(BigInteger.Parse((string)jsonResponse["data"]["userid"]));
                AccountHandler.instance.SwitchPanel(0);
                Tools.UpdateStatusText(loginPanelStatusText, "", Color.red);
            }
            else
            {
                Tools.UpdateStatusText(loginPanelStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        loginBackButton.interactable = true;
        loginSubmitButton.interactable = true;
    }
}