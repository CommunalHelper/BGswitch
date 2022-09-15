using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            BGModeManager.Load();

            // Our static variables aren't saved automatically with Speedrun Tool, so we need to register them
            EverestModuleMetadata speedrunTool = new() { 
                Name = "SpeedrunTool", 
                Version = new Version(3, 11, 4) 
            };
            if (Everest.Loader.TryGetDependency(speedrunTool, out EverestModule srtModule)) {
                Type saveLoadAction = srtModule.GetType().Assembly.GetType("Celeste.Mod.SpeedrunTool.SaveLoad.SaveLoadAction");
                MethodInfo safeAdd = saveLoadAction.GetMethod("SafeAdd", BindingFlags.Public | BindingFlags.Static);
                MethodInfo saveStaticMemberValues = saveLoadAction.GetMethod("SaveStaticMemberValues", BindingFlags.Public | BindingFlags.Static);
                // Interface might change soon, so don't immediately crash if that happens lol
                MethodInfo loadStaticMemberValues = saveLoadAction.GetMethod("LoadStaticMemberValues", BindingFlags.NonPublic | BindingFlags.Static) 
                    ?? saveLoadAction.GetMethod("LoadStaticMemberValues", BindingFlags.Public | BindingFlags.Static);

                Action<Dictionary<Type, Dictionary<string, object>>, Level> saveAction = (savedValues, _) => 
                    saveStaticMemberValues.Invoke(null, new object[] { savedValues, typeof(BGModeManager), new string[] { "bgMode", "bgSolidTiles", "bgSolidTilesGrid" } });
                Action<Dictionary<Type, Dictionary<string, object>>, Level> loadAction = (savedValues, _) => 
                    loadStaticMemberValues.Invoke(null, new object[] { savedValues });

                safeAdd.Invoke(null, new object[] { saveAction, loadAction, null, null, null });
            }
        }

        public override void Unload() {
            BGModeManager.Unload();
        }

        public override void LoadContent(bool firstLoad) {
            spriteBank = new SpriteBank(GFX.Game, "Graphics/BGSwitchSprites.xml");
        }

        [Command("bg_mode", "set the bg mode of the level [BGswitch]")]
        private static void CmdBgMode(bool bgMode = false, bool persistent = false) {
            if (Engine.Scene is Level) {
                BGModeManager.BGMode = bgMode;
                if (persistent) {
                    Session.BGMode = bgMode;
                }
            }
        }
    }
}
