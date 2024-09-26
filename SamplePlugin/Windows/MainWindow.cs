using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private string PagManImagePath;
    private string SadgeImagePath;
    private string PlayerName;
    private List<SimplePlayer> PartyList;
    private Plugin Plugin;


    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string pagmanImagePath, string sadgeImagePath, string playerName, List<SimplePlayer> partyList)
        : base("Proggers##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        SadgeImagePath = sadgeImagePath;
        PagManImagePath = pagmanImagePath;
        PlayerName = playerName;
        PartyList = partyList;
        Plugin = plugin;
    }

    public void Dispose() { }

    public bool isPog() 
    {
        if (Plugin.Configuration.SomePropertyToBeSavedAndWithADefault)
        {
            return true;
        }
        return false;
    }

    public override void Draw()
    {

        ImGui.Text($"I am: {PlayerName}");
        ImGui.Text($"partylength: {PartyList.Count}");
        foreach (var member in PartyList) {
            ImGui.Text($"Party member: {member.Name} World: {member.World}");
        }

        ImGui.Text(isPog()?"Man, we're POGGING2": "Man, we are NOT pogging");
        

        ImGui.Spacing();

        ImGui.Text(Plugin.Configuration.SomePropertyToBeSavedAndWithADefault?"PAGMAN:":"Sadge:");
        var pagmanImage = Plugin.TextureProvider.GetFromFile(PagManImagePath).GetWrapOrDefault();
        var sadgeImage = Plugin.TextureProvider.GetFromFile(SadgeImagePath).GetWrapOrDefault();
        if (isPog())
        {
            if (pagmanImage != null)
            {
                ImGuiHelpers.ScaledIndent(55f);
                ImGui.Image(pagmanImage.ImGuiHandle, new Vector2(pagmanImage.Width, pagmanImage.Height));
                ImGuiHelpers.ScaledIndent(-55f);
            }
        }else
            if (sadgeImage != null)
            {
                ImGuiHelpers.ScaledIndent(55f);
                ImGui.Image(sadgeImage.ImGuiHandle, new Vector2(128, 128));
                ImGuiHelpers.ScaledIndent(-55f);
            }



        else
        {
            ImGui.Text("Image not found.");
        }
        if (ImGui.Button("Show Settings"))
        {
            Plugin.ToggleConfigUI();
        }

    }

}
