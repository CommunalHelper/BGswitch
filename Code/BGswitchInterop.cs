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

        public static bool IsBGMode() => BGModeManager.BGMode;

        [Command("bg_mode", "[BGswitch] Sets the bg mode of the level")]
        public static void SetBGMode(bool bgMode = false, bool persistent = false) {
            if (Engine.Scene is Level) {
                BGModeManager.BGMode = bgMode;
                if (persistent) {
                    BGswitch.Session.BGMode = bgMode;
                }
            }
        }

        public static Component GetBGModeListener(Action<bool> action) {
            return new BGModeListener(action);
        }
    }

    [ModImportName("SpeedrunTool.SaveLoad")]
    public static class SpeedrunToolImports {
        public static Action<Type, string[]> RegisterStaticTypes;
    }
}
