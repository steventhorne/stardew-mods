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
    public bool ShowOnScreenNames { get; set; }
    public bool ShowOnScreenGiftIndicator { get; set; } = true;
    public bool ShowOnScreenTalkIndicator { get; set; }
    public bool ShowOnScreenBirthdayIndicator { get; set; } = true;
    public bool ShowOffScreenIcons { get; set; } = true;
    public bool ShowOffScreenNamesOnHover { get; set; } = true;
    public bool ShowOffScreenGiftIndicator { get; set; } = true;
    public bool ShowOffScreenTalkIndicator { get; set; }
    public bool ShowOffScreenBirthdayIndicator { get; set; } = true;
    public float IconScale { get; set; } = 1.0f;
    public int SafeZoneMargin { get; set; } = 15;
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

    private const int ICON_SIZE = 48;
    private readonly Rectangle GIFT_ICON_SOURCE_RECT = new(167, 175, 12, 11);
    private readonly Rectangle TALK_ICON_SOURCE_RECT = new(181, 175, 12, 11);
    private readonly Rectangle BIRTHDAY_ICON_SOURCE_RECT = new(5 * 16, 9 * 16, 16, 16);

    private int IconSizeScaled => (int)(ICON_SIZE * Config.IconScale * Game1.options.uiScale);
    private int SubIconSizeScaled => IconSizeScaled / 2;

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

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show On-Screen Names",
            getValue: () => Config.ShowOnScreenNames,
            setValue: value => Config.ShowOnScreenNames = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show On-Screen Gift Indicator",
            getValue: () => Config.ShowOnScreenGiftIndicator,
            setValue: value => Config.ShowOnScreenGiftIndicator = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show On-Screen Talk Indicator",
            getValue: () => Config.ShowOnScreenTalkIndicator,
            setValue: value => Config.ShowOnScreenTalkIndicator = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show On-Screen Birthday Indicator",
            getValue: () => Config.ShowOnScreenBirthdayIndicator,
            setValue: value => Config.ShowOnScreenBirthdayIndicator = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Off-Screen Icons",
            getValue: () => Config.ShowOffScreenIcons,
            setValue: value => Config.ShowOffScreenIcons = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Off-Screen Names On Hover",
            getValue: () => Config.ShowOffScreenNamesOnHover,
            setValue: value => Config.ShowOffScreenNamesOnHover = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Off-Screen Gift Indicator",
            getValue: () => Config.ShowOffScreenGiftIndicator,
            setValue: value => Config.ShowOffScreenGiftIndicator = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Off-Screen Talk Indicator",
            getValue: () => Config.ShowOffScreenTalkIndicator,
            setValue: value => Config.ShowOffScreenTalkIndicator = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Show Off-Screen Birthday Indicator",
            getValue: () => Config.ShowOffScreenBirthdayIndicator,
            setValue: value => Config.ShowOffScreenBirthdayIndicator = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Icon Scale (%)",
            getValue: () => Config.IconScale,
            setValue: value => Config.IconScale = value,
            min: 0.1f,
            max: 4.0f,
            interval: 0.1f
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Safe Zone Margin",
            getValue: () => Config.SafeZoneMargin,
            setValue: value => Config.SafeZoneMargin = value,
            min: 0,
            max: 100,
            interval: 1
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

            var friendshipData = Game1.player.friendshipData[character.Name];
            bool canGift = friendshipData.GiftsToday == 0
                && (friendshipData.GiftsThisWeek < 2
                    || Game1.player.spouse == character.Name
                    || character.isBirthday());

            var origcharPos = character.getLocalPosition(Game1.viewport);
            var charPos = new Vector2(origcharPos.X + (Game1.tileSize / 2), origcharPos.Y + Game1.tileSize);
            var name = character.Name;

            var font = Game1.smallFont;
            var labelSize = font.MeasureString(name);
            var labelPos = Utility.ModifyCoordinatesForUIScale(charPos);
            labelPos = new Vector2(labelPos.X - labelSize.X / 2, labelPos.Y);

            bool isOnScreen = Game1.viewport.Contains(new xTile.Dimensions.Location((int)character.Position.X, (int)character.Position.Y));
            if (isOnScreen)
            {
                if (Config.ShowOnScreenNames)
                {
                    var oldShadowColor = Game1.textShadowDarkerColor;
                    Game1.textShadowDarkerColor = Color.Black;
                    Utility.drawTextWithShadow(b, name, font, labelPos, Color.White);
                    Game1.textShadowDarkerColor = oldShadowColor;
                }

                var charBottom = Utility.ModifyCoordinatesForUIScale(charPos);
                if (Config.ShowOnScreenGiftIndicator)
                {
                    if (canGift)
                    {
                        var giftIconRect = new Rectangle((int)charBottom.X + (int)(SubIconSizeScaled * 0.75f), (int)charBottom.Y - SubIconSizeScaled, SubIconSizeScaled, SubIconSizeScaled);
                        b.Draw(Game1.mouseCursors2, giftIconRect, GIFT_ICON_SOURCE_RECT, Color.White);
                    }
                }

                if (Config.ShowOnScreenTalkIndicator && !friendshipData.TalkedToToday)
                {
                    var talkIconRect = new Rectangle((int)charBottom.X - (int)(SubIconSizeScaled * 1.75f), (int)charBottom.Y - SubIconSizeScaled, SubIconSizeScaled, SubIconSizeScaled);
                    b.Draw(Game1.mouseCursors2, talkIconRect, TALK_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOnScreenBirthdayIndicator && character.isBirthday())
                {
                    var birthdayIconRect = new Rectangle((int)charBottom.X - (int)(SubIconSizeScaled * 0.5f), (int)charBottom.Y - SubIconSizeScaled, SubIconSizeScaled, SubIconSizeScaled);
                    b.Draw(SpringObjectsTexture, birthdayIconRect, BIRTHDAY_ICON_SOURCE_RECT, Color.White);
                }
            }

            if (!isOnScreen && Config.ShowOffScreenIcons)
            {
                var characterData = GetCharacterTextureData(character);
                var edgePos = new Vector2(
                    MathHelper.Clamp(charPos.X, 0, Game1.viewport.Width),
                    MathHelper.Clamp(charPos.Y, 0, Game1.viewport.Height)
                );
                var screenPos = Utility.ModifyCoordinatesForUIScale(edgePos);
                screenPos = new Vector2(
                    MathHelper.Clamp(screenPos.X, Config.SafeZoneMargin, Game1.uiViewport.Width - Config.SafeZoneMargin - IconSizeScaled),
                    MathHelper.Clamp(screenPos.Y, Config.SafeZoneMargin, Game1.uiViewport.Height - Config.SafeZoneMargin - IconSizeScaled)
                );
                var drawRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, IconSizeScaled, IconSizeScaled);
                b.Draw(characterData.Texture, drawRect, characterData.SourceRect, Color.White);

                if (Config.ShowOffScreenNamesOnHover && drawRect.Contains(Game1.getMousePosition()))
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

                if (Config.ShowOffScreenGiftIndicator)
                {
                    if (canGift)
                    {
                        var giftIconRect = new Rectangle(drawRect.Center.X + (int)(SubIconSizeScaled * 0.75f), drawRect.Bottom - SubIconSizeScaled + 10, SubIconSizeScaled, SubIconSizeScaled);
                        b.Draw(Game1.mouseCursors2, giftIconRect, GIFT_ICON_SOURCE_RECT, Color.White);
                    }
                }

                if (Config.ShowOffScreenTalkIndicator && !friendshipData.TalkedToToday)
                {
                    var talkIconRect = new Rectangle(drawRect.Center.X - (int)(SubIconSizeScaled * 1.75f), drawRect.Bottom - SubIconSizeScaled + 10, SubIconSizeScaled, SubIconSizeScaled);
                    b.Draw(Game1.mouseCursors2, talkIconRect, TALK_ICON_SOURCE_RECT, Color.White);
                }

                if (Config.ShowOffScreenBirthdayIndicator && character.isBirthday())
                {
                    var birthdayIconRect = new Rectangle(drawRect.Center.X - (int)(SubIconSizeScaled * 0.5f), drawRect.Bottom - SubIconSizeScaled + 10, SubIconSizeScaled, SubIconSizeScaled);
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
