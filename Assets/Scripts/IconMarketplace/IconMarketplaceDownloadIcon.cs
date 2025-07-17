using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceDownloadIcon : MonoBehaviour
{
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;
    public TMP_Text balanceText;
    public Button backButton;
    public GameObject content;
    public GameObject sample;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
    }

    internal void Load()
    {
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
        Tools.UpdateStatusText(statusText, "Loading...", Color.white);
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getMarketplaceIcons.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Tools.UpdateStatusText(statusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            Tools.UpdateStatusText(statusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            Tools.UpdateStatusText(statusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            Tools.UpdateStatusText(statusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            Tools.UpdateStatusText(statusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            Tools.UpdateStatusText(statusText, "", Color.red);
            var jsonResponse = JArray.Parse(response);
            foreach (var item in jsonResponse)
            {
                MarketplaceIconType entry = ((JObject)item).ToObject<MarketplaceIconType>();
                GameObject newIcon = Instantiate(sample, content.transform);
                newIcon.name = "IconEntry";

                Tools.RenderFromBase64(entry.Data, newIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>());
                newIcon.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bird Name: " + entry.Name;
                newIcon.transform.GetChild(2).GetComponent<TMP_Text>().text = "Price " + entry.Price + " coin";
                newIcon.transform.GetChild(3).GetComponent<TMP_Text>().text = "Designer Name: " + entry.CreatorUsername;
                newIcon.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = "Purchase";
                newIcon.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => HandlePurchase(entry));

                newIcon.SetActive(true);
            }
        }
    }

    void HandlePurchase(MarketplaceIconType data)
    {
        //will work on this
    }
}
