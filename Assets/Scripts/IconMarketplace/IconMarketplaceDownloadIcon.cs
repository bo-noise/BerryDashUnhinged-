using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceDownloadIcon : MonoBehaviour
{
    private readonly static WaitForSeconds _waitForSeconds2 = new(2f);
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;
    public TMP_Text balanceText;
    public Button backButton;
    public GameObject content;
    public GameObject sample;
    private string statusMessage;
    private Coroutine statusRoutine;
    public AudioSource iconPurchaseSound;
    public Button refreshButton;
    public Button optionsButton;
    public GameObject optionsPanel;
    public Button optionsPanelSubmitButton;
    public Button optionsPanelResetButton;

    public TMP_Dropdown optionsPanelSortByDropdown;

    public Toggle optionsPanelPriceRangeToggle;
    public TMP_InputField optionsPanelPriceRangeMinInput;
    public TMP_InputField optionsPanelPriceRangeMaxInput;
    internal string priceRangeMin = "10";
    internal string priceRangeMax = "250";

    public Toggle optionsPanelSearchForToggle;
    public TMP_InputField optionsPanelSearchForInputField;
    internal string searchForValue = "";

    public Toggle optionsPanelOnlyShowToggle;
    public TMP_Dropdown optionsPanelOnlyShowDropdown;

    internal bool anyChanges = false;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
        refreshButton.onClick.AddListener(GetIcons);
        optionsButton.onClick.AddListener(() => optionsPanel.SetActive(true));
        optionsPanelSubmitButton.onClick.AddListener(() =>
        {
            optionsPanel.SetActive(false);
            if (anyChanges)
            {
                anyChanges = false;
                GetIcons();
            }
        });
        optionsPanelResetButton.onClick.AddListener(() =>
        {
            optionsPanelSortByDropdown.value = 3;
            optionsPanelPriceRangeToggle.isOn = false;
            optionsPanelSearchForToggle.isOn = false;
            optionsPanelOnlyShowToggle.isOn = false;
        });

        optionsPanelPriceRangeToggle.onValueChanged.AddListener((on) =>
        {
            anyChanges = true;
            if (!on)
            {
                optionsPanelPriceRangeMinInput.text = "10";
                optionsPanelPriceRangeMaxInput.text = "250";
            }
            optionsPanelPriceRangeMinInput.interactable = on;
            optionsPanelPriceRangeMaxInput.interactable = on;
        });
        optionsPanelSearchForToggle.onValueChanged.AddListener((on) =>
        {
            anyChanges = true;
            if (!on) optionsPanelSearchForInputField.text = "";
            optionsPanelSearchForInputField.interactable = on;
        });
        optionsPanelOnlyShowToggle.onValueChanged.AddListener((on) =>
        {
            anyChanges = true;
            if (!on) optionsPanelOnlyShowDropdown.value = 0;
            optionsPanelOnlyShowDropdown.interactable = on;
        });

        optionsPanelSortByDropdown.onValueChanged.AddListener((_) => anyChanges = true);
        optionsPanelPriceRangeMinInput.onValueChanged.AddListener((value) =>
        {
            anyChanges = true;
            priceRangeMin = value;
        });
        optionsPanelPriceRangeMaxInput.onValueChanged.AddListener((value) =>
        {
            anyChanges = true;
            priceRangeMax = value;
        });
        optionsPanelSearchForInputField.onValueChanged.AddListener((value) =>
        {
            anyChanges = true;
            searchForValue = value;
        });
        optionsPanelOnlyShowDropdown.onValueChanged.AddListener((_) => anyChanges = true);
    }

    internal void Load()
    {
        GetIcons();
        balanceText.text = "You have " + Tools.FormatWithCommas(BazookaManager.Instance.GetCustomBirdIconData().Balance) + " coins to spend";
    }

    async void GetIcons()
    {
        refreshButton.interactable = false;
        optionsButton.interactable = false;
        backButton.interactable = false;
        foreach (Transform item in content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        var currentIcons = new JArray();
        foreach (var icon in BazookaManager.Instance.GetCustomBirdIconData().Data)
        {
            currentIcons.Add(icon.UUID);
        }
        ShowStatus("Loading...");
        WWWForm dataForm = new();
        dataForm.AddField("userId", (BazookaManager.Instance.GetAccountID() ?? 0).ToString());
        dataForm.AddField("sortBy", optionsPanelSortByDropdown.value.ToString());
        dataForm.AddField("priceRangeEnabled", optionsPanelPriceRangeToggle.isOn.ToString());
        dataForm.AddField("priceRangeMin", priceRangeMin);
        dataForm.AddField("priceRangeMax", priceRangeMax);
        dataForm.AddField("searchForEnabled", optionsPanelSearchForToggle.isOn.ToString());
        dataForm.AddField("searchForValue", searchForValue);
        dataForm.AddField("onlyShowEnabled", optionsPanelOnlyShowToggle.isOn.ToString());
        dataForm.AddField("onlyShowValue", optionsPanelOnlyShowDropdown.value.ToString());
        dataForm.AddField("currentIcons", Convert.ToBase64String(Encoding.UTF8.GetBytes(currentIcons.ToString(Formatting.None))));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/getMarketplaceIcons.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            refreshButton.interactable = true;
            optionsButton.interactable = true;
            backButton.interactable = true;
            ShowStatus("Failed to make HTTP request");
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "-999")
        {
            ShowStatus("Server error while fetching data");
        }
        else if (response == "-998")
        {
            ShowStatus("Client version too outdated to access servers");
        }
        else if (response == "-997")
        {
            ShowStatus("Encryption/decryption issues");
        }
        else if (response == "-996")
        {
            ShowStatus("Can't send requests on self-built instance");
        }
        else
        {
            ShowStatus(null);
            var icons = JsonConvert.DeserializeObject<MarketplaceIconType[]>(response);
            var localUserID = BazookaManager.Instance.GetAccountID();
            foreach (var entry in icons)
            {
                GameObject newIcon = Instantiate(sample, content.transform);
                newIcon.name = "IconEntry";

                Tools.RenderFromBase64(entry.Data, newIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>());
                newIcon.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bird Name: " + entry.Name;
                newIcon.transform.GetChild(2).GetComponent<TMP_Text>().text = "Price " + Tools.FormatWithCommas(entry.Price) + " coins";
                newIcon.transform.GetChild(3).GetComponent<TMP_Text>().text = "Designer Name: " + entry.CreatorUsername;

                var btnGrid = newIcon.transform.GetChild(4);
                var buybtn = btnGrid.transform.GetChild(0).GetComponent<Button>();
                var sellbtn = btnGrid.transform.GetChild(1).GetComponent<Button>();
                var buyttnText = buybtn.transform.GetChild(0).GetComponent<TMP_Text>();

                sellbtn.onClick.AddListener(() =>
                {
                    HandleSell(entry, buybtn, buyttnText, sellbtn, localUserID);
                    Tools.RefreshHierarchy(newIcon);
                });
                buybtn.onClick.AddListener(() =>
                {
                    HandlePurchase(entry, buybtn, sellbtn, localUserID);
                    Tools.RefreshHierarchy(newIcon);
                });
                bool alreadyBought = BazookaManager.Instance.GetCustomBirdIconData().Data.Any(d => d.UUID == entry.UUID);
                if (alreadyBought)
                {
                    buybtn.interactable = false;
                    if (localUserID != entry.CreatorUserID)
                    {
                        buyttnText.text = "Purchased";
                        sellbtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        buyttnText.text = "Claimed";
                    }
                }
                else
                {
                    if (localUserID != entry.CreatorUserID)
                    {
                        buyttnText.text = "Purchase";
                    }
                    else
                    {
                        buyttnText.text = "Claim";
                    }
                }

                newIcon.SetActive(true);
                Tools.RefreshHierarchy(newIcon);
            }
        }
        refreshButton.interactable = true;
        optionsButton.interactable = true;
        backButton.interactable = true;
    }

    void HandlePurchase(MarketplaceIconType data, Button button, Button sellButton, BigInteger? localUserID)
    {
        button.interactable = false;
        MarketplaceIconStorageType marketplaceIconStorage = BazookaManager.Instance.GetCustomBirdIconData();
        if (localUserID != data.CreatorUserID)
        {
            if (data.Price > marketplaceIconStorage.Balance)
            {
                button.interactable = true;
                ShowStatus("You can't afford this icon! You need " + Tools.FormatWithCommas(data.Price - marketplaceIconStorage.Balance) + " more coins");
                return;
            }
            marketplaceIconStorage.Balance -= data.Price;
            iconPurchaseSound.Stop();
            iconPurchaseSound.Play();
        }
        var list = marketplaceIconStorage.Data.ToList();
        list.Add(data);
        marketplaceIconStorage.Data = list.ToArray();
        if (localUserID != data.CreatorUserID)
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Purchased";
            sellButton.gameObject.SetActive(true);
        }
        else
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Claimed";
        }
        balanceText.text = "You have " + Tools.FormatWithCommas(marketplaceIconStorage.Balance) + " coins to spend";
        BazookaManager.Instance.SetCustomBirdIconData(marketplaceIconStorage);
    }

    void HandleSell(MarketplaceIconType data, Button buyBtn, TMP_Text buyttnText, Button sellButton, BigInteger? localUserID)
    {
        MarketplaceIconStorageType marketplaceIconStorage = BazookaManager.Instance.GetCustomBirdIconData();
        var list = marketplaceIconStorage.Data.ToList();
        var owned = list.FirstOrDefault(d => d.UUID == data.UUID);
        if (owned != null)
        {
            list.Remove(owned);
            marketplaceIconStorage.Data = list.ToArray();
            marketplaceIconStorage.Balance += data.Price;
            if (data.UUID == marketplaceIconStorage.Selected) marketplaceIconStorage.Selected = null;
            iconPurchaseSound.Stop();
            iconPurchaseSound.Play();
            balanceText.text = "You have " + Tools.FormatWithCommas(marketplaceIconStorage.Balance) + " coins to spend";
            BazookaManager.Instance.SetCustomBirdIconData(marketplaceIconStorage);
            if (localUserID != data.CreatorUserID)
            {
                buyttnText.text = "Purchase";
            }
            else
            {
                buyttnText.text = "Claim";
            }
            buyBtn.interactable = true;
            sellButton.gameObject.SetActive(false);
        }
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

        yield return _waitForSeconds2;

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
