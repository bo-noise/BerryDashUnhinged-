using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountLoggedOut : MonoBehaviour
{
    public Button loggedOutLoginButton;
    public Button loggedOutRegisterButton;
    public Button loggedOutBackButton;
    public bool clearValues = false;

    void Awake()
    {
        loggedOutLoginButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(2));
        loggedOutRegisterButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(3));
        loggedOutBackButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
    }

    void OnEnable()
    {
        if (clearValues)
        {
            BazookaManager.Instance.UnsetAccountSession();
            BazookaManager.Instance.UnsetAccountName();
            BazookaManager.Instance.UnsetAccountID();
            BazookaManager.Instance.UnsetGameStoreHighScore();
            BazookaManager.Instance.UnsetBirdIcon();
            BazookaManager.Instance.UnsetBirdOverlay();
            BazookaManager.Instance.UnsetGameStoreTotalNormalBerries();
            BazookaManager.Instance.UnsetGameStoreTotalPoisonBerries();
            BazookaManager.Instance.UnsetGameStoreTotalSlowBerries();
            BazookaManager.Instance.UnsetGameStoreTotalUltraBerries();
            BazookaManager.Instance.UnsetGameStoreTotalSpeedyBerries();
            BazookaManager.Instance.UnsetGameStoreTotalAttepts();
            BazookaManager.Instance.UnsetColorSettingIcon();
            BazookaManager.Instance.UnsetColorSettingOverlay();
            clearValues = false;
        }
    }
}