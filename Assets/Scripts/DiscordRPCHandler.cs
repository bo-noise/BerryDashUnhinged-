using DiscordRPC;
using UnityEngine;

public class DiscordRPCHandler : MonoBehaviour
{
    public static DiscordRPCHandler Instance;
    public DiscordRpcClient client;
    private readonly Timestamps timestamp = Timestamps.Now;

    private bool isIdle = false;
    private string savedDetails;
    private string savedState;

    void Awake()
    {
        if (Application.isMobilePlatform || Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        client = new DiscordRpcClient("1421877993176961155");
        client.Initialize();
    }

    void OnApplicationQuit()
    {
        client.Dispose();
    }

    void OnApplicationPause(bool pause)
    {
        isIdle = pause;
        if (pause)
        {
            UpdateRPC("Idle", null, true);
        }
        else
        {
            UpdateRPC(savedDetails, savedState, true);
        }
    }

    public void UpdateRPC(string details, string state, bool force = false)
    {
        if (!force && isIdle)
        {
            savedDetails = details;
            savedState = state;
            return;
        }

        client.SetPresence(new RichPresence
        {
            Details = details,
            State = state,
            Assets = new Assets
            {
                LargeImageKey = "https://berrydash.lncvrt.xyz/assets/icon.png",
                LargeImageText = "Berry Dash",
                SmallImageKey = "https://cdn.lncvrt.xyz/pfp.png",
                SmallImageText = "Made by Lncvrt!"
            },
            Buttons = new[]
            {
                new Button { Label = "Website", Url = "https://berrydash.lncvrt.xyz" },
                new Button { Label = "Download", Url = "https://berrydash.lncvrt.xyz/download" }
            },
            Timestamps = timestamp
        });

        savedDetails = details;
        savedState = state;
    }
}
