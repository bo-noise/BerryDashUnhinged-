using System.Text.RegularExpressions;
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
        dataForm.AddField("inputPassword", changePasswordCurrentPasswordInput.text);
        dataForm.AddField("inputNewPassword", changePasswordNewPasswordInput.text);
        dataForm.AddField("session", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("userName", BazookaManager.Instance.GetAccountName());
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
        switch (response)
        {
            case "-999":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Encryption/decryption issues", Color.red);
                break;
            case "-996":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Can't send requests on self-built instance", Color.red);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "New Password is too short or too long", Color.red);
                break;
            case "-2":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Password must have 8 characters, one number and one letter", Color.red);
                break;
            case "-3":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Incorrect current password", Color.red);
                break;
            case "-4":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Failed to find info about your user (refresh login?)", Color.red);
                break;
            case "-5":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "New password cannot be the same as your old password", Color.red);
                break;
        }
        if (Regex.IsMatch(response, "^[a-zA-Z0-9]{512}$"))
        {
            BazookaManager.Instance.SetAccountSession(response);
            AccountHandler.instance.SwitchPanel(0);
            AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Password changed successfully", Color.green);
        }
        else
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Unknown server response " + response, Color.red);
        }
    }
}