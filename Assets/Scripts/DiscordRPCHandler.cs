using DiscordRPC;
using UnityEngine;

public class DiscordRPCHandler : MonoBehaviour
{
    public static DiscordRPCHandler Instance;
    public DiscordRpcClient client;
    private readonly Timestamps timestamp = Timestamps.Now;

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

    public void UpdateRPC(string details, string state)
    {
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
    }
}
