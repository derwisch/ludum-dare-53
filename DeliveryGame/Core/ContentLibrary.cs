using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    public class ContentLibrary
    {
        public class Keys
        {
            // Misc
            public const string TextureChecker = "checker";
            public const string TextureMap = "map";
            public const string TexturePuffParticle = "particle-puff";
            public const string TextureWhitePixel = "white-pixel";

            // Tiles
            public const string TextureGrass = "grass";
            public const string TextureDepositCoal = "deposit-coal";
            public const string TextureDepositIron = "deposit-iron";
            public const string TextureDepositCopper = "deposit-copper";
            public const string TextureDepositSilicon = "deposit-silicon";
            public const string TextureDepositOil = "deposit-oil";

            // Buildings
            public const string TextureConveyorConnectorHorizontal = "conveyor-horizontal-connector";
            public const string TextureConveyorHorizontalLeft = "conveyor-horizontal-left";
            public const string TextureConveyorHorizontalRight = "conveyor-horizontal-right";
            public const string TextureConveyorConnectorVertical = "conveyor-vertical-connector";
            public const string TextureConveyorVerticalTop = "conveyor-vertical-top";
            public const string TextureConveyorVerticalBottom = "conveyor-vertical-bottom";
            public const string TextureConveyorConnectorTopLeft = "conveyor-top-left";
            public const string TextureConveyorConnectorTopRight = "conveyor-top-right";
            public const string TextureConveyorConnectorBottomLeft = "conveyor-bottom-left";
            public const string TextureConveyorConnectorBottomRight = "conveyor-bottom-right";
            public const string TextureDivider = "divider";
            public const string TextureMerger = "merger";
            public const string TextureExtractor = "extractor";
            public const string TextureSmeltery = "smeltery";
            public const string TextureAssembler = "assembler";
            public const string TextureHub = "hub";
            
            // Wares
            public const string TextureWareCoal = "coal";
            public const string TextureWareOreIron = "iron-ore";
            public const string TextureWareBarIron = "iron-bar";
            public const string TextureWareOreCopper = "copper-ore";
            public const string TextureWareBarCopper = "copper-bar";
            public const string TextureWareSilicon = "silicon";
            public const string TextureWareOil = "oil";
            public const string TextureWarePlastic = "plastic";
            public const string TextureWareGear = "gear";
            public const string TextureWareCopperCoil = "copper-coil";
            public const string TextureWareCircuit = "circuit";
            public const string TextureWareMotor = "motor";
            public const string TextureWareComputer = "computer";
            public const string TextureWareRobot = "robot";

            // UI
            public const string TextureBuildablesUI = "ui-buildables";
            public const string TextureMenuUI = "ui-menu";
            public const string TextureButtonUp = "ui-btn-up";
            public const string TextureButtonDown = "ui-btn-down";
            public const string TextureButtonHover = "ui-btn-hover";
            public const string TextureActiveBorder = "ui-active-border";
            public const string TextureIconConveyor = "ui-icon-conveyor";
            public const string TextureIconDivider = "ui-icon-divider";
            public const string TextureIconMerger = "ui-icon-merger";
            public const string TextureIconExtractor = "ui-icon-extractor";
            public const string TextureIconSelect = "ui-icon-select";
            public const string TextureIconRecipes = "ui-icon-recipes";
            public const string TextureIconRequests = "ui-icon-requests";
            public const string TextureDirectionArrow = "direction-arrow";
            public const string TextureWindow = "window";
            public const string TextureSplash = "splash";
            public const string TextureRecipeWindow = "recipe-window";
            public const string TextureMenuButtonUp = "ui-menu-btn-up";
            public const string TextureMenuButtonDown = "ui-menu-btn-down";
            public const string TextureMenuButtonHover = "ui-menu-btn-hover";



            // Sound effects
            public const string SoundEffectClick = "click";
            public const string SoundEffectSuccess = "success";
            public const string SoundEffectPlace = "place";

        }

        public SpriteFont Font { get; private set; }
        public SpriteFont TitleFont { get; private set; }

        public AnimatedTexture GetAnimatedTexture(string key)
        {
            return animatedTextures[key];
        }

        public Texture2D GetTexture(string key)
        {
            return textures[key];
        }

        public SoundEffect GetSoundEffect(string key)
        {
            return sounds[key];
        }

        private ContentLibrary() { }

        public static ContentLibrary Instance => instance.Value;
        private static readonly Lazy<ContentLibrary> instance = new(() => new());

        public static Indexer<Texture2D> Textures { get; } = new(instance.Value.textures);
        public static Indexer<SoundEffect> SoundEffects { get; } = new(instance.Value.sounds);
        public static Indexer<AnimatedTexture> Animations { get; } = new(instance.Value.animatedTextures);

        private readonly Dictionary<string, Texture2D> textures = new();
        private readonly Dictionary<string, SoundEffect> sounds = new();
        private readonly Dictionary<string, AnimatedTexture> animatedTextures = new();

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Font
            Font = content.Load<SpriteFont>("font");
            TitleFont = content.Load<SpriteFont>("title_font");

            // Misc
            textures[Keys.TextureChecker] = content.Load<Texture2D>("Images/checker");
            textures[Keys.TextureMap] = content.Load<Texture2D>("Images/map");
            textures[Keys.TexturePuffParticle] = content.Load<Texture2D>("Images/particle_puff");
            textures[Keys.TextureWhitePixel] = new Texture2D(graphicsDevice, 1, 1);
            textures[Keys.TextureWhitePixel].SetData(new[] { Color.White });

            // Tiles
            textures[Keys.TextureGrass] = content.Load<Texture2D>("Images/grass");
            textures[Keys.TextureDepositCoal] = content.Load<Texture2D>("Images/deposit_coal");
            textures[Keys.TextureDepositIron] = content.Load<Texture2D>("Images/deposit_iron");
            textures[Keys.TextureDepositCopper] = content.Load<Texture2D>("Images/deposit_copper");
            textures[Keys.TextureDepositSilicon] = content.Load<Texture2D>("Images/deposit_silicon");
            textures[Keys.TextureDepositOil] = content.Load<Texture2D>("Images/deposit_oil");
            animatedTextures[Keys.TextureDepositOil] = AnimatedTexture.Load(content, 200, "Images/deposit_oil", 5);

            // Buildings
            animatedTextures[Keys.TextureConveyorConnectorHorizontal] = AnimatedTexture.Load(content, 100, "BeltAnimation/horizontal");
            animatedTextures[Keys.TextureConveyorHorizontalLeft] = AnimatedTexture.Load(content, 100, "BeltAnimation/left");
            animatedTextures[Keys.TextureConveyorHorizontalRight] = AnimatedTexture.Load(content, 100, "BeltAnimation/right");
            animatedTextures[Keys.TextureConveyorConnectorVertical] = AnimatedTexture.Load(content, 100, "BeltAnimation/vertical", reverse: true);
            animatedTextures[Keys.TextureConveyorVerticalTop] = AnimatedTexture.Load(content, 100, "BeltAnimation/top", reverse: true);
            animatedTextures[Keys.TextureConveyorVerticalBottom] = AnimatedTexture.Load(content, 100, "BeltAnimation/bottom", reverse: true);
            animatedTextures[Keys.TextureConveyorConnectorTopLeft] = AnimatedTexture.Load(content, 100, "BeltAnimation/north-west");
            animatedTextures[Keys.TextureConveyorConnectorTopRight] = AnimatedTexture.Load(content, 100, "BeltAnimation/north-east");
            animatedTextures[Keys.TextureConveyorConnectorBottomLeft] = AnimatedTexture.Load(content, 100, "BeltAnimation/south-west");
            animatedTextures[Keys.TextureConveyorConnectorBottomRight] = AnimatedTexture.Load(content, 100, "BeltAnimation/south-east");
            textures[Keys.TextureDivider] = content.Load<Texture2D>("Images/divider");
            textures[Keys.TextureMerger] = content.Load<Texture2D>("Images/merger");
            textures[Keys.TextureExtractor] = content.Load<Texture2D>("Images/extractor");
            textures[Keys.TextureSmeltery] = content.Load<Texture2D>("Images/smeltery");
            textures[Keys.TextureAssembler] = content.Load<Texture2D>("Images/assembler");
            textures[Keys.TextureHub] = content.Load<Texture2D>("Images/hub");


            // Wares
            textures[Keys.TextureWareCoal] = content.Load<Texture2D>("Images/ware_coal");
            textures[Keys.TextureWareOreIron] = content.Load<Texture2D>("Images/ware_ironore");
            textures[Keys.TextureWareBarIron] = content.Load<Texture2D>("Images/ware_ironbar");
            textures[Keys.TextureWareOreCopper] = content.Load<Texture2D>("Images/ware_copperore");
            textures[Keys.TextureWareBarCopper] = content.Load<Texture2D>("Images/ware_copperbar");
            textures[Keys.TextureWareSilicon] = content.Load<Texture2D>("Images/ware_silicon");
            textures[Keys.TextureWareOil] = content.Load<Texture2D>("Images/ware_oil");
            textures[Keys.TextureWarePlastic] = content.Load<Texture2D>("Images/ware_plastic");
            textures[Keys.TextureWareGear] = content.Load<Texture2D>("Images/ware_gear");
            textures[Keys.TextureWareCopperCoil] = content.Load<Texture2D>("Images/ware_coppercoil");
            textures[Keys.TextureWareCircuit] = content.Load<Texture2D>("Images/ware_circuit");
            textures[Keys.TextureWareMotor] = content.Load<Texture2D>("Images/ware_motor");
            textures[Keys.TextureWareComputer] = content.Load<Texture2D>("Images/ware_computer");
            textures[Keys.TextureWareRobot] = content.Load<Texture2D>("Images/ware_robot");


            // UI
            textures[Keys.TextureBuildablesUI] = content.Load<Texture2D>("Images/ui_buildables");
            textures[Keys.TextureMenuUI] = content.Load<Texture2D>("Images/ui_menu");
            textures[Keys.TextureButtonUp] = content.Load<Texture2D>("Images/btn_normal");
            textures[Keys.TextureButtonDown] = content.Load<Texture2D>("Images/btn_down");
            textures[Keys.TextureButtonHover] = content.Load<Texture2D>("Images/btn_hover");
            textures[Keys.TextureActiveBorder] = content.Load<Texture2D>("Images/active_border");
            textures[Keys.TextureIconConveyor] = content.Load<Texture2D>("Images/icon_conveyor");
            textures[Keys.TextureIconDivider] = content.Load<Texture2D>("Images/icon_divider");
            textures[Keys.TextureIconMerger] = content.Load<Texture2D>("Images/icon_merger");
            textures[Keys.TextureIconExtractor] = content.Load<Texture2D>("Images/icon_extractor");
            textures[Keys.TextureIconSelect] = content.Load<Texture2D>("Images/icon_select_hand");
            textures[Keys.TextureIconRecipes] = content.Load<Texture2D>("Images/icon_recipes");
            textures[Keys.TextureIconRequests] = content.Load<Texture2D>("Images/icon_requests");
            textures[Keys.TextureDirectionArrow] = content.Load<Texture2D>("Images/direction_arrow");
            textures[Keys.TextureWindow] = content.Load<Texture2D>("Images/window");
            textures[Keys.TextureRecipeWindow] = content.Load<Texture2D>("Images/recipe_window");
            textures[Keys.TextureMenuButtonUp] = content.Load<Texture2D>("Images/menu_btn_up");
            textures[Keys.TextureMenuButtonDown] = content.Load<Texture2D>("Images/menu_btn_down");
            textures[Keys.TextureMenuButtonHover] = content.Load<Texture2D>("Images/menu_btn_hover");
            textures[Keys.TextureSplash] = content.Load<Texture2D>("Images/splash");

            // Sounds
            sounds[Keys.SoundEffectClick] = content.Load<SoundEffect>("Sounds/click");
            sounds[Keys.SoundEffectSuccess] = content.Load<SoundEffect>("Sounds/vgmenuselect");
            sounds[Keys.SoundEffectPlace] = content.Load<SoundEffect>("Sounds/place");
        }

        public class Indexer<T>
        {
            private readonly Dictionary<string, T> data;

            public Indexer(Dictionary<string, T> data)
            {
                this.data = data;
            }

            public T this[string key] => data[key];
        }
    }
}