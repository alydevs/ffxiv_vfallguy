using System;
using System.Linq;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace vfallguy;

public sealed class Plugin : IDalamudPlugin
{
    public IDalamudPluginInterface Dalamud { get; init; }

    public WindowSystem WindowSystem = new("vfallguy");
    private MainWindow _wnd;
    private const uint _MGF = 41629;
    private static readonly unsafe CurrencyManager* currencyManager = CurrencyManager.Instance();
    private static int? _mgf;
    private static DateTime? _mgfLastUpdate;
    internal unsafe static int MGF
    {
        get
        {
            if (_mgf != null && _mgfLastUpdate != null && (DateTime.Now - _mgfLastUpdate.Value).Seconds < 5)
                return _mgf.Value;
            _mgf = currencyManager->ItemBucket.First(i => i.Key.Equals(_MGF)).Value.Count;
            _mgfLastUpdate = DateTime.Now;
            Service.Log.Verbose($"Updated MGF: {_mgf}");
            return _mgf.Value;
        }
    }

    public Plugin(IDalamudPluginInterface dalamud)
    {
        dalamud.Create<Service>();

        _wnd = new();
        WindowSystem.AddWindow(_wnd);

        Dalamud = dalamud;
        dalamud.UiBuilder.DisableAutomaticUiHide = true;
        Dalamud.UiBuilder.Draw += WindowSystem.Draw;
        //Dalamud.UiBuilder.OpenConfigUi += () => _wndConfig.IsOpen = true;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        _wnd.Dispose();
    }
}
