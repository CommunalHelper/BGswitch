using System;
using System.Collections;
using Celeste;
using Celeste.Mod.BGswitch;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    public class BGModeToggle : Entity
    {
        private const float Cooldown = 1f;

        private bool iceMode;

        public static bool BGMode = false;

        public static bool Persist = false;

        public bool TrueBGMode = false;

        public static bool OldBGMode = false;

        public static Solid bgSolidTiles;

        private float cooldownTimer;

        private bool onlyFire;

        private bool starter;

        private bool onlyIce;

        private bool persistent;

        private bool playSounds;

        private Sprite sprite;

        private bool Usable => (!onlyFire || iceMode) && (!onlyIce || !iceMode);

        public BGModeToggle(Vector2 position, bool onlyFire, bool onlyIce, bool persistent)
            : base(position)
        {
            this.onlyFire = onlyFire;
            this.onlyIce = onlyIce;
            this.persistent = persistent;
            base.Collider = new Hitbox(16f, 20f, -8f, -8f);
            base.Add(new PlayerCollider(OnPlayer, null, null));
            base.Add(sprite = BGswitch.spriteBank.Create("coreFlipSwitch"));
            base.Depth = 2000;
        }

        public BGModeToggle(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("onlyOn", false), data.Bool("onlyOff", false), data.Bool("persistent", false))
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            iceMode = (BGMode == false);
            SetSprite(false);
            starter = true;
        }

        private void SetSprite(bool animate)
        {
            iceMode = (BGMode == false);
            if (animate)
            {
                if (playSounds)
                {
                    Audio.Play(iceMode ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", base.Position);
                }
                if (Usable)
                {
                    sprite.Play(iceMode ? "ice" : "hot", false, false);
                }
                else
                {
                    if (playSounds)
                    {
                        Audio.Play("event:/game/09_core/switch_dies", base.Position);
                    }
                    sprite.Play(iceMode ? "iceOff" : "hotOff", false, false);
                }
            }
            else if (Usable)
            {
                sprite.Play(iceMode ? "iceLoop" : "hotLoop", false, false);
            }
            else
            {
                sprite.Play(iceMode ? "iceOffLoop" : "hotOffLoop", false, false);
            }
            playSounds = false;
        }

        private void OnPlayer(Player player)
        {
            if (Usable && cooldownTimer <= 0f)
            {
                playSounds = true;
                Level level = base.SceneAs<Level>();
                if (BGMode == false)
                {
                    BGMode = true;
                }
                else
                {
                    BGMode = false;
                }
                if(persistent)
                {
                    Persist = BGMode;
                }
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                level.Flash(Color.White * 0.5f, true);
                Celeste.Freeze(0.05f);
                cooldownTimer = 1f;
            }
        }

        public override void Update()
        {
            base.Update();
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Engine.DeltaTime;
            }
            BGUpdate();
        }

        public void BGUpdate()
        {
            Level level = base.Scene as Level;
            if (starter)
            {
                SetSprite(false);
            }
            if (!starter && BGMode != OldBGMode)
            {
                Add(new Coroutine(FlipFlash(level)));
            }
            UpdateBG(level);
            if (TrueBGMode != BGMode)
            {
                SetSprite(!starter);
                TrueBGMode = BGMode;
            }
            starter = false;
        }

        public static IEnumerator FlipFlash(Level level)
        {
            level.Shake(0.1f);
            level.Flash(Color.White, false);
            level.FormationBackdrop.Display = true;
            level.FormationBackdrop.Alpha = 0.4f;
            yield return 0.3f;
            level.FormationBackdrop.Display = false;
        }

        public static void Setup(Level level)
        {
            if (bgSolidTiles != null)
            {
                level.Remove(bgSolidTiles);
            }
            Rectangle bounds = level.Bounds;
            Rectangle rectangle = new Rectangle(bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            bool[,] array = new bool[rectangle.Width, rectangle.Height];
            for (int i = 0; i < rectangle.Width; i++)
            {
                for (int j = 0; j < rectangle.Height; j++)
                {
                    bool[,] array2 = array;
                    int num = i;
                    int num2 = j;
                    bool num3 = level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0';
                    array2[num, num2] = num3;
                }
            }
            bounds = level.Bounds;
            float x = (float)bounds.Left;
            bgSolidTiles = new Solid(new Vector2(x, (float)bounds.Top), 1f, 1f, true)
            {
                Collider = new Grid(8f, 8f, new VirtualMap<bool>( array, false))
            };
            bgSolidTiles.Collidable = false;
            level.Add(bgSolidTiles);
        }

        public static void UpdateBG(Level level)
        {
            if (OldBGMode == false && BGMode == true)
            {
                //level.Tracker.GetEntity<Player>().Depth = 1;
                level.Tracker.GetEntity<Player>().Sprite.Position += new Vector2(0, +2f);
                level.SolidTiles.Collidable = false;
                bgSolidTiles.Collidable = true;
            }
            if (OldBGMode == true && BGMode == false)
            {
                //level.Tracker.GetEntity<Player>().Depth = 0;
                level.Tracker.GetEntity<Player>().Sprite.Position += new Vector2(0, -2);
                level.SolidTiles.Collidable = true;
                bgSolidTiles.Collidable = false;
            }
            OldBGMode = BGMode;
        }
    }
}