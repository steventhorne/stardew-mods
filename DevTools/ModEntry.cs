using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DevTools;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree)) return;
        if (e.Button != SButton.OemTilde) return;

        Game1.activeClickableMenu = new WarpInterface();
    }
}
