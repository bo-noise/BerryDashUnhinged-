using Newtonsoft.Json.Linq;
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
        dataForm.AddField("oldusername", changeUsernameNewUsernameInput.text);
        dataForm.AddField("newusername", changeUsernameCurrentUsernameInput.text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
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
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountName(changeUsernameNewUsernameInput.text);
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Username changed successfully", Color.green);
            }
            else
            {
                AccountHandler.UpdateStatusText(changeUsernameStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
    }
}