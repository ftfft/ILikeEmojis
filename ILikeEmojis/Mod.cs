using MelonLoader;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRC.UI.Elements.Menus;
using System.Linq;
using UnityEngine.SceneManagement;

namespace ILikeEmojis
{
    public class Mod : MelonMod
    {
        public static MelonPreferences_Category Category;
        public static MelonPreferences_Entry<bool> EnableEmojiButton;
        public static MelonPreferences_Entry<bool> DisableInteractionPauseButton;

        private static GameObject emojiButton;
        private static GameObject InteractionPauseButton;

        public override void OnApplicationStart()
        {
            Category = MelonPreferences.CreateCategory("ILikeEmojis");
            EnableEmojiButton = Category.CreateEntry("EnableEmojiButton", true);
            DisableInteractionPauseButton = Category.CreateEntry("DisableInteractionPauseButton", true);



            MelonCoroutines.Start(WaitForUiManagerInit());


        }

        private IEnumerator WaitForUiManagerInit()
        {
            yield return new WaitForEndOfFrame();

            List<GameObject> prefabs = Prefabs();

            GameObject qm_prefab = prefabs.First((ob) => ob.name == "Canvas_QuickMenu");

            GameObject qmw = SearchNonActive(qm_prefab, "Container/Window");
            GameObject qmp = SearchNonActive(qmw, "QMParent");
            
            Sprite ei = SearchNonActive(qmw, "Wing_Left/Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup/Button_Emoji/Container/Icon").GetComponent<UnityEngine.UI.Image>().sprite;

            SearchNonActive(qmp, "Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Emojis/Icon").GetComponent<UnityEngine.UI.Image>().sprite = ei;

            Sprite ai = SearchNonActive(qmp, "Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_WorldActions/Arrow").GetComponent<UnityEngine.UI.Image>().sprite;
            GameObject qmgf_prefab = prefabs.First((ob) => ob.name == "QM_GridFoldout");


            SearchNonActive(qmgf_prefab, "Arrow").GetComponent<UnityEngine.UI.Image>().sprite = ai;


            while (VRCUiManager.prop_VRCUiManager_0 == null) yield return null;
            GameObject rw = SearchNonActive(GameObject.Find("UserInterface"), "Canvas_QuickMenu(Clone)/Container/Window/");
            GameObject rp = SearchNonActive(rw, "QMParent");
            GameObject rd = SearchNonActive(rp, "Menu_Dashboard");
            GameObject ro = SearchNonActive(rd, "ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/");
            emojiButton = SearchNonActive(ro, "Button_Emojis");
            InteractionPauseButton = SearchNonActive(ro, " Button_InteractionPauseWithState");
            InteractionPauseButton.active = !DisableInteractionPauseButton.Value;
            EnableEmojiButton.OnValueChanged += (bool o, bool n) => emojiButton.active = n;
            DisableInteractionPauseButton.OnValueChanged += (bool o, bool n) => InteractionPauseButton.active = !n;

            LaunchPadQMMenu lm = rd.GetComponent<LaunchPadQMMenu>();
            while (lm.field_Public_Button_6?.onClick?.m_Calls?.Count == 0) yield return null; // wait for the button to get its onClick assigned
            lm.field_Public_Button_6 = null;
            emojiButton.active = EnableEmojiButton.Value;
        }

        private GameObject SearchNonActive(GameObject root, string path) => Transform.FindRelativeTransformWithPath(root.transform, path, false).gameObject;

        private List<GameObject> Prefabs()
        {
            List<GameObject> ret = new List<GameObject>();
            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.transform.parent == null && !go.scene.IsValid() && (go.name == "Canvas_QuickMenu" || go.name == "QM_GridFoldout"))
                    ret.Add(go);
            }
            return ret;
        }
    }
}