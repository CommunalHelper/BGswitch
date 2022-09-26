using System;

namespace Celeste.Mod.BGswitch {
    public class BGswitch : EverestModule {
        public static BGswitch Instance;

        public BGswitch() {
            Instance = this;
        }

        public static BGswitchSession Session => (BGswitchSession)Instance._Session;
        public override Type SessionType => typeof(BGswitchSession);

        public override void Load() {
            BGswitchInterop.Load();
            BGModeManager.Load();
        }

        public override void Unload() {
            BGModeManager.Unload();
        }
    }
}
