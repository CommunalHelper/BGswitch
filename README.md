# BGswitch
A helper for Celeste. Originally created by PenguinOwl and now maintained by the Communal Helper organization. 

The release build can be downloaded [here](https://gamebanana.com/mods/53642).

Head to our [issues page](https://github.com/CommunalHelper/BGswitch/issues) to leave a bug report or feature request.

## API
This mod uses a [ModInterop API](https://github.com/EverestAPI/Resources/wiki/Cross-Mod-Functionality#modinterop), which allows you to import methods into your own mods without referencing BGswitch directly. To use it, add BGswitch as an [optional dependency](https://github.com/EverestAPI/Resources/wiki/Mod-Structure#optional-dependencies-for-everestyaml-advanced) and use the version where the methods you need were exported (or later).

Check out the [API code](https://github.com/CommunalHelper/BGswitch/blob/dev/Code/BGswitchInterop.cs) for documentation on individual methods.

Basic usage guide and version info:
```csharp
// Add somewhere in your mod. You only need to include delegates that you need.
[ModImportName("BGswitch")]
public static class BGswitchImports {
  // Added in v1.2.0
  public static Func<bool> IsBGMode;
  public static Action<bool, bool> SetBGMode;
  public static Func<Action<bool>, Component> GetBGModeListener;
}

// Add to YourModule.Load()
typeof(BGswitchImports).ModInterop();

// Example usages
public void LogMode(bool mode) {
  Logger.Log("MyMod", $"BG mode changed to: { mode }");
}

Component listener = GetBGModeListener?.Invoke(LogMode);
if (listener != null) {
  myEntity.Add(listener);
}

if (BGswitchImports.IsBGMode?.Invoke() ?? false) {
  Logger.Log("MyMod", "BG mode is active! Let's turn it off.");
  SetBGMode?.Invoke(false, false);
}
```
