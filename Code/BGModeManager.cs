using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.BGswitch {
    public static class BGModeManager {
        private static ILHook transitionRoutineHook;
        private static Solid bgSolidTiles;
        private static Grid bgSolidTilesGrid;

        private static bool bgMode;
        public static bool BGMode {
            get { return bgMode; }
            set {
                if (bgMode != value) {
                    bgMode = value;
                    if (Engine.Scene is Level level) {
                        level.Session.SetFlag("bg_mode", bgMode);
                        level.SolidTiles.Collidable = !bgMode;
                        bgSolidTiles.Collidable = bgMode;

                        // If something external is using BG mode, enable our collider
                        if (bgSolidTiles.Collider == null) {
                            bgSolidTiles.Collider = bgSolidTilesGrid;
                        }

                        if (level.Tracker.GetEntity<Player>() is Player player) {
                            player.Sprite.Position.Y += bgMode ? 2f : -2f;
                            player.Depth = bgMode ? Depths.BGTerrain + 1 : Depths.Player;
                        }
                        
                        foreach (BGModeListener listener in level.Tracker.GetComponents<BGModeListener>()) {
                            listener.OnChange(bgMode);
                        }
                    }
                }
            }
        }

        public static IEnumerator FlipFlash(Level level) {
            level.Shake(0.1f);
            level.FormationBackdrop.Display = true;
            level.FormationBackdrop.Alpha = 0.4f;
            yield return 0.3f;
            level.FormationBackdrop.Display = false;
        }

        internal static void Load() {
            On.Celeste.Level.LoadLevel += OnLoadLevel;
            On.Celeste.Player.NormalBegin += OnNormalBegin;
            Everest.Events.Level.OnTransitionTo += OnTransition;

            // Unfortunately this is a pretty fragile hook. Fortunately it's not that important as BGMode isn't that usable with vanilla anyway. Mostly to prevent freezes if people try.
            MethodInfo transitionRoutine = typeof(Level).GetNestedType("<TransitionRoutine>d__26", BindingFlags.NonPublic)?.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
            if (transitionRoutine != null) {
                transitionRoutineHook = new ILHook(transitionRoutine, ModTransitionRoutine);
            } else {
                Logger.Log(LogLevel.Warn, "BGswitch", "Could not hook TransitionRoutine - signature has changed!");
            }                
        }

        internal static void Unload() {
            On.Celeste.Level.LoadLevel -= OnLoadLevel;
            Everest.Events.Level.OnTransitionTo -= OnTransition;
            On.Celeste.Player.NormalBegin -= OnNormalBegin;
            transitionRoutineHook?.Dispose();
            transitionRoutineHook = null;
        }

        private static void OnLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
            if (level.Session.JustStarted && level.Session.FirstLevel) {
                BGswitch.Session.BGMode = false;
            }

            bgMode = BGswitch.Session.BGMode;

            orig(level, playerIntro, isFromLoader);

            // Create a collision solid for BG tiles
            // Colliders draw hitboxes even when inactive, so only set if the room is likely to use BG mode
            bgSolidTilesGrid = CreateBgtileGrid(level);
            bgSolidTiles = new Solid(new Vector2(level.Bounds.Left, level.Bounds.Top), 1f, 1f, safe: true) {
                AllowStaticMovers = false,
                Collidable = bgMode,
                Collider = (bgMode || BgEntityInLevel(level)) ? bgSolidTilesGrid : null,
                EnableAssistModeChecks = false
            };
            level.Add(bgSolidTiles);

            // Set our defaults
            level.Session.SetFlag("bg_mode", bgMode);
            level.SolidTiles.Collidable = !bgMode;
            if (level.Tracker.GetEntity<Player>() is Player player) {
                player.Sprite.Position.Y = bgMode ? 2f : 0f;
                player.Depth = bgMode ? Depths.BGTerrain + 1 : Depths.Player;
            }
        }

        private static void OnTransition(Level level, LevelData next, Vector2 direction) {
            BGswitch.Session.BGMode = bgMode;
        }

        // Restore BG mode depth if something (e.g. dream dash state) changed it
        private static void OnNormalBegin(On.Celeste.Player.orig_NormalBegin orig, Player player) {
            orig(player);
            if (bgMode) {
                player.Depth = Depths.BGTerrain + 1;
            }
        }

        // Normally the GBJ check is disabled in vanilla. This hook re-enables it if we are using BG mode
        private static void ModTransitionRoutine(ILContext ctx) {
            ILCursor cursor = new(ctx);
            // Return true to skip GBJ only if we're in vanilla AND we are not in BG mode
            if (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchCall(typeof(AreaKeyExt), "GetLevelSet"),
                instr => instr.MatchLdstr("Celeste"),
                instr => instr.MatchCall(typeof(string), "op_Equality"))) {
                cursor.EmitDelegate<Func<bool, bool>>((isVanillaMap) => {
                    return isVanillaMap && !bgMode;
                });
            }
        }

        private static Grid CreateBgtileGrid(Level level) {
            Rectangle rectangle = new(level.Bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            bool[,] array = new bool[rectangle.Width, rectangle.Height];
            for (int i = 0; i < rectangle.Width; i++) {
                for (int j = 0; j < rectangle.Height; j++) {
                    array[i, j] = level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0';
                }
            }

            return new Grid(8f, 8f, array);
        }

        private static bool BgEntityInLevel(Level level) {
            return level.Entities.Any(e => e is BGModeToggle || e is BGModeTrigger);
        }
    }
}
