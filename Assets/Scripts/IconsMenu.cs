using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Iconsmenu : MonoBehaviour
{
    public GameObject iconsPanel;
    public GameObject overlaysPanel;
    public Button backButton;
    public Sprite defaultIcon;
    public Button placeholderButton;
    public TMP_Text selectionText;
    public Image previewBird;
    public Image previewOverlay;
    public Button icon1;
    public Button icon2;
    public Button icon3;
    public Button icon4;
    public Button icon5;
    public Button icon6;
    public Button icon7;
    public Button icon8;
    public Button overlay0;
    public Button overlay1;
    public Button overlay2;
    public Button overlay3;
    public Button overlay4;
    public Button overlay5;
    public Button overlay6;
    public Button overlay7;
    public Button overlay8;
    public Button overlay9;
    public Button overlay10;
    public Button overlay11;
    public Button overlay12;
    public Button overlay13;
    public Button overlay14;
    public GameObject previewBirdObject;
    public ColorPanel iconColorPanel;
    public ColorPanel overlayColorPanel;

    private void Start()
    {
        iconColorPanel.Init(BazookaManager.Instance.GetColorSettingIcon(), Color.white);
        iconColorPanel.OnColorChanged += color =>
        {
            BazookaManager.Instance.SetColorSettingIcon(color);
        };
        overlayColorPanel.Init(BazookaManager.Instance.GetColorSettingOverlay(), Color.white);
        overlayColorPanel.OnColorChanged += color =>
        {
            BazookaManager.Instance.SetColorSettingOverlay(color);
        };

        defaultIcon = Tools.GetIconForUser(BazookaManager.Instance.GetAccountID() ?? 0);
        icon1.transform.GetChild(0).GetComponent<Image>().sprite = defaultIcon;
        SwitchToIcon();
        SelectOverlay(BazookaManager.Instance.GetBirdOverlay());
        SelectIcon(BazookaManager.Instance.GetBirdIcon());
        if (BazookaManager.Instance.GetBirdIcon() == 7)
        {
            SelectOverlay(0);
            placeholderButton.interactable = false;
        }
        placeholderButton.onClick.AddListener(ToggleKit);
        backButton.onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        });
        previewBird.GetComponentInParent<Button>().onClick.AddListener(() =>
        {
            var scale = previewBird.transform.localScale;
            if (scale.x == -1)
            {
                scale.x = 1;
            }
            else
            {
                scale.x = -1;
            }
            previewBird.transform.localScale = scale;
        });
        icon1.onClick.AddListener(() => SelectIcon(1));
        icon2.onClick.AddListener(() => SelectIcon(2));
        icon3.onClick.AddListener(() => SelectIcon(3));
        icon4.onClick.AddListener(() => SelectIcon(4));
        icon5.onClick.AddListener(() => SelectIcon(5));
        icon6.onClick.AddListener(() => SelectIcon(6));
        icon7.onClick.AddListener(() => SelectIcon(7));
        icon8.onClick.AddListener(() => SelectIcon(8));
        overlay0.onClick.AddListener(() => SelectOverlay(0));
        overlay1.onClick.AddListener(() => SelectOverlay(1));
        overlay2.onClick.AddListener(() => SelectOverlay(2));
        overlay3.onClick.AddListener(() => SelectOverlay(3));
        overlay4.onClick.AddListener(() => SelectOverlay(4));
        overlay5.onClick.AddListener(() => SelectOverlay(5));
        overlay6.onClick.AddListener(() => SelectOverlay(6));
        overlay7.onClick.AddListener(() => SelectOverlay(7));
        overlay8.onClick.AddListener(() => SelectOverlay(8));
        overlay9.onClick.AddListener(() => SelectOverlay(9));
        overlay10.onClick.AddListener(() => SelectOverlay(10));
        overlay11.onClick.AddListener(() => SelectOverlay(11));
        overlay12.onClick.AddListener(() => SelectOverlay(12));
        overlay13.onClick.AddListener(() => SelectOverlay(13));
        overlay14.onClick.AddListener(() => SelectOverlay(14));
    }

    private void SwitchToIcon()
    {
        iconsPanel.SetActive(true);
        overlaysPanel.SetActive(false);
        selectionText.text = "Icon selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Overlays";
    }

    private void SwitchToOverlay()
    {
        iconsPanel.SetActive(false);
        overlaysPanel.SetActive(true);
        selectionText.text = "Overlay selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Icons";
    }

    private void ToggleKit()
    {
        if (GetCurrentKit() == 1)
        {
            SwitchToOverlay();
        }
        else if (GetCurrentKit() == 2)
        {
            SwitchToIcon();
        }
    }

    private int GetCurrentKit()
    {
        if (iconsPanel.activeSelf)
        {
            return 1;
        }
        if (overlaysPanel.activeSelf)
        {
            return 2;
        }
        return 0;
    }

    private void SelectIcon(int iconID)
    {
        BazookaManager.Instance.SetBirdIcon(iconID);
        icon1.interactable = iconID != 1;
        icon2.interactable = iconID != 2;
        icon3.interactable = iconID != 3;
        icon4.interactable = iconID != 4;
        icon5.interactable = iconID != 5;
        icon6.interactable = iconID != 6;
        icon7.interactable = iconID != 7;
        icon8.interactable = iconID != 8;
        previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + iconID);
        if (iconID == 1)
        {
            previewBird.sprite = defaultIcon;
        }
        if (iconID == 7)
        {
            SelectOverlay(0, false);
            placeholderButton.interactable = false;
        }
        else
        {
            SelectOverlay(BazookaManager.Instance.GetBirdPastOverlay(), false);
            placeholderButton.interactable = true;
        }
    }

    private void SelectOverlay(int overlayID, bool savePast = true)
    {
        if (savePast)
        {
            BazookaManager.Instance.SetBirdPastOverlay(BazookaManager.Instance.GetBirdOverlay());
        }
        BazookaManager.Instance.SetBirdOverlay(overlayID);
        overlay0.interactable = overlayID != 0;
        overlay1.interactable = overlayID != 1;
        overlay2.interactable = overlayID != 2;
        overlay3.interactable = overlayID != 3;
        overlay4.interactable = overlayID != 4;
        overlay5.interactable = overlayID != 5;
        overlay6.interactable = overlayID != 6;
        overlay7.interactable = overlayID != 7;
        overlay8.interactable = !(BazookaManager.Instance.GetAccountID() == 1 && BazookaManager.Instance.GetBirdIcon() == 1) && overlayID != 8;
        overlay9.interactable = overlayID != 9;
        overlay10.interactable = overlayID != 10;
        overlay11.interactable = overlayID != 11;
        overlay12.interactable = overlayID != 12;
        overlay13.interactable = overlayID != 13;
        overlay14.interactable = overlayID != 14;
        previewOverlay.rectTransform.localPosition = new Vector3(-32f, 44.50001f, 0f);
        previewOverlay.gameObject.SetActive(true);
        if (overlayID == 8)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-35.36f, 31.6f, 0f);
        }
        else if (overlayID == 11)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-31.44f, 43.50004f, 0f);
        }
        else if (overlayID == 13)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-35.28575f, 31.3667f, 0f);
        }

        if (overlayID == 0)
        {
            previewOverlay.gameObject.SetActive(false);
            previewOverlay.sprite = null;
        }
        else
        {
            previewOverlay.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlayID);
        }
    }
}