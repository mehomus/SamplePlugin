using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Dalamud.Game.Network.Structures.InfoProxy;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Collections.Generic;
using System.Linq;

namespace SamplePlugin;
public class SimplePlayer
{
    public string Name { get; private set; }
    public string World { get; private set; }

    public SimplePlayer(string name, string world)
    {
        Name = name;
        World = world;
    }
}

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        //make dict
        Dictionary<int, string> worldDict = new Dictionary<int, string>();
        worldDict.Add(402, "Alpha");
        worldDict.Add(36, "Lich");
        worldDict.Add(66, "Odin");
        worldDict.Add(56, "Phoenix");
        worldDict.Add(403, "Raiden");
        worldDict.Add(67, "Shiva");
        worldDict.Add(33, "Twintania");
        worldDict.Add(42, "Zodiark");
        worldDict.Add(80, "Cerberus");
        worldDict.Add(83, "Louisoix");
        worldDict.Add(71, "Moogle");
        worldDict.Add(39, "Omega");
        worldDict.Add(401, "Phantom");
        worldDict.Add(97, "Ragnarok");
        worldDict.Add(400, "Sagittarius");
        worldDict.Add(85, "Spriggan");


        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var pagManImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "PagMan.png");
        var sadgeImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "sadge.png");
        string playerName;
        var playerList = new List<SimplePlayer>();
        unsafe
        {
            playerName = PlayerState.Instance()->CharacterNameString;
           
            for (int i = 0; i < InfoProxyPartyMember.Instance()->EntryCount; i++)
            {
                var name = InfoProxyPartyMember.Instance()->CharDataSpan[i].NameString;
                var world = worldDict[InfoProxyPartyMember.Instance()->CharDataSpan[i].HomeWorld];
                playerList.Add(new SimplePlayer(name, world));
                
            }
        }


        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, pagManImagePath, sadgeImagePath, playerName, playerList);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
