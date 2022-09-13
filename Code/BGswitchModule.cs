using Celeste.Mod.UI;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using On.Celeste;
using static Celeste.Mod.Everest.Events;

namespace Celeste.Mod.BGswitch
{
    public class BGswitch : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static BGswitch Instance;

        public static bool Fade;
        public static SpriteBank spriteBank;

        public BGswitch()
        {
            Instance = this;
        }

        // If you don't need to store any settings, => null
        public override Type SettingsType => null;

        // If you don't need to store any save data, => null
        public override Type SaveDataType => null;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            Everest.Events.Level.OnLoadEntity += OnLoadEntity;
            On.Celeste.LevelLoader.LoadingThread += OnChapter;
            On.Celeste.Level.LoadLevel += OnLoadLevel;
        }

        private void OnLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            BGModeToggle.BGMode = BGModeToggle.Persist;
            BGModeToggle.OldBGMode = !BGModeToggle.Persist;
            orig(self, playerIntro, isFromLoader);

            if (self != null)
            {
                if (self.Tracker != null)
                {
                    if (self.Tracker.GetEntity<Player>() != null)
                    {
                        if (self.Entities.Any(entity => (entity is BGModeToggle)) || BGModeToggle.BGMode) {
                            self.Tracker.GetEntity<Player>().Sprite.Position = new Vector2();
                            if (!BGModeToggle.Persist)
                            {
                                self.Tracker.GetEntity<Player>().Sprite.Position += new Vector2(0, +2);
                            }
                            BGModeToggle.Setup(self);
                            BGModeToggle.UpdateBG(self);
                        }
                    }
                }
            }
        }

        private void OnChapter(On.Celeste.LevelLoader.orig_LoadingThread orig, LevelLoader self)
        {
            BGModeToggle.BGMode = false;
            BGModeToggle.OldBGMode = false;
            BGModeToggle.Persist = false;
            Fade = true;
            orig(self);
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }


        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
            spriteBank = new SpriteBank(GFX.Game, "Graphics/BGSwitchSprites.xml");
        }

        // Unload the entirety of your mod's content, remove any event listeners ands undo all hooks.
        public override void Unload()
        {
            Everest.Events.Level.OnLoadEntity -= OnLoadEntity;
            On.Celeste.LevelLoader.LoadingThread -= OnChapter;
            On.Celeste.Level.LoadLevel -= OnLoadLevel;
        }

        private bool OnLoadEntity(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            string name = entityData.Name;

            switch (name)
            { 
                case "bgSwitch/bgModeToggle":
                    level.Add(new BGModeToggle(entityData, offset));
                    return true;

                case "bgSwitch/bgTrigger":
                    level.Add(new BGModeTrigger(entityData, offset));
                    return true;

                default:
                    return false;
            }
        }

    }
}
