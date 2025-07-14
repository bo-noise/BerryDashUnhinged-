using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceUploadIcon : MonoBehaviour
{
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;
    public Button backButton;
    public Button uploadButton;
    public Button selectButton;
    public TMP_InputField birdNameInput;
    public TMP_InputField birdPriceInput;
    public Image birdImage;
    private string birdData = null;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
        uploadButton.onClick.AddListener(() =>
        {
            if (birdNameInput.text.Trim() == string.Empty)
            {
                AccountHandler.UpdateStatusText(statusText, "Bird name can't be empty", Color.red);
            }
            else if (birdPriceInput.text.Trim() == string.Empty)
            {
                AccountHandler.UpdateStatusText(statusText, "Bird price can't be empty", Color.red);
            }
            else if (birdData == null)
            {
                AccountHandler.UpdateStatusText(statusText, "You must upload an image", Color.red);
            }
            else
            {
                UploadIcon();
            }
        });
        selectButton.onClick.AddListener(() =>
        {
            string path = OpenFilePicker();
            if (path == null) return;
            byte[] fileContent;
            try
            {
                fileContent = File.ReadAllBytes(path);
                if (fileContent.Length > 1024 * 1024)
                {
                    AccountHandler.UpdateStatusText(statusText, "File size exceeds 1 MB limit", Color.red);
                    return;
                }
                birdData = Convert.ToBase64String(fileContent);
                ImageUtil.RenderFromBase64(birdData, birdImage);
            }
            catch (Exception e)
            {
                AccountHandler.UpdateStatusText(statusText, "Failed to read file: " + e.Message, Color.red);
                return;
            }
        });
    }

    string OpenFilePicker()
    {
        var filters = new[] {
            new ExtensionFilter("Image Files", "png"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select an icon file", "", filters, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            return paths[0];
        }
        return null;
    }

    async void UploadIcon()
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("name", birdNameInput.text);
        dataForm.AddField("price", birdPriceInput.text);
        dataForm.AddField("filecontent", birdData);
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
                birdImage.sprite = Resources.Load<Sprite>("Other/X");
                birdData = null;
                birdNameInput.text = "";
                birdPriceInput.text = "";
                AccountHandler.UpdateStatusText(statusText, "Icon uploaded successfully!", Color.green);
            }
            else
            {
                AccountHandler.UpdateStatusText(statusText, (string)jsonResponse["message"], Color.red);
            }
        }
    }
}
