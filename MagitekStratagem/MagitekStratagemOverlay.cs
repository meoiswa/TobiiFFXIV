using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace MagitekStratagemPlugin
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    public unsafe class MagitekStratagemOverlay : Window, IDisposable
  {
    private readonly MagitekStratagemPlugin plugin;

    public MagitekStratagemOverlay(MagitekStratagemPlugin plugin)
      : base(
        "MagitekStratagem##Overlay",
        ImGuiWindowFlags.AlwaysAutoResize
        | ImGuiWindowFlags.NoResize
        | ImGuiWindowFlags.NoCollapse
        | ImGuiWindowFlags.NoDecoration
        | ImGuiWindowFlags.NoInputs
        | ImGuiWindowFlags.NoBringToFrontOnFocus
        | ImGuiWindowFlags.NoFocusOnAppearing
        | ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoBackground
      )
    {
      this.plugin = plugin;
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
    }

    public void DrawBubble(float x, float y)
    {
      var size = ImGui.GetIO().DisplaySize;
      var xp = x * (size.X / 2) + (size.X / 2);
      var yp = -y * (size.Y / 2) + (size.Y / 2);
      var pixelCoord = new Vector2(xp, yp);

      var white = ImGui.GetColorU32(new Vector4(1, 1, 1, 1));
      var black = ImGui.GetColorU32(new Vector4(0, 0, 0, 1));

      const float whiteThick = 3f;
      const float blackThick = 1.5f;

      ImGui.SetNextWindowSize(size);
      ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
      ImGui.SetNextWindowPos(Vector2.Zero);
      const ImGuiWindowFlags flags = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoDecoration |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoBackground |
                                     ImGuiWindowFlags.NoInputs;
      ImGui.Begin("Crosshair Window", flags);

      var dl = ImGui.GetWindowDrawList();

      dl.AddCircle(pixelCoord, plugin.Configuration.GazeCircleRadius + blackThick, black, plugin.Configuration.GazeCircleSegments, blackThick);
      dl.AddCircle(pixelCoord, plugin.Configuration.GazeCircleRadius - blackThick, black, plugin.Configuration.GazeCircleSegments, blackThick);
      dl.AddCircle(pixelCoord, plugin.Configuration.GazeCircleRadius, white, plugin.Configuration.GazeCircleSegments, whiteThick);

      ImGui.End();
    }

    public override void Draw()
    {
      if (!plugin.Configuration.IsVisible && !plugin.Configuration.OverlayEnabled)
      {
        return;
      }

      if (plugin.TrackerService == null)
      {
        return;
      }

      DrawBubble(plugin.TrackerService.LastGazeX, plugin.TrackerService.LastGazeY);
    }
  }
}
