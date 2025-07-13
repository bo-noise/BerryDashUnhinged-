using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatroomMenu : MonoBehaviour
{
    public TMP_Text statusText;
    public Button backButton;
    public Button sendButton;
    public TMP_InputField messageInputField;
    public GameObject content;
    public GameObject sampleObject;
    private string statusMessage;
    private Coroutine statusRoutine;
    private Coroutine refreshLoopRoutine;

    void Start()
    {
        if (BazookaManager.Instance.GetAccountID() == null || BazookaManager.Instance.GetAccountName() == null || BazookaManager.Instance.GetAccountSession() == null)
        {
            sendButton.interactable = false;
            messageInputField.interactable = false;
            ShowStatus("Warning: You are not logged in. Please log in to send messages.");
        }
        backButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
        sendButton.onClick.AddListener(async () => await HandleMessageSubmit());
        messageInputField.textComponent.textWrappingMode = TextWrappingModes.Normal;
        messageInputField.onSubmit.AddListener(async (_) => await HandleMessageSubmit());
        refreshLoopRoutine = StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            Refresh();
            yield return new WaitForSeconds(3f);
        }
    }

    async Task HandleMessageSubmit()
    {
        if (!sendButton.interactable) return;
        var text = messageInputField.text.Clone() as string;
        messageInputField.text = string.Empty;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        EncryptedWWWForm dataForm = new();
        dataForm.AddField("content", text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "sendChatroomMessage.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            ShowStatus("Server error while fetching data");
            return;
        }
        else if (response == "-998")
        {
            ShowStatus("Client version too outdated to access servers");
            return;
        }
        else if (response == "-997")
        {
            ShowStatus("Encryption/decryption issues");
            return;
        }
        else if (response == "-996")
        {
            ShowStatus("Can't send requests on self-built instance");
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                StopCoroutine(refreshLoopRoutine);
                refreshLoopRoutine = StartCoroutine(Loop());
                content.transform.localPosition = new UnityEngine.Vector2(0f, 0f);
            }
            else
            {
                ShowStatus((string)jsonResponse["message"]);
            }
        }
    }

    void ShowStatus(string content)
    {
        statusMessage = content;
        if (statusRoutine != null) StopCoroutine(statusRoutine);
        statusRoutine = StartCoroutine(StatusRoutine());
    }

    IEnumerator StatusRoutine()
    {
        statusText.gameObject.SetActive(true);
        statusText.text = statusMessage;
        statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, 0f);

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float a = t / 0.5f;
            statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, a);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float a = 1f - (t / 0.5f);
            statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, a);
            yield return null;
        }

        statusText.gameObject.SetActive(false);
        statusText.text = "";
        statusRoutine = null;
    }

    async void Refresh()
    {
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getChatroomMessages.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch chatroom messages: " + request.error);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        var shouldClear = true;
        switch (response)
        {
            case "-999":
                ShowStatus("Server error while fetching data");
                break;
            case "-998":
                ShowStatus("Client version too outdated to access servers");
                break;
            case "-997":
                ShowStatus("Encryption/decryption issues");
                break;
            case "-996":
                ShowStatus("Can't send requests on self-built instance");
                break;
            default:
                shouldClear = false;
                var jsonResponse = JArray.Parse(response);
                foreach (JObject entry in jsonResponse.Cast<JObject>())
                {
                    var username = (string)entry["username"];
                    var chatContent = Encoding.UTF8.GetString(Convert.FromBase64String((string)entry["content"]));
                    var id = BigInteger.Parse((string)entry["id"]);
                    var icon = (int)entry["icon"];
                    var overlay = (int)entry["overlay"];
                    var uid = BigInteger.Parse((string)entry["userid"]);
                    var birdColor = (JArray)entry["birdColor"];
                    var overlayColor = (JArray)entry["overlayColor"];

                    if (content.transform.Find("ChatroomRow_" + id + "_" + uid) != null)
                    {
                        continue;
                    }
                    if (content.transform.childCount > 50)
                    {
                        var firstChild = content.transform.GetChild(0);
                        Destroy(firstChild.gameObject);
                    }

                    var rowInfo = Instantiate(sampleObject, content.transform);
                    var usernameText = rowInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var messageText = rowInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    usernameText.text = username;
                    messageText.text = chatContent;
                    playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                    if (icon == 1)
                    {
                        playerIcon.sprite = Tools.GetIconForUser(uid);
                    }
                    playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                    if (overlay == 0)
                    {
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    else if (overlay == 8)
                    {
                        playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                    }
                    else if (overlay == 11)
                    {
                        playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                    }
                    else if (overlay == 13)
                    {
                        playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                    }
                    try
                    {
                        playerIcon.color = new Color((int)birdColor[0] / 255f, (int)birdColor[1] / 255f, (int)birdColor[2] / 255f);
                        playerOverlayIcon.color = new Color((int)overlayColor[0] / 255f, (int)overlayColor[1] / 255f, (int)overlayColor[2] / 255f);
                    }
                    catch (Exception)
                    {
                        playerIcon.color = Color.white;
                        playerOverlayIcon.color = Color.white;
                    }
                    rowInfo.name = "ChatroomRow_" + id + "_" + uid;
                    rowInfo.SetActive(true);
                }
                break;
        }
        if (shouldClear)
        {
            foreach (Transform item in content.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}