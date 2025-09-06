
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomColorObject : MonoBehaviour
{
    public ColorObjectType type;
    public bool invert;

    public void SetColor()
    {
        switch (type)
        {
            case ColorObjectType.InGameBackgroundColor:
                JArray color = BazookaManager.Instance.GetColorSettingBackground();
                Color colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                if (invert) colorType = new(1f - colorType.r, 1f - colorType.g, 1f - colorType.b);
                gameObject.GetComponent<Camera>().backgroundColor = colorType;
                break;
            case ColorObjectType.MenuBackgroundColor:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                if (invert) colorType = new(1f - colorType.r, 1f - colorType.g, 1f - colorType.b);
                gameObject.GetComponent<Camera>().backgroundColor = colorType;
                break;
            case ColorObjectType.ButtonColor:
                color = BazookaManager.Instance.GetColorSettingButton();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                if (invert) colorType = new(1f - colorType.r, 1f - colorType.g, 1f - colorType.b);
                gameObject.GetComponent<Image>().color = colorType;
                break;
            case ColorObjectType.TextColor:
                color = BazookaManager.Instance.GetColorSettingText();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                if (invert) colorType = new(1f - colorType.r, 1f - colorType.g, 1f - colorType.b);
                gameObject.GetComponent<TMP_Text>().color = colorType;
                break;
        }
    }

    void Start()
    {
        SetColor();
    }
}