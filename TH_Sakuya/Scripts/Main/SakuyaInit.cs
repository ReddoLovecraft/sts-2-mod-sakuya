using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using System;
using System.Linq;
using System.Reflection;
namespace TH_Sakuya.Scripts.Main
{
    [ModInitializer("Init")]
    public class SakuyaInit
    {
   private const string ModSfxPrefix = "mod_sfx://";

        public static string ToModSfxPath(string localPath)
        {
            return ModSfxPrefix + localPath;
        }
    private static Harmony? _harmony;
    public static void Init()
    {
        TryRegisterGodotScriptAssembly();
        _harmony = new Harmony("TH_Sakuya");
        _harmony.PatchAll();
        Log.Debug("Sakuya mod has been loaded successfully");
    }

    private static void TryRegisterGodotScriptAssembly()
    {
        try
        {
            Assembly modAssembly = typeof(SakuyaInit).Assembly;
            Type? scriptManagerBridgeType = Type.GetType("Godot.Bridge.ScriptManagerBridge, GodotSharp");

            if (scriptManagerBridgeType == null)
            {
                return;
            }

            MethodInfo? lookupMethod = scriptManagerBridgeType.GetMethod(
                "LookupScriptsInAssembly",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: [typeof(Assembly)],
                modifiers: null
            );

            lookupMethod ??= scriptManagerBridgeType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(m =>
                {
                    ParameterInfo[] ps = m.GetParameters();
                    return ps.Length == 1
                        && ps[0].ParameterType == typeof(Assembly)
                        && (m.Name.Contains("Lookup", StringComparison.OrdinalIgnoreCase)
                            || m.Name.Contains("Load", StringComparison.OrdinalIgnoreCase)
                            || m.Name.Contains("Register", StringComparison.OrdinalIgnoreCase));
                });

            lookupMethod?.Invoke(null, [modAssembly]);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to register Godot scripts for TH_Sakuya: {e}");
        }
    }
    }
    [HarmonyPatch(typeof(NAudioManager), "PlayOneShot", [typeof(string), typeof(float)])]
    public static class ModSfxPatch
    {
        static bool Prefix(string path, float volume)
        {
            if (path.StartsWith("mod_sfx://"))
            {
                try 
                {
                    string resPath = "res://" + path.Substring(10); // 10 is "mod_sfx://".Length
                    var stream = ResourceLoader.Load<AudioStream>(resPath);
                    if (stream != null)
                    {
                        var player = new AudioStreamPlayer();
                        player.Stream = stream;
                        player.VolumeDb = Mathf.LinearToDb(volume);
                        NGame.Instance.AddChild(player);
                        player.Play();
                        player.Connect("finished", Callable.From(player.QueueFree));
                    }
                }
                catch (System.Exception e)
                {
                    Log.Error($"Failed to play mod sfx: {path}. Error: {e.Message}");
                }
                return false; // 拦截原本的 FMOD 播放
            }
            return true;
        }
    }
}
