using Monocle;
using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.BGswitch {
    [ModExportName("BGswitch")]
    public static class BGswitchInterop {
        internal static void Load() {
            typeof(BGswitchInterop).ModInterop();
            typeof(SpeedrunToolImports).ModInterop();
        }

        // Returns the current BG mode
        public static bool IsBGMode() => BGModeManager.BGMode;

        // Sets the BG mode
        //   bool bgMode: the value to set the BG mode to
        //   bool persistent: whether or not the value should be saved to the session
        [Command("bg_mode", "[BGswitch] Sets the bg mode of the level")]
        public static void SetBGMode(bool bgMode = false, bool persistent = false) {
            if (Engine.Scene is Level) {
                BGModeManager.BGMode = bgMode;
                if (persistent) {
                    BGswitch.Session.BGMode = bgMode;
                }
            }
        }

        // Creates and returns a BGModeListener as a Component.
        //   Action<bool> action: the delegate that will be called when the BG mode changes
        public static Component GetBGModeListener(Action<bool> action) {
            return new BGModeListener(action);
        }
    }

    [ModImportName("SpeedrunTool.SaveLoad")]
    public static class SpeedrunToolImports {
        public static Func<Type, string[], object> RegisterStaticTypes;
        public static Action<object> Unregister;
    }
}
