using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountRegister : MonoBehaviour
{
    public TMP_Text registerPanelStatusText;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerRetypeEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerRetypePasswordInput;
    public Button registerBackButton;
    public Button registerSubmitButton;

    void Awake()
    {
        registerBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        registerSubmitButton.onClick.AddListener(() => SubmitRegister());
    }

    void OnEnable()
    {
        registerUsernameInput.text = "";
        registerEmailInput.text = "";
        registerRetypeEmailInput.text = "";
        registerPasswordInput.text = "";
        registerRetypePasswordInput.text = "";
        registerPanelStatusText.text = "";
    }

    async void SubmitRegister()
    {
        if (
            registerUsernameInput.text == string.Empty ||
            registerEmailInput.text == string.Empty ||
            registerRetypeEmailInput.text == string.Empty ||
            registerPasswordInput.text == string.Empty ||
            registerRetypePasswordInput.text == string.Empty
        )
        {
            Tools.UpdateStatusText(registerPanelStatusText, "All input fields must be filled", Color.red);
            return;
        }
        if (registerEmailInput.text != registerRetypeEmailInput.text)
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Emails don't match", Color.red);
            return;
        }
        if (registerPasswordInput.text != registerRetypePasswordInput.text)
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Passwords don't match", Color.red);
            return;
        }
        registerBackButton.interactable = false;
        registerSubmitButton.interactable = false;
        WWWForm dataForm = new();
        dataForm.AddField("username", registerUsernameInput.text);
        dataForm.AddField("email", registerEmailInput.text);
        dataForm.AddField("password", registerPasswordInput.text);
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "registerAccount.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            registerBackButton.interactable = true;
            registerSubmitButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "-999")
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Server error while fetching data", Color.red);
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Client version too outdated to access servers", Color.red);
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Encryption/decryption issues", Color.red);
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(registerPanelStatusText, "Can't send requests on self-built instance", Color.red);
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                AccountHandler.instance.SwitchPanel(2);
            }
            else
            {
                Tools.UpdateStatusText(registerPanelStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        registerBackButton.interactable = true;
        registerSubmitButton.interactable = true;
    }
}