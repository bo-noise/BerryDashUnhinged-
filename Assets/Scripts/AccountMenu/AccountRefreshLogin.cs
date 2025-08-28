using System.Numerics;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountRefreshLogin : MonoBehaviour
{
    public TMP_Text refreshLoginStatusText;
    public TMP_InputField refreshLoginUsernameInput;
    public TMP_InputField refreshLoginPasswordInput;
    public Button refreshLoginBackButton;
    public Button refreshLoginSubmitButton;

    void Awake()
    {
        refreshLoginBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        refreshLoginSubmitButton.onClick.AddListener(() => RefreshLogin());
    }

    void OnEnable()
    {
        refreshLoginUsernameInput.text = "";
        refreshLoginPasswordInput.text = "";
    }

    async void RefreshLogin()
    {
        refreshLoginBackButton.interactable = false;
        refreshLoginSubmitButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", refreshLoginUsernameInput.text);
        dataForm.AddField("password", refreshLoginPasswordInput.text);
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            refreshLoginBackButton.interactable = true;
            refreshLoginSubmitButton.interactable = true;
            Tools.UpdateStatusText(refreshLoginStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            Tools.UpdateStatusText(refreshLoginStatusText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(refreshLoginStatusText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(refreshLoginStatusText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(refreshLoginStatusText, "Can't send requests on self-built instance", Color.red);
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
                Tools.UpdateStatusText(refreshLoginStatusText, "", Color.red);
            }
            else
            {
                Tools.UpdateStatusText(refreshLoginStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        refreshLoginBackButton.interactable = true;
        refreshLoginSubmitButton.interactable = true;
    }
}