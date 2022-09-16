using Monocle;
using System;

namespace Celeste.Mod.BGswitch {
    public class BGswitch : EverestModule {
        public static BGswitch Instance;
        public static SpriteBank spriteBank;

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

        public override void LoadContent(bool firstLoad) {
            spriteBank = new SpriteBank(GFX.Game, "Graphics/BGSwitchSprites.xml");
        }
    }
}
