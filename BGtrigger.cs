using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.BGswitch

{
    public class BGModeTrigger : Trigger
    {
        private readonly bool mode;
        private readonly bool persistant;

        public BGModeTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            mode = data.Bool("mode", true);
            persistant = data.Bool("persistant", true);
        }

        public override void OnEnter(Player player)
        {
            Level level = base.Scene as Level;
            if (BGModeToggle.BGMode != mode)
            {
                BGModeToggle.BGMode = mode;
                if (persistant)
                {
                    BGModeToggle.Persist = mode;
                }
                BGModeToggle.UpdateBG(level);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Celeste.Freeze(0.05f);
                Add(new Coroutine(BGModeToggle.FlipFlash(level)));
            }
        }
    }
}
