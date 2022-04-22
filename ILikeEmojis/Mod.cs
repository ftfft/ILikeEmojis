using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VRC.UI.Elements.Menus;

namespace ILikeEmojis
{
    public class Mod : MelonMod
    {
        public static MelonPreferences_Category Category;
        public static MelonPreferences_Entry<bool> EnableEmojiButton;
        public static MelonPreferences_Entry<bool> DisableInteractionPauseButton;

        private static GameObject emojiButton;
        private static GameObject InteractionPauseButton;
        public override void OnApplicationStart() {
            Category = MelonPreferences.CreateCategory("ILikeEmojis");
            EnableEmojiButton = Category.CreateEntry("EnableEmojiButton", false);
            DisableInteractionPauseButton = Category.CreateEntry("DisableInteractionPauseButton", false);
            MelonCoroutines.Start(WaitForUiManagerInit());
        }

        private IEnumerator WaitForUiManagerInit() {
            while (VRCUiManager.prop_VRCUiManager_0 == null) yield return null;
            GameObject rd = SearchNonActive(GameObject.Find("UserInterface"), "Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/");
            GameObject ro = SearchNonActive(rd, "ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/");
            emojiButton = SearchNonActive(ro, "Button_Emojis");
            InteractionPauseButton = SearchNonActive(ro, " Button_InteractionPauseWithState");
            InteractionPauseButton.active = !DisableInteractionPauseButton.Value;
            EnableEmojiButton.OnValueChanged += (bool o, bool n) => emojiButton.active = n;
            DisableInteractionPauseButton.OnValueChanged += (bool o, bool n) => InteractionPauseButton.active = !n;
            LaunchPadQMMenu lm = rd.GetComponent<LaunchPadQMMenu>();
            while (lm.field_Public_Button_6?.onClick?.m_Calls?.Count == 0) yield return null;
            lm.field_Public_Button_6 = null;
            emojiButton.active = EnableEmojiButton.Value;
        }

        private GameObject SearchNonActive(GameObject root, string path) => Transform.FindRelativeTransformWithPath(root.transform, path, false).gameObject;
    }
}