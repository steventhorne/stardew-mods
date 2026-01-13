using System.Collections.Generic;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NpcHud;

public sealed class ModConfig
{
    // General
    public bool ShowBadgesOnHover { get; set; } = false;
    public float BadgeScale { get; set; } = 1.0f;
    public bool HideBadgesIfMaxedFriendship { get; set; } = true;

    // On-Screen
    public bool ShowOnScreenNames { get; set; } = false;
    public bool ShowOnScreenGiftBadge { get; set; } = true;
    public bool ShowOnScreenTalkBadge { get; set; } = false;
    public bool ShowOnScreenBirthdayBadge { get; set; } = true;

    // Off-Screen
    public bool ShowOffScreenTrackers { get; set; } = true;
    public float TrackerScale { get; set; } = 1.0f;
    public int TrackerSafeZone { get; set; } = 15;
    public bool ShowOffScreenNamesOnHover { get; set; } = true;
    public bool ShowOffScreenGiftBadge { get; set; } = true;
    public bool ShowOffScreenTalkBadge { get; set; } = false;
    public bool ShowOffScreenBirthdayBadge { get; set; } = true;
}

internal sealed class CharacterTextureData
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRect { get; set; }
}

internal sealed class ModEntry : Mod
{
    private ModConfig Config;

    private Dictionary<string, CharacterTextureData> CharacterTextures = new();

    private Texture2D SpringObjectsTexture;

    private const int TRACKER_SIZE = 48;
    private const int BADGE_SIZE = 24;

    private readonly Rectangle GIFT_ICON_SOURCE_RECT = new(167, 175, 12, 11);
    private readonly Rectangle TALK_ICON_SOURCE_RECT = new(181, 175, 12, 11);
    private readonly Rectangle BIRTHDAY_ICON_SOURCE_RECT = new(5 * 16, 9 * 16, 16, 16);

    private int TrackerSizeScaled => (int)(TRACKER_SIZE * Config.TrackerScale * Game1.options.uiScale);
    private int BadgeSizeScaled => (int)(BADGE_SIZE * Config.BadgeScale * Game1.options.uiScale);

    public override void Entry(IModHelper helper)
    {
        Config = Helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Display.RenderingHud += OnRenderingHud;
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        SpringObjectsTexture = Game1.content.Load<Texture2D>("Maps\\springobjects");

        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => "General Options"
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Badges On Hover",
            tooltip: () => "Only show gift/talk/birthday badges when hovering over the NPC or tracker.",
            getValue: () => Config.ShowBadgesOnHover,
            setValue: value => Config.ShowBadgesOnHover = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Badge Scale (%)",
            tooltip: () => "The scale of the gift/talk/birthday badges.",
            getValue: () => Config.BadgeScale,
            setValue: value => Config.BadgeScale = value,
            min: 0.1f,
            max: 4.0f,
            interval: 0.1f
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Hide Badges If Maxed Friendship",
            tooltip: () => "Hide gift/talk badges for NPCs with max friendship.",
            getValue: () => Config.HideBadgesIfMaxedFriendship,
            setValue: value => Config.HideBadgesIfMaxedFriendship = value
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => "On-Screen Options"
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Names",
            tooltip: () => "Show NPC names below their feet when they are on-screen.",
            getValue: () => Config.ShowOnScreenNames,
            setValue: value => Config.ShowOnScreenNames = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Gift Badge",
            tooltip: () => "Show a badge on NPCs if you can give them a gift today.",
            getValue: () => Config.ShowOnScreenGiftBadge,
            setValue: value => Config.ShowOnScreenGiftBadge = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Talk Badge",
            tooltip: () => "Show a badge on NPCs if you haven't talked to them today.",
            getValue: () => Config.ShowOnScreenTalkBadge,
            setValue: value => Config.ShowOnScreenTalkBadge = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Birthday Badge",
            tooltip: () => "Show a badge on NPCs if it's their birthday.",
            getValue: () => Config.ShowOnScreenBirthdayBadge,
            setValue: value => Config.ShowOnScreenBirthdayBadge = value
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => "Off-Screen Options"
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Trackers",
            tooltip: () => "Show indicators at the edge of the screen for off-screen NPCs.",
            getValue: () => Config.ShowOffScreenTrackers,
            setValue: value => Config.ShowOffScreenTrackers = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Tracker Scale (%)",
            tooltip: () => "The scale of the off-screen NPC trackers.",
            getValue: () => Config.TrackerScale,
            setValue: value => Config.TrackerScale = value,
            min: 0.1f,
            max: 4.0f,
            interval: 0.1f
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Tracker Safe Zone",
            tooltip: () => "The distance from the edge of the screen that trackers will stay within.",
            getValue: () => Config.TrackerSafeZone,
            setValue: value => Config.TrackerSafeZone = value,
            min: 0,
            max: 100,
            interval: 1
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Names On Hover",
            tooltip: () => "Show NPC names when hovering over off-screen trackers.",
            getValue: () => Config.ShowOffScreenNamesOnHover,
            setValue: value => Config.ShowOffScreenNamesOnHover = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Gift Badge",
            tooltip: () => "Show a badge on off-screen trackers if you can give the NPC a gift today.",
            getValue: () => Config.ShowOffScreenGiftBadge,
            setValue: value => Config.ShowOffScreenGiftBadge = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Talk Badge",
            tooltip: () => "Show a badge on off-screen trackers if you haven't talked to the NPC today.",
            getValue: () => Config.ShowOffScreenTalkBadge,
            setValue: value => Config.ShowOffScreenTalkBadge = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Birthday Badge",
            tooltip: () => "Show a badge on off-screen trackers if it's the NPC's birthday.",
            getValue: () => Config.ShowOffScreenBirthdayBadge,
            setValue: value => Config.ShowOffScreenBirthdayBadge = value
        );
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
    }

    private void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        Draw(e.SpriteBatch);
    }

    private void Draw(SpriteBatch b)
    {
        if (!Context.IsWorldReady) return;

        foreach (var character in Game1.currentLocation.characters)
        {
            if (!character.IsVillager) continue;

            // friendship data
            bool canGift = false; // default to hidden
            bool talkedToToday = true; // default to hidden
            bool isMaxedFriendship = false;
            if (Game1.player.friendshipData.TryGetValue(character.Name, out var friendshipData))
            {
                canGift = friendshipData.GiftsToday == 0
                    && (friendshipData.GiftsThisWeek < 2
                        || Game1.player.spouse == character.Name
                        || character.isBirthday());
                var maxHearts = Utility.GetMaximumHeartsForCharacter(character);
                isMaxedFriendship = friendshipData.Points >= maxHearts * 250;
                talkedToToday = friendshipData.TalkedToToday;
            }

            var origcharPos = character.getLocalPosition(Game1.viewport);
            var charPos = new Vector2(origcharPos.X + (Game1.tileSize / 2), origcharPos.Y + Game1.tileSize);
            var name = character.getName();

            var font = Game1.smallFont;
            var labelSize = font.MeasureString(name);
            var labelPos = Utility.ModifyCoordinatesForUIScale(charPos);
            labelPos = new Vector2(labelPos.X - labelSize.X / 2, labelPos.Y);

            bool isOnScreen = Game1.viewport.Contains(new xTile.Dimensions.Location((int)character.Position.X, (int)character.Position.Y));
            if (isOnScreen)
            {
                Vector2 inWorldMousePos = Utility.ModifyCoordinatesFromUIScale(new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y));
                var boundingBox = new Rectangle((int)origcharPos.X, (int)origcharPos.Y - Game1.tileSize, Game1.tileSize, Game1.tileSize * 2);
                bool hovering = boundingBox.Contains((int)inWorldMousePos.X, (int)inWorldMousePos.Y);
                if (Config.ShowOnScreenNames && (!Config.ShowBadgesOnHover || hovering))
                {
                    var oldShadowColor = Game1.textShadowDarkerColor;
                    Game1.textShadowDarkerColor = Color.Black;
                    Utility.drawTextWithShadow(b, name, font, labelPos, Color.White);
                    Game1.textShadowDarkerColor = oldShadowColor;
                }

                var charBottom = Utility.ModifyCoordinatesForUIScale(charPos);
                if (Config.ShowOnScreenGiftBadge && canGift && (hovering || !Config.ShowBadgesOnHover) && (!Config.HideBadgesIfMaxedFriendship || !isMaxedFriendship))
                {
                    var giftIconRect = new Rectangle((int)charBottom.X + (int)(BadgeSizeScaled * 0.75f), (int)charBottom.Y - BadgeSizeScaled, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(Game1.mouseCursors2, giftIconRect, GIFT_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOnScreenTalkBadge && !talkedToToday && (hovering || !Config.ShowBadgesOnHover) && (!Config.HideBadgesIfMaxedFriendship || !isMaxedFriendship))
                {
                    var talkIconRect = new Rectangle((int)charBottom.X - (int)(BadgeSizeScaled * 1.75f), (int)charBottom.Y - BadgeSizeScaled, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(Game1.mouseCursors2, talkIconRect, TALK_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOnScreenBirthdayBadge && character.isBirthday() && (hovering || !Config.ShowBadgesOnHover))
                {
                    var birthdayIconRect = new Rectangle((int)charBottom.X - (int)(BadgeSizeScaled * 0.5f), (int)charBottom.Y - BadgeSizeScaled, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(SpringObjectsTexture, birthdayIconRect, BIRTHDAY_ICON_SOURCE_RECT, Color.White);
                }
            }

            if (!isOnScreen && Config.ShowOffScreenTrackers)
            {
                var characterData = GetCharacterTextureData(character);
                var edgePos = new Vector2(
                    MathHelper.Clamp(charPos.X, 0, Game1.viewport.Width),
                    MathHelper.Clamp(charPos.Y, 0, Game1.viewport.Height)
                );
                var screenPos = Utility.ModifyCoordinatesForUIScale(edgePos);
                screenPos = new Vector2(
                    MathHelper.Clamp(screenPos.X, Config.TrackerSafeZone, Game1.uiViewport.Width - Config.TrackerSafeZone - TrackerSizeScaled),
                    MathHelper.Clamp(screenPos.Y, Config.TrackerSafeZone, Game1.uiViewport.Height - Config.TrackerSafeZone - TrackerSizeScaled)
                );
                var drawRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, TrackerSizeScaled, TrackerSizeScaled);
                b.Draw(characterData.Texture, drawRect, characterData.SourceRect, Color.White);

                bool hovering = drawRect.Contains(Game1.getMousePosition());

                if (Config.ShowOffScreenNamesOnHover && hovering)
                {
                    bool isOffLeft = origcharPos.X < 0;
                    bool isOffRight = origcharPos.X > Game1.viewport.Width;
                    bool isOffTop = origcharPos.Y < 0;
                    bool isOffBottom = origcharPos.Y > Game1.viewport.Height;
                    labelPos = new Vector2(
                        isOffLeft ? drawRect.Right + 10 :
                        isOffRight ? drawRect.Left - labelSize.X - 10 :
                        drawRect.Center.X - labelSize.X / 2,
                        (isOffLeft || isOffRight) ?
                        drawRect.Center.Y - labelSize.Y / 2 :
                        (isOffTop ? drawRect.Bottom + 5 :
                        isOffBottom ? drawRect.Top - labelSize.Y - 5 :
                        drawRect.Center.Y - labelSize.Y / 2)
                    );
                    var oldShadowColor = Game1.textShadowDarkerColor;
                    Game1.textShadowDarkerColor = Color.Black;
                    Utility.drawTextWithShadow(b, name, font, labelPos, Color.White);
                    Game1.textShadowDarkerColor = oldShadowColor;
                }

                if (Config.ShowOffScreenGiftBadge && canGift && (hovering || !Config.ShowBadgesOnHover) && (!Config.HideBadgesIfMaxedFriendship || !isMaxedFriendship))
                {
                    var giftIconRect = new Rectangle(drawRect.Center.X + (int)(BadgeSizeScaled * 0.75f), drawRect.Bottom - BadgeSizeScaled + 10, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(Game1.mouseCursors2, giftIconRect, GIFT_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOffScreenTalkBadge && !talkedToToday && (hovering || !Config.ShowBadgesOnHover) && (!Config.HideBadgesIfMaxedFriendship || !isMaxedFriendship))
                {
                    var talkIconRect = new Rectangle(drawRect.Center.X - (int)(BadgeSizeScaled * 1.75f), drawRect.Bottom - BadgeSizeScaled + 10, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(Game1.mouseCursors2, talkIconRect, TALK_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOffScreenBirthdayBadge && character.isBirthday() && (hovering || !Config.ShowBadgesOnHover))
                {
                    var birthdayIconRect = new Rectangle(drawRect.Center.X - (int)(BadgeSizeScaled * 0.5f), drawRect.Bottom - BadgeSizeScaled + 10, BadgeSizeScaled, BadgeSizeScaled);
                    b.Draw(SpringObjectsTexture, birthdayIconRect, BIRTHDAY_ICON_SOURCE_RECT, Color.White);
                }
            }
        }
    }

    private CharacterTextureData GetCharacterTextureData(NPC character)
    {
        if (CharacterTextures.TryGetValue(character.Name, out var data))
            return data;

        var texture = Game1.content.Load<Texture2D>($"Characters\\{character.getTextureName()}");

        // iterate over the pixels from top to bottom to find the first non-transparent row
        var pixels = new Color[texture.Width * texture.Height];
        texture.GetData(pixels);

        int yStart = 0;
        for (int y = 0; y < texture.Height; y++)
        {
            bool foundNonTransparent = false;
            for (int x = 0; x < texture.Width; x++)
            {
                if (pixels[y * texture.Width + x].A != 0)
                {
                    foundNonTransparent = true;
                    break;
                }
            }
            if (foundNonTransparent)
            {
                yStart = y;
                break;
            }
        }
        var sourceRect = new Rectangle(0, yStart, 16, 16);

        data = new CharacterTextureData
        {
            Texture = texture,
            SourceRect = sourceRect
        };

        CharacterTextures[character.Name] = data;
        return data;
    }
}
