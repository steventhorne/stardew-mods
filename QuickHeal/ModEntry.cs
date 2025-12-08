using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;

namespace QuickHeal
{
    public sealed class ModConfig
    {
        public SButton Keybind { get; set; } = SButton.H;
    }

    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Quick Heal",
                getValue: () => Config.Keybind,
                setValue: value => Config.Keybind = value
            );
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            if (e.Button != Config.Keybind) return;
            if (Game1.player.isEating) return;

            Object firstFood = null;
            foreach (var item in Game1.player.Items)
            {
                if (item == null) continue;
                if (item.TypeDefinitionId != "(O)") continue;

                var obj = (Object)item;
                if (obj.Edibility <= 0) continue;

                firstFood = obj;
                break;
            }

            if (firstFood != null)
            {
                if (firstFood.Stack > 1) firstFood.Stack--;
                else Game1.player.removeItemFromInventory(firstFood);
                Game1.player.eatObject(firstFood, true);
            }
        }
    }
}
