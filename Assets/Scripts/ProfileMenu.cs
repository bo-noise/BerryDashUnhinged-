
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileMenu : MonoBehaviour
{
    public Button exitButton;
    public Button refreshButton;
    public Button postButton;

    public GameObject contentPanel;
    public TMP_Text loadingText;
    public GameObject voteOverlay;
    public GameObject postOverlay;

    public TMP_Text normalBerryStat;
    public TMP_Text poisonBerryStat;
    public TMP_Text slowBerryStat;
    public TMP_Text ultraBerryStat;
    public TMP_Text speedyBerryStat;
    public TMP_Text coinBerryStat;
    public TMP_Text randomBerryStat;
    public TMP_Text antiBerryStat;
    public TMP_Text coinStat;
    public TMP_Text playerNameText;
    public Image playerIconImage;
    public Image playerOverlayImage;
    public GameObject postSample;

    public bool canVote = true;


    void Awake()
    {
        exitButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public async Task Init(
        string normalBerries,
        string poisonBerries,
        string slowBerries,
        string ultraBerries,
        string speedyBerries,
        string coinBerries,
        string randomBerries,
        string antiBerries,
        string coins,
        string playerName,
        BigInteger playerID,
        int playerIcon,
        int playerOverlay,
        string customIcon,
        Color playerIconColor,
        Color playerOverlayColor
    )
    {
        normalBerryStat.text = normalBerries;
        poisonBerryStat.text = poisonBerries;
        slowBerryStat.text = slowBerries;
        ultraBerryStat.text = ultraBerries;
        speedyBerryStat.text = speedyBerries;
        coinBerryStat.text = coinBerries;
        randomBerryStat.text = randomBerries;
        antiBerryStat.text = antiBerries;
        coinStat.text = coins;
        playerNameText.text = playerName;

        if (customIcon == null)
        {
            playerIconImage.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + playerIcon);
            if (playerIcon == 1)
            {
                playerIconImage.sprite = Tools.GetIconForUser(playerID);
            }
            playerOverlayImage.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + playerOverlay);
            if (playerOverlay == 0)
            {
                playerOverlayImage.gameObject.SetActive(false);
            }
            else if (playerOverlay == 8)
            {
                playerOverlayImage.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
            }
            else if (playerOverlay == 11)
            {
                playerOverlayImage.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
            }
            else if (playerOverlay == 13)
            {
                playerOverlayImage.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
            }
            playerIconImage.color = playerIconColor;
            playerOverlayImage.color = playerOverlayColor;
        }
        else
        {
            Tools.RenderFromBase64(customIcon, playerIconImage);
            playerOverlayImage.gameObject.SetActive(false);
        }
        if (BazookaManager.Instance.GetAccountID() == playerID)
        {
            postButton.gameObject.SetActive(true);
            postButton.onClick.AddListener(UploadPostPopup);
        }

        contentPanel.SetActive(true);
        loadingText.gameObject.SetActive(false);

        Tools.RefreshHierarchy(gameObject);
        refreshButton.onClick.AddListener(async () => await RefreshPosts(playerID, playerName));
        await RefreshPosts(playerID, playerName);
    }

    public async Task Init(BigInteger playerID)
    {
        WWWForm dataForm = new();
        dataForm.AddField("uesrId", playerID.ToString());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/getAccountProfile.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = request.downloadHandler.text;
        var jsonResponse = JObject.Parse(response);
        if ((bool)jsonResponse["success"])
        {
            var totalNormalBerries = (string)jsonResponse["totalNormalBerries"];
            var totalPoisonBerries = (string)jsonResponse["totalPoisonBerries"];
            var totalSlowBerries = (string)jsonResponse["totalSlowBerries"];
            var totalUltraBerries = (string)jsonResponse["totalUltraBerries"];
            var totalSpeedyBerries = (string)jsonResponse["totalSpeedyBerries"];
            var totalCoinBerries = (string)jsonResponse["totalCoinBerries"];
            var totalRandomBerries = (string)jsonResponse["totalRandomBerries"];
            var totalAntiBerries = (string)jsonResponse["totalAntiBerries"];
            var coins = (string)jsonResponse["coins"];
            var name = (string)jsonResponse["name"];
            var icon = (int)jsonResponse["icon"];
            var overlay = (int)jsonResponse["overlay"];
            var customIcon = (string)jsonResponse["customIcon"];
            var playerIconColorArray = JArray.Parse(jsonResponse["playerIconColor"].ToString());
            var playerIconColor = new Color((int)playerIconColorArray[0] / 255f, (int)playerIconColorArray[1] / 255f, (int)playerIconColorArray[2] / 255f);
            var playerOverlayColorArray = JArray.Parse(jsonResponse["playerOverlayColor"].ToString());
            var playerOverlayColor = new Color((int)playerOverlayColorArray[0] / 255f, (int)playerOverlayColorArray[1] / 255f, (int)playerOverlayColorArray[2] / 255f);
            await Init(
                totalNormalBerries,
                totalPoisonBerries,
                totalSlowBerries,
                totalUltraBerries,
                totalSpeedyBerries,
                totalCoinBerries,
                totalRandomBerries,
                totalAntiBerries,
                coins,
                name,
                playerID,
                icon,
                overlay,
                customIcon,
                playerIconColor,
                playerOverlayColor
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async Task RefreshPosts(BigInteger playerID, string playerName)
    {
        exitButton.interactable = false;
        refreshButton.interactable = false;
        postButton.interactable = false;
        foreach (Transform post in postSample.transform.parent)
        {
            if (post.gameObject.activeSelf)
            {
                Destroy(post.gameObject);
            }
        }
        WWWForm dataForm = new();
        dataForm.AddField("targetId", playerID.ToString());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/getAccountProfileMessages.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = request.downloadHandler.text;
        var posts = JArray.Parse(response).ToObject<ProfileMessageResponse[]>();
        foreach (var post in posts)
        {
            var entry = Instantiate(postSample, postSample.transform.parent);
            var entryUsername = entry.transform.GetChild(0).GetComponent<TMP_Text>();
            var entryLikesParent = entry.transform.GetChild(1);
            var entryLikesTexture = entryLikesParent.GetChild(1).GetComponent<TMP_Text>();
            var entryLikesButton = entryLikesParent.GetChild(1).GetComponent<Button>();
            var entryLikesCount = entryLikesParent.GetChild(2).GetComponent<TMP_Text>();
            var entryMessage = entry.transform.GetChild(2).GetComponent<TMP_Text>();
            var entryTimestamp = entry.transform.GetChild(3).GetComponent<TMP_Text>();
            var entryDeleteButton = entryLikesParent.GetChild(0).GetComponent<Button>();

            entryUsername.text = playerName;
            entryLikesTexture.text = post.Likes < 0 ? "\\uf165" : "\\uf164";
            entryLikesTexture.color = post.Likes < 0 ? new Color(1f, 0f, 0.5f) : new Color(1f, 1f, 0f);
            entryLikesCount.text = Tools.FormatWithCommas(post.Likes);
            entryLikesButton.onClick.AddListener(() =>
            {
                if (canVote && BazookaManager.Instance.GetAccountID() != null && BazookaManager.Instance.GetAccountID() != post.UserID)
                {
                    var likedPosts = BazookaManager.Instance
                        .GetLikedPosts()
                        .Select(x => x.ToString())
                        .ToList();
                    if (!likedPosts.Any(x => x.ToString() == post.ID.ToString())) VotePost(post.ID, entryLikesCount, entryLikesTexture);
                }
            });
            entryMessage.text = Encoding.UTF8.GetString(Convert.FromBase64String(post.Content));
            entryTimestamp.text = post.Timestamp.ToString();
            if (BazookaManager.Instance.GetAccountID() == post.UserID)
            {
                entryDeleteButton.gameObject.SetActive(true);
                entryDeleteButton.onClick.AddListener(async () => await DeletePost(entry, post.ID));
            }

            entry.name = "PostEntry_" + post.ID;
            entry.SetActive(true);

            Tools.RefreshHierarchy(entry);
        }
        exitButton.interactable = true;
        refreshButton.interactable = true;
        postButton.interactable = true;
    }

    void VotePost(BigInteger postId, TMP_Text entryLikesCount, TMP_Text entryLikesTexture)
    {
        var popup = Instantiate(voteOverlay, voteOverlay.transform.parent);
        popup.SetActive(true);
        var exitButton = popup.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        var likeButton = popup.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        var dislikeButton = popup.transform.GetChild(0).GetChild(3).GetComponent<Button>();
        exitButton.onClick.AddListener(() => Destroy(popup));
        likeButton.onClick.AddListener(async () =>
        {
            canVote = false;
            Destroy(popup);
            await SendPostVote(postId, true, entryLikesCount, entryLikesTexture);
            canVote = true;
        });
        dislikeButton.onClick.AddListener(async () =>
        {
            canVote = false;
            Destroy(popup);
            await SendPostVote(postId, false, entryLikesCount, entryLikesTexture);
            canVote = true;
        });
    }

    async Task SendPostVote(BigInteger postId, bool liked, TMP_Text entryLikesCount, TMP_Text entryLikesTexture)
    {
        WWWForm dataForm = new();
        dataForm.AddField("targetId", postId.ToString());
        dataForm.AddField("liked", liked ? "1" : "0");
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/voteAccountProfileMessage.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = request.downloadHandler.text;
        try
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BigInteger likes = BigInteger.Parse((string)jsonResponse["likes"]);
                entryLikesCount.text = Tools.FormatWithCommas(likes);
                entryLikesTexture.text = likes < 0 ? "\\uf165" : "\\uf164";
                entryLikesTexture.color = likes < 0 ? new Color(1f, 0f, 0.5f) : new Color(1f, 1f, 0f);
                var likedPosts = BazookaManager.Instance.GetLikedPosts();
                likedPosts.Add(postId.ToString());
                BazookaManager.Instance.SetLikedPosts(likedPosts);
            }
            else
            {
                if ((string)jsonResponse["message"] == "You have already voted")
                {
                    var likedPosts = BazookaManager.Instance.GetLikedPosts();
                    likedPosts.Add(postId.ToString());
                    BazookaManager.Instance.SetLikedPosts(likedPosts);
                }
            }
        }
        catch (Exception)
        {
            Debug.LogError("Failed to proccess vote");
        }
    }

    void UploadPostPopup()
    {
        var popup = Instantiate(postOverlay, postOverlay.transform.parent);
        popup.SetActive(true);
        var inputBox = popup.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>();
        var cancelButton = popup.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        var submitButton = popup.transform.GetChild(0).GetChild(3).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => Destroy(popup));
        submitButton.onClick.AddListener(async () =>
        {
            if (inputBox.text.Trim().Length == 0) return;
            Destroy(popup);
            await UploadPost(inputBox.text);
        });
    }

    async Task UploadPost(string message)
    {
        WWWForm dataForm = new();
        dataForm.AddField("content", message);
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/uploadAccountProfileMessage.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = request.downloadHandler.text;
        try
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                await RefreshPosts(BazookaManager.Instance.GetAccountID() ?? 0, BazookaManager.Instance.GetAccountName());
            }
        }
        catch (Exception)
        {
            Debug.LogError("Failed to upload post");
        }
    }

    async Task DeletePost(GameObject entryObject, BigInteger targetId)
    {
        WWWForm dataForm = new();
        dataForm.AddField("targetId", targetId.ToString());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "berrydash/deleteAccountProfileMessage.php", dataForm);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = request.downloadHandler.text;
        try
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                Destroy(entryObject);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Failed to upload post");
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) Destroy(gameObject);
    }
}