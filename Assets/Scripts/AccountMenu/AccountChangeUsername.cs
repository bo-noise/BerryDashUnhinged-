using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountChangeUsername : MonoBehaviour
{
    public TMP_Text changeUsernameStatusText;
    public TMP_InputField changeUsernameCurrentUsernameInput;
    public TMP_InputField changeUsernameNewUsernameInput;
    public Button changeUsernameBackButton;
    public Button changeUsernameSubmitButton;

    void Awake()
    {
        changeUsernameBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        changeUsernameSubmitButton.onClick.AddListener(() => ChangeUsername());
    }

    void OnEnable()
    {
        changeUsernameCurrentUsernameInput.text = "";
        changeUsernameNewUsernameInput.text = "";
        changeUsernameStatusText.text = "";
    }

    async void ChangeUsername()
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("inputUserName", changeUsernameCurrentUsernameInput.text);
        dataForm.AddField("inputNewUserName", changeUsernameNewUsernameInput.text);
        dataForm.AddField("session", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("userName", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "changeAccountUsername.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "-999":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Encryption/decryption issues", Color.red);
                break;
            case "-996":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Can't send requests on self-built instance", Color.red);
                break;
            case "1":
                BazookaManager.Instance.SetAccountName(changeUsernameNewUsernameInput.text);
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Username changed successfully", Color.green);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "New Username must be 3-16 characters, letters and numbers only", Color.red);
                break;
            case "-2":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "New username already exists", Color.red);
                break;
            case "-3":
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Failed to find info about your user (refresh login?)", Color.red);
                break;
            default:
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Unknown server response", Color.red);
                break;
        }
    }
}