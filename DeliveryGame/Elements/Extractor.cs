using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DeliveryGame.Elements
{
    internal class Extractor : StaticElement
    {
        private ParticleSystem leftSmokeParticleSystem;
        private ParticleSystem rightSmokeParticleSystem;
        public Extractor(Tile parent) : base(parent)
        {
            WareHandler = new WareHandler(1, 4, Array.Empty<Side>(), Constants.AllSides, parent);
            DisplayName = "Extractor";
            IsRemoveable = false;

            leftSmokeParticleSystem = new(
                x: tileX * Constants.TileWidth + (0.25f * Constants.TileWidth),
                y: tileY * Constants.TileHeight - 2,
                particleAge: 1500,
                particleSpawnCooldown: 100,
                particleCount: 500,
                fadePercentage: 0.5f,
                particleSpeed: new Vector2(0.1f, -0.15f),
                particleTexture: ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TexturePuffParticle),
                particleColor: Color.Black,
                particleVariation: new Vector2(3, 2));

            rightSmokeParticleSystem = new(
                x: tileX * Constants.TileWidth + (0.65f * Constants.TileWidth),
                y: tileY * Constants.TileHeight - 2,
                particleAge: 1500,
                particleSpawnCooldown: 100,
                particleCount: 500,
                fadePercentage: 0.5f,
                particleSpeed: new Vector2(0.1f, -0.15f),
                particleTexture: ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TexturePuffParticle),
                particleColor: Color.Black,
                particleVariation: new Vector2(3, 2));

            RenderPool.Instance.RegisterRenderable(leftSmokeParticleSystem);
            RenderPool.Instance.RegisterRenderable(rightSmokeParticleSystem);
            ParticleSystem.RegisterSystem(leftSmokeParticleSystem);
            ParticleSystem.RegisterSystem(rightSmokeParticleSystem);
        }

        public override string Info
        {
            get
            {
                var ware = UI.UserInterface.GetWareDisplayName(MapTileTypeToWareType(parent.Type));
                return $"This extractor extracts: \n{ware}";
            }
        }

        public override int ZIndex => Constants.LayerBuildables;

        protected override Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureExtractor));
        public override void Update(GameTime gameTime)
        {
            WareHandler.UpdateSlots();
            if (WareHandler.HasStorageSpace)
            {
                WareType type = MapTileTypeToWareType(parent.Type);

                Ware result = new(type);
                RenderPool.Instance.RegisterRenderable(result);
                WareHandler.AddStorage(result);
            }
        }

        internal override void CleanUp()
        {
            RenderPool.Instance.UnregisterRenderable(leftSmokeParticleSystem);
            RenderPool.Instance.UnregisterRenderable(rightSmokeParticleSystem);
            ParticleSystem.UnregisterSystem(leftSmokeParticleSystem);
            ParticleSystem.UnregisterSystem(rightSmokeParticleSystem);
            leftSmokeParticleSystem = null;
            rightSmokeParticleSystem = null;
        }

        private static WareType MapTileTypeToWareType(TileType tileType) => tileType switch
        {
            TileType.DepositCoal => WareType.Coal,
            TileType.DepositIron => WareType.IronOre,
            TileType.DepositCopper => WareType.CopperOre,
            TileType.DepositSilicon => WareType.Silicon,
            TileType.DepositOil => WareType.Oil,
            _ => WareType.Coal
        };
    }
}