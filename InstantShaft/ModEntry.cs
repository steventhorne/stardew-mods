using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace InstantShaft
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            foreach (var key in Helper.Translation.GetKeys())
                Monitor.Log($"{key}: {Helper.Translation.Get(key)}", LogLevel.Debug);
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null) return;

            if (e.NewMenu is DialogueBox box)
            {
                if (!box.isQuestion) return;
                if (box.responses.Length < 2) return;
                if (box.responses[0].responseKey != "Jump") return;

                box.safetyTimer = 0;
                box.transitioning = false;
                box.receiveKeyPress(box.responses[0].hotkey);
            }
        }
    }
}
