using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds7 = new WaitForSeconds(7);
    private static WaitForSeconds _waitForSeconds3 = new WaitForSeconds(3);
    public GameObject selectionMenu;
    public GameObject customMenu;
    public Button customButton;
    public Button customBackButton;
    public Button customPlayButton;

    public TMP_InputField normalBerryChance;
    public TMP_InputField poisonBerryChance;
    public TMP_InputField slowBerryChance;
    public TMP_InputField ultraBerryChance;
    public TMP_InputField speedyBerryChance;
    public TMP_InputField randomBerryChance;
    public TMP_InputField antiBerryChance;

    public TMP_Text validateTotalText;

    public Image jumpscareImage;
    public AudioSource jumpscareAudio;
    public Button jumpscareButton;

    void Awake()
    {
        customButton.onClick.AddListener(() =>
        {
            selectionMenu.SetActive(false);
            customMenu.SetActive(true);
        });
        customBackButton.onClick.AddListener(() =>
        {
            customMenu.SetActive(false);
            selectionMenu.SetActive(true);

            normalBerryChance.text = "47.5%";
            poisonBerryChance.text = "12.5%";
            slowBerryChance.text = "10%";
            ultraBerryChance.text = "10%";
            speedyBerryChance.text = "10%";
            randomBerryChance.text = "5%";
            antiBerryChance.text = "5%";
            ValidateTotal();
        });
        customPlayButton.onClick.AddListener(async () =>
        {
            GameObject obj = new("CustomGameTempData");
            obj.AddComponent<CustomGameTempData>();
            CustomGameTempData customGameTempData = obj.GetComponent<CustomGameTempData>();
            customGameTempData.normalBerryChance = float.Parse(normalBerryChance.text.Replace("%", "").Trim());
            customGameTempData.poisonBerryChance = float.Parse(poisonBerryChance.text.Replace("%", "").Trim());
            customGameTempData.slowBerryChance = float.Parse(slowBerryChance.text.Replace("%", "").Trim());
            customGameTempData.ultraBerryChance = float.Parse(ultraBerryChance.text.Replace("%", "").Trim());
            customGameTempData.speedyBerryChance = float.Parse(speedyBerryChance.text.Replace("%", "").Trim());
            customGameTempData.randomBerryChance = float.Parse(randomBerryChance.text.Replace("%", "").Trim());
            customGameTempData.antiBerryChance = float.Parse(antiBerryChance.text.Replace("%", "").Trim());
            await SceneManager.LoadSceneAsync("CustomGamePlayer");
        });

        normalBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            normalBerryChance.text = value.Replace("%", "");
        });
        normalBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            normalBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        poisonBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            poisonBerryChance.text = value.Replace("%", "");
        });
        poisonBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            poisonBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        slowBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            slowBerryChance.text = value.Replace("%", "");
        });
        slowBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            slowBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        ultraBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            ultraBerryChance.text = value.Replace("%", "");
        });
        ultraBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            ultraBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        speedyBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            speedyBerryChance.text = value.Replace("%", "");
        });
        speedyBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            speedyBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        randomBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            randomBerryChance.text = value.Replace("%", "");
        });
        randomBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            randomBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        antiBerryChance.onSelect.AddListener((value) =>
        {
            validateTotalText.gameObject.SetActive(false);
            customBackButton.interactable = false;
            customPlayButton.interactable = false;
            antiBerryChance.text = value.Replace("%", "");
        });
        antiBerryChance.onDeselect.AddListener((value) =>
        {
            if (float.TryParse(value, out var value2) && value2 < 0f)
            {
                value = "0";
            }
            antiBerryChance.text = value + "%";
            customBackButton.interactable = true;
            ValidateTotal();
        });
        jumpscareButton.onClick.AddListener(() =>
        {
            jumpscareButton.GetComponent<Image>().color = Color.red;
            jumpscareButton.onClick.RemoveAllListeners();
            StartCoroutine(Jumpscare());
        });
    }

    void ValidateTotal()
    {
        customPlayButton.interactable = false;
        float total = 0f;
        try
        {
            total += float.Parse(normalBerryChance.text.Replace("%", ""));
            total += float.Parse(poisonBerryChance.text.Replace("%", ""));
            total += float.Parse(slowBerryChance.text.Replace("%", ""));
            total += float.Parse(ultraBerryChance.text.Replace("%", ""));
            total += float.Parse(speedyBerryChance.text.Replace("%", ""));
            total += float.Parse(randomBerryChance.text.Replace("%", ""));
            total += float.Parse(antiBerryChance.text.Replace("%", ""));
        }
        catch (Exception)
        {
            validateTotalText.text = "Failed to parse total";
            validateTotalText.gameObject.SetActive(true);
            return;
        }
        if (total == 100f)
        {
            customPlayButton.interactable = true;
            validateTotalText.gameObject.SetActive(false);
        }
        else
        {
            validateTotalText.text = "Total must add up to 100%!";
            validateTotalText.gameObject.SetActive(true);
        }
    }

    IEnumerator Jumpscare()
    {
        jumpscareAudio.Play();
        float t = 0;
        jumpscareImage.gameObject.SetActive(true);
        jumpscareImage.rectTransform.localScale = Vector3.zero;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            float p = t / 0.25f;
            jumpscareImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, p);
            jumpscareImage.color = Color.Lerp(Color.white, Color.red, p);
            yield return null;
        }
        jumpscareImage.rectTransform.localScale = Vector3.one;
        yield return _waitForSeconds3;
        jumpscareAudio.Stop();
        jumpscareImage.gameObject.SetActive(false);
        jumpscareImage.rectTransform.localScale = Vector3.zero;
        jumpscareImage.color = Color.white;
        yield return _waitForSeconds7;
        jumpscareButton.GetComponent<Image>().color = Color.white;
        jumpscareButton.onClick.AddListener(() =>
        {
            jumpscareButton.GetComponent<Image>().color = Color.red;
            jumpscareButton.onClick.RemoveAllListeners();
            StartCoroutine(Jumpscare());
        });
    }
}