using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountChangePassword : MonoBehaviour
{
    public TMP_Text changePasswordStatusText;
    public TMP_InputField changePasswordCurrentPasswordInput;
    public TMP_InputField changePasswordNewPasswordInput;
    public TMP_InputField changePasswordRetypeNewPasswordInput;
    public Button changePasswordBackButton;
    public Button changePasswordSubmitButton;

    void Awake()
    {
        changePasswordBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        changePasswordSubmitButton.onClick.AddListener(() => ChangePassword());
    }

    void OnEnable()
    {
        changePasswordCurrentPasswordInput.text = "";
        changePasswordNewPasswordInput.text = "";
        changePasswordRetypeNewPasswordInput.text = "";
        changePasswordStatusText.text = "";
    }

    async void ChangePassword()
    {
        if (changePasswordNewPasswordInput.text != changePasswordRetypeNewPasswordInput.text)
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Passwords do not match", Color.red);
            return;
        }
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("oldpassword", changePasswordCurrentPasswordInput.text);
        dataForm.AddField("newpassword", changePasswordNewPasswordInput.text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "changeAccountPassword.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountSession((string)jsonResponse["token"]);
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Password changed successfully", Color.green);
            }
            else
            {
                AccountHandler.UpdateStatusText(changePasswordStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
    }
}