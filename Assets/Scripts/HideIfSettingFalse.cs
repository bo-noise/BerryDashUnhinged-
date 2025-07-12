using UnityEngine;

public class HideIfSettingFalse : MonoBehaviour {
    public BazookaSetting setting;
    public bool reverse;

    void Start() {
        bool value = GetSettingValue(setting);
        gameObject.SetActive(value == !reverse);
    }

    bool GetSettingValue(BazookaSetting s) {
        var b = BazookaManager.Instance;
        return s switch {
            BazookaSetting.FullScreen => b.GetSettingFullScreen() ?? false,
            BazookaSetting.ShowFPS => b.GetSettingShowFPS(),
            BazookaSetting.Vsync => b.GetSettingVsync() ?? false,
            BazookaSetting.HideSocials => b.GetSettingHideSocials() ?? false,
            _ => false
        };
    }
}