using UnityEngine;

public class DiscordRPCUpdater : MonoBehaviour
{
    public string details;
    public string state;

    void Awake()
    {
        if (DiscordRPCHandler.Instance != null) DiscordRPCHandler.Instance.UpdateRPC(details, state);
    }
}