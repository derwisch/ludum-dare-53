using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    internal class ContentLibrary
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

            // Static Elements
            public const string TextureConveyorHorizontal = "conveyor-horizontal";
            public const string TextureConveyorVertical = "conveyor-vertical";
            public const string TextureConveyorTopLeft = "conveyor-top-left";
            public const string TextureConveyorTopRight = "conveyor-top-right";
            public const string TextureConveyorBottomLeft = "conveyor-bottom-left";
            public const string TextureConveyorBottomRight = "conveyor-bottom-right";
            public const string TextureDivider = "divider";
            public const string TextureMerger = "merger";
            public const string TextureExtractor = "extractor";
            public const string TextureSmeltery = "smeltery";
            public const string TextureAssembler = "assembler";
            public const string TextureHub = "hub";
            
            // Dynamic Elements
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
            public const string TextureActiveBorder = "ui-active-border";
            public const string TextureButtonHover = "ui-btn-hover";
            public const string TextureIconConveyor = "ui-icon-conveyor";
            public const string TextureIconDivider = "ui-icon-divider";
            public const string TextureIconMerger = "ui-icon-merger";
            public const string TextureIconExtractor = "ui-icon-extractor";
            public const string TextureIconSelect = "ui-icon-select";
            public const string TextureIconRecipes = "ui-icon-recipes";
            public const string TextureIconRequests = "ui-icon-requests";
            public const string TextureDirectionArrow = "direction-arrow";
            public const string TextureWindow = "window";
        }

        public SpriteFont Font { get; private set; }
        public SpriteFont TitleFont { get; private set; }

        internal Texture2D GetTexture(string key)
        {
            return textures[key];
        }

        private ContentLibrary() { }

        public static ContentLibrary Instance => instance.Value;
        private static readonly Lazy<ContentLibrary> instance = new(() => new());


        private readonly Dictionary<string, Texture2D> textures = new();

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

            // Static Elements
            textures[Keys.TextureConveyorHorizontal] = content.Load<Texture2D>("Images/conveyor_we");
            textures[Keys.TextureConveyorVertical] = content.Load<Texture2D>("Images/conveyor_ns");
            textures[Keys.TextureConveyorTopLeft] = content.Load<Texture2D>("Images/conveyor_nw");
            textures[Keys.TextureConveyorTopRight] = content.Load<Texture2D>("Images/conveyor_ne");
            textures[Keys.TextureConveyorBottomLeft] = content.Load<Texture2D>("Images/conveyor_ws");
            textures[Keys.TextureConveyorBottomRight] = content.Load<Texture2D>("Images/conveyor_es");
            textures[Keys.TextureDivider] = content.Load<Texture2D>("Images/divider");
            textures[Keys.TextureMerger] = content.Load<Texture2D>("Images/merger");
            textures[Keys.TextureExtractor] = content.Load<Texture2D>("Images/extractor");
            textures[Keys.TextureSmeltery] = content.Load<Texture2D>("Images/smeltery");
            textures[Keys.TextureAssembler] = content.Load<Texture2D>("Images/assembler");
            textures[Keys.TextureHub] = content.Load<Texture2D>("Images/hub");


            // Dynamic Elements
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
        }
    }
}