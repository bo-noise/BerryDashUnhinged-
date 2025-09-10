
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileMenu : MonoBehaviour
{
    public Button exitButton;
    public Button refreshButton;
    public Button postButton;

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

        Tools.RefreshHierarchy(gameObject);
        refreshButton.onClick.AddListener(async () => await RefreshPosts(playerID, playerName));
        await RefreshPosts(playerID, playerName);
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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("targetId", playerID.ToString());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getAccountProfileMessages.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
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
                if (canVote && BazookaManager.Instance.GetAccountID() != post.UserID)
                {
                    var likedPosts = BazookaManager.Instance
                        .GetLikedPosts()
                        .Select(x => x.ToString())
                        .ToList();
                    if (!likedPosts.Any(x => x.ToString() == post.ID.ToString())) VotePost(post.ID, entryLikesCount);
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

    void VotePost(BigInteger postId, TMP_Text entryLikesCount)
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
            await SendPostVote(postId, true, entryLikesCount);
            canVote = true;
        });
        dislikeButton.onClick.AddListener(async () =>
        {
            canVote = false;
            Destroy(popup);
            await SendPostVote(postId, false, entryLikesCount);
            canVote = true;
        });
    }

    async Task SendPostVote(BigInteger postId, bool liked, TMP_Text entryLikesCount)
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("targetId", postId.ToString());
        dataForm.AddField("liked", liked ? "1" : "0");
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "voteAccountProfileMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        try
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                entryLikesCount.text = Tools.FormatWithCommas((string)jsonResponse["likes"]);
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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("content", message);
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "uploadAccountProfileMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("targetId", targetId.ToString());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "deleteAccountProfileMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
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
}