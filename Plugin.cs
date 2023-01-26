using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BugFixes
{
    [BepInPlugin("de.benediktwerner.stacklands.bugfixes", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource L;

        private void Awake()
        {
            L = Logger;
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Demon), nameof(Demon.Die))]
        public static void AllowMoreThan1DemonSwordDrop(Demon __instance)
        {
            __instance.Drops.Chances.ForEach(c => c.HasMaxCount = false);
        }

        public void Update()
        {
            if (
                Screen.currentResolution.width != OptionsScreen.CurrentWidth
                || Screen.currentResolution.height != OptionsScreen.CurrentHeight
            )
            {
                L.LogWarning("Found real resolution != options resolution. Adjusting.");
                OptionsScreen.SetResolution();
            }
        }
    }
}
