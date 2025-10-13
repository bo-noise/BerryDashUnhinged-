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
        changeUsernameBackButton.interactable = false;
        changeUsernameSubmitButton.interactable = false;
        WWWForm dataForm = new();
        dataForm.AddField("oldusername", changeUsernameCurrentUsernameInput.text);
        dataForm.AddField("newusername", changeUsernameNewUsernameInput.text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/changeAccountUsername.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            changeUsernameBackButton.interactable = true;
            changeUsernameSubmitButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "-999")
        {
            Tools.UpdateStatusText(changeUsernameStatusText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(changeUsernameStatusText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(changeUsernameStatusText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(changeUsernameStatusText, "Can't send requests on self-built instance", Color.red);
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountName(changeUsernameNewUsernameInput.text);
                AccountHandler.instance.SwitchPanel(0);
                Tools.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Username changed successfully", Color.green);
            }
            else
            {
                Tools.UpdateStatusText(changeUsernameStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        changeUsernameBackButton.interactable = true;
        changeUsernameSubmitButton.interactable = true;
    }
}