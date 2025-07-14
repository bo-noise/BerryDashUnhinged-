using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceDownloadIcon : MonoBehaviour
{
    public TMP_Text statusText;
    public Button backButton;
    private IconMarketplaceManager marketplaceManager;
    public GameObject content;
    public GameObject sample;

    public void Load(IconMarketplaceManager loadedFrom)
    {
        marketplaceManager = loadedFrom;
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
        GetIcons();
    }

    async void GetIcons()
    {
        foreach (Transform item in content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        AccountHandler.UpdateStatusText(statusText, "Loading...", Color.white);
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getMarketplaceIcons.php");
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
            AccountHandler.UpdateStatusText(statusText, "", Color.red);
            var jsonResponse = JArray.Parse(response);
            foreach (var item in jsonResponse)
            {
                JObject entry = (JObject)item;
                GameObject newIcon = Instantiate(sample, content.transform);

                ImageUtil.RenderFromBase64(entry["data"].ToString(), newIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>());
                newIcon.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bird Name: " + entry["name"].ToString();
                newIcon.transform.GetChild(2).GetComponent<TMP_Text>().text = "Designer Name: " + entry["username"].ToString();
                newIcon.transform.GetChild(3).GetComponent<TMP_Text>().text = "Price " + entry["price"].ToString() + " coin";
                newIcon.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = "Purchase";

                newIcon.SetActive(true);
            }
        }
    }
}
