using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomColorObject : MonoBehaviour
{
    public ColorObjectType type;
    public bool invert;
    public Color addMore;
    public bool reverseAdd;

    Color ApplyModifiers(Color baseColor)
    {
        if (invert) baseColor = new(1f - baseColor.r, 1f - baseColor.g, 1f - baseColor.b);

        if (reverseAdd)
        {
            baseColor.r = Mathf.Clamp01(baseColor.r - addMore.r);
            baseColor.g = Mathf.Clamp01(baseColor.g - addMore.g);
            baseColor.b = Mathf.Clamp01(baseColor.b - addMore.b);
        }
        else
        {
            baseColor.r = Mathf.Clamp01(baseColor.r + addMore.r);
            baseColor.g = Mathf.Clamp01(baseColor.g + addMore.g);
            baseColor.b = Mathf.Clamp01(baseColor.b + addMore.b);
        }

        return baseColor;
    }

    public void SetColor()
    {
        JArray color = null;
        Color colorType = Color.white;

        switch (type)
        {
            case ColorObjectType.InGameBackgroundColor:
                color = BazookaManager.Instance.GetColorSettingBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<Camera>().backgroundColor = colorType;
                break;
            case ColorObjectType.MenuBackgroundColor:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<Camera>().backgroundColor = colorType;
                break;
            case ColorObjectType.MenuBackgroundColorImage:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<Image>().color = colorType;
                break;
            case ColorObjectType.ButtonColor:
                color = BazookaManager.Instance.GetColorSettingButton();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<Image>().color = colorType;
                break;
            case ColorObjectType.ButtonColorText:
                color = BazookaManager.Instance.GetColorSettingButton();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<TMP_Text>().color = colorType;
                break;
            case ColorObjectType.TextColor:
                color = BazookaManager.Instance.GetColorSettingText();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<TMP_Text>().color = colorType;
                break;
            case ColorObjectType.TextColorImage:
                color = BazookaManager.Instance.GetColorSettingText();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                gameObject.GetComponent<Image>().color = colorType;
                break;
        }
    }

    void Start()
    {
        SetColor();
    }
}