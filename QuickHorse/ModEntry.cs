using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;
using System.Linq;
using System;

namespace QuickHorse
{
    public sealed class ModConfig
    {
        public SButton Keybind { get; set; } = SButton.H;
        public bool SkipDelay { get; set; }
        public bool RequireFlute { get; set; } = true;
    }

    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        private readonly Lazy<StardewValley.Object> Flute = new(() => ItemRegistry.Create<StardewValley.Object>(HORSE_FLUTE_ID));

        private const string HORSE_FLUTE_ID = ItemRegistry.type_object + "911";

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
                name: () => "Keybind",
                getValue: () => Config.Keybind,
                setValue: value => Config.Keybind = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Skip Delay",
                getValue: () => Config.SkipDelay,
                setValue: value => Config.SkipDelay = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Require Flute in Inventory",
                getValue: () => Config.RequireFlute,
                setValue: value => Config.RequireFlute = value
            );
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.IsPlayerFree) return;
            if (Config.Keybind == SButton.None) return;
            if (e.Button != Config.Keybind) return;

            if (Config.RequireFlute)
            {
                var hasFlute = Game1.player.Items.Any(i => i?.QualifiedItemId == HORSE_FLUTE_ID);
                if (!hasFlute) return;
            }

            if (Config.SkipDelay)
            {
                Game1.MusicDuckTimer = 2000f;
                Game1.playSound("horse_flute");
                string horseWarpErrorMessage = Utility.GetHorseWarpErrorMessage(Utility.GetHorseWarpRestrictionsForFarmer(Game1.player));
                if (horseWarpErrorMessage != null)
                    Game1.showRedMessage(horseWarpErrorMessage);
                else
                    Game1.player.team.requestHorseWarpEvent.Fire(Game1.player.UniqueMultiplayerID);
            }
            else
            {
                Flute.Value.performUseAction(Game1.currentLocation);
            }
        }
    }
}
