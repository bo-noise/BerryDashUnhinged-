using System.Collections;
using System.Linq;
using Newtonsoft.Json;
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
    private string statusMessage;
    private Coroutine statusRoutine;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
    }

    internal void Load()
    {
        GetIcons();
        balanceText.text = "You have " + Tools.FormatWithCommas(BazookaManager.Instance.GetCustomBirdIconData().Balance) + " coins to spend";
    }

    async void GetIcons()
    {
        backButton.interactable = false;
        foreach (Transform item in content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        ShowStatus("Loading...");
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getMarketplaceIcons.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowStatus("Failed to make HTTP request");
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
            ShowStatus(null);
            var icons = JsonConvert.DeserializeObject<MarketplaceIconType[]>(response);
            foreach (var entry in icons)
            {
                GameObject newIcon = Instantiate(sample, content.transform);
                newIcon.name = "IconEntry";

                Tools.RenderFromBase64(entry.Data, newIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>());
                newIcon.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bird Name: " + entry.Name;
                newIcon.transform.GetChild(2).GetComponent<TMP_Text>().text = "Price " + Tools.FormatWithCommas(entry.Price) + " coin";
                newIcon.transform.GetChild(3).GetComponent<TMP_Text>().text = "Designer Name: " + entry.CreatorUsername;

                var btn = newIcon.transform.GetChild(4).GetComponent<Button>();
                var btnText = btn.transform.GetChild(0).GetComponent<TMP_Text>();

                bool alreadyBought = BazookaManager.Instance.GetCustomBirdIconData().Data.Any(d => d.UUID == entry.UUID);
                if (alreadyBought)
                {
                    btn.interactable = false;
                    btnText.text = "Purchased";
                }
                else
                {
                    btn.onClick.AddListener(() => HandlePurchase(entry, btn));
                    btnText.text = "Purchase";
                }

                newIcon.SetActive(true);
            }
        }
        backButton.interactable = true;
    }

    void HandlePurchase(MarketplaceIconType data, Button button)
    {
        button.interactable = false;
        MarketplaceIconStorageType marketplaceIconStorage = BazookaManager.Instance.GetCustomBirdIconData();
        if (data.Price > marketplaceIconStorage.Balance)
        {
            button.interactable = true;
            ShowStatus("You can't afford this icon! You need " + (data.Price - marketplaceIconStorage.Balance) + " more coins");
            return;
        }
        var list = marketplaceIconStorage.Data.ToList();
        list.Add(data);
        marketplaceIconStorage.Data = list.ToArray();
        marketplaceIconStorage.Balance -= data.Price;
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Purchased";
        balanceText.text = "You have " + Tools.FormatWithCommas(marketplaceIconStorage.Balance) + " coins to spend";
        BazookaManager.Instance.SetCustomBirdIconData(marketplaceIconStorage);
    }

    void ShowStatus(string content)
    {
        if (content == null)
        {
            if (statusRoutine != null) StopCoroutine(statusRoutine);
            statusText.gameObject.SetActive(false);
            statusText.text = "";
        }
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
}
