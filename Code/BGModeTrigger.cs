using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BGswitch {
    [CustomEntity("bgSwitch/bgTrigger")]
    public class BGModeTrigger : Trigger {
        private readonly bool mode;
        private readonly bool persistent;
        private readonly bool playEffects;

        public BGModeTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {
            mode = data.Bool("mode", true);
            persistent = data.Has("persistent") ? data.Bool("persistent", false) : data.Bool("persistant", true);
            playEffects = data.Bool("playEffects", true);
        }

        public override void OnEnter(Player player) {
            if (BGModeManager.BGMode != mode) {
                BGModeManager.BGMode = mode;
                if (persistent) {
                    BGswitch.Session.BGMode = mode;
                }
                if (playEffects) {
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    Celeste.Freeze(0.05f);
                    Add(new Coroutine(BGModeManager.FlipFlash(SceneAs<Level>())));
                }
            }
        }
    }
}
