using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.BGswitch {
    [CustomEntity("bgSwitch/bgModeToggle")]
    public class BGModeToggle : Entity {
        private readonly bool onlyOn;
        private readonly bool onlyOff;
        private readonly bool persistent;
        private readonly Sprite sprite;
        private float cooldownTimer;
        private bool playSounds;

        public BGModeToggle(Vector2 position, bool onlyOn, bool onlyOff, bool persistent)
            : base(position) {
            this.onlyOn = onlyOn;
            this.onlyOff = onlyOff;
            this.persistent = persistent;
            Collider = new Hitbox(16f, 20f, -8f, -8f);
            Add(new BGModeListener(new Action<bool>(OnChangeMode)));
            Add(new PlayerCollider(OnPlayer));
            Add(sprite = GFX.SpriteBank.Create("BGswitch_toggle"));
            Depth = 2000;
        }

        public BGModeToggle(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("onlyOn", false), data.Bool("onlyOff", false), data.Bool("persistent", false)) {
        }

        private bool Usable => (!onlyOn || !BGModeManager.BGMode) && (!onlyOff || BGModeManager.BGMode);

        public override void Added(Scene scene) {
            base.Added(scene);
            if (Usable) {
                sprite.Play(BGModeManager.BGMode ? "bgLoop" : "fgLoop");
            } else {
                sprite.Play(BGModeManager.BGMode ? "bgOffLoop" : "fgOffLoop");
            }
        }

        public override void Update() {
            base.Update();
            if (cooldownTimer > 0f) {
                cooldownTimer -= Engine.DeltaTime;
            }
        }

        private void OnChangeMode(bool bgMode) {
            if (playSounds) {
                Audio.Play(bgMode ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", Position);
            }

            if (Usable) {
                sprite.Play(bgMode ? "bg" : "fg");
            } else {
                sprite.Play(bgMode ? "bgOff" : "fgOff");
                if (playSounds) {
                    Audio.Play("event:/game/09_core/switch_dies", Position);
                }
            }

            playSounds = false;
        }

        private void OnPlayer(Player player) {
            if (Usable && cooldownTimer <= 0f) {
                playSounds = true;
                BGModeManager.BGMode = !BGModeManager.BGMode;
                if (persistent) {
                    BGswitch.Session.BGMode = BGModeManager.BGMode;
                }

                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Celeste.Freeze(0.05f);
                Add(new Coroutine(BGModeManager.FlipFlash(SceneAs<Level>())));
                cooldownTimer = 1f;
            }
        }
    }
}