using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconMarketplaceManager : MonoBehaviour
{
    public GameObject normalPanel;
    public GameObject downloadPanel;
    public GameObject uploadPanel;

    public IconMarketplaceUploadIcon uploadPanelScript;
    public IconMarketplaceDownloadIcon downloadPanelScript;

    public Button downloadButton;
    public Button uploadButton;

    void Start()
    {
        downloadButton.onClick.AddListener(() => SwitchPanel(1));
#if !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        uploadButton.interactable = false;
        uploadButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 10, 0);
        uploadButton.transform.GetChild(1).GetComponent<TMP_Text>().text = "Unsupported platform";
#else
        if (BazookaManager.Instance.GetAccountID() != null && BazookaManager.Instance.GetAccountName() != null && BazookaManager.Instance.GetAccountSession() != null)
        {
            uploadButton.onClick.AddListener(() => SwitchPanel(2));
        }
        else
        {
            uploadButton.interactable = false;
            uploadButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 10, 0);
            uploadButton.transform.GetChild(1).GetComponent<TMP_Text>().text = "Not logged in to an account";
        }
#endif
    }

    public void SwitchPanel(int panelIndex)
    {
        switch (panelIndex)
        {
            case 0:
                normalPanel.SetActive(true);
                downloadPanel.SetActive(false);
                uploadPanel.SetActive(false);
                break;
            case 1:
                downloadPanelScript.Load(this);
                normalPanel.SetActive(false);
                downloadPanel.SetActive(true);
                uploadPanel.SetActive(false);
                break;
            case 2:
                uploadPanelScript.Load();
                normalPanel.SetActive(false);
                downloadPanel.SetActive(false);
                uploadPanel.SetActive(true);
                break;
        }
    }
}