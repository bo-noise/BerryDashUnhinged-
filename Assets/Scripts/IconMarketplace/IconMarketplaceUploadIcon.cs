using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class IconMarketplaceUploadIcon : MonoBehaviour
{
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;

    void Awake()
    {
        OpenFilePicker();
    }

    void OpenFilePicker()
    {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
        var filters = new[] {
            new ExtensionFilter("Image Files", "png"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select an icon file", "", filters, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            UploadFile(paths[0]);
        }
#endif
    }

    async void UploadFile(string path)
    {
        byte[] fileContent;
        try
        {
            fileContent = File.ReadAllBytes(path);
            if (fileContent.Length > 1024 * 1024)
            {
                AccountHandler.UpdateStatusText(statusText, "File size exceeds 1 MB limit", Color.red);
                return;
            }
        }
        catch (Exception e)
        {
            AccountHandler.UpdateStatusText(statusText, "Failed to read file: " + e.Message, Color.red);
            return;
        }

        EncryptedWWWForm dataForm = new();
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("filecontent", Convert.ToBase64String(fileContent));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "uploadMarketplaceIcon.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(statusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(statusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(statusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(statusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            AccountHandler.UpdateStatusText(statusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                AccountHandler.UpdateStatusText(statusText, "Icon uploaded successfully!", Color.green);
            }
            else
            {
                AccountHandler.UpdateStatusText(statusText, (string)jsonResponse["message"], Color.red);
            }
        }
    }
}
