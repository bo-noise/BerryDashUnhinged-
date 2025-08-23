using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatroomMenuEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image bgImg;
    private Button optionsButton;
    private bool isMadeBySelf = false;

    public void Init(Image bgImgArg, Button optionsButtonArg, bool isMadeBySelfArg)
    {
        bgImg = bgImgArg;
        optionsButton = optionsButtonArg;
        isMadeBySelf = isMadeBySelfArg;
        bgImg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        optionsButton.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e) {
        bgImg.color = new Color(60f/255f, 60f/255f, 60f/255f);
        optionsButton.gameObject.SetActive(isMadeBySelf);
    }

    public void OnPointerExit(PointerEventData e) {
        bgImg.color = new Color(50f/255f, 50f/255f, 50f/255f);
        optionsButton.gameObject.SetActive(false);
    }
}
