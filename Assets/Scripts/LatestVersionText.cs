using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LatestVersionText : MonoBehaviour
{
    public static LatestVersionText Instance;
    public TMP_Text text;
    public Button updateButton;
    private string latest = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (updateButton != null)
        {
            updateButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://berrydash.lncvrt.xyz/download");
            });
        }
        RefreshText();
        if (latest == null) GetLatestVersion();
    }

    void OnEnable()
    {
        RefreshText();
    }

    async void GetLatestVersion()
    {
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getLatestVersion.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            latest = "-1";
        }
        else
        {
            latest = request.downloadHandler.text;
        }
        RefreshText();
    }

    internal void RefreshText()
    {
        if (text == null || updateButton == null) return;
        if (latest == null)
            text.text = "Latest: Loading...";
        else if (latest == "-1")
            text.text = "Latest: N/A";
        else
            text.text = "Latest: v" + latest;
        updateButton.gameObject.SetActive(latest != Application.version);
    }
}