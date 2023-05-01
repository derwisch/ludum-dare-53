using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DeliveryGame.Elements
{
    internal class Smeltery : CraftingPlant
    {
        private ParticleSystem centerFlameParticleSystem;
        private ParticleSystem leftFlameParticleSystem;
        private ParticleSystem rightFlameParticleSystem;

        public Smeltery(Tile parent) : base(parent)
        {
            recipes.Add(new(WareType.IronBar, WareType.Coal, WareType.IronOre));
            recipes.Add(new(WareType.CopperBar, WareType.Coal, WareType.CopperOre));
            recipes.Add(new(WareType.Plastic, WareType.Coal, WareType.Oil));

            DisplayName = "Smelting Plant";
            IsRemoveable = false;
            InitializeParticleSystems();
        }

        public override string Info
        {
            get
            {
                var ware = UI.UserInterface.GetWareDisplayName(lastCraftedWare);

                if (String.IsNullOrWhiteSpace(ware))
                {
                    ware = "<nothing yet>";
                }

                return $"Currently producing: \n{ware}";
            }
        }

        protected override Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureSmeltery));
        internal override void CleanUp()
        {
            RenderPool.Instance.UnregisterRenderable(leftFlameParticleSystem);
            RenderPool.Instance.UnregisterRenderable(centerFlameParticleSystem);
            RenderPool.Instance.UnregisterRenderable(rightFlameParticleSystem);
            ParticleSystem.UnregisterSystem(leftFlameParticleSystem);
            ParticleSystem.UnregisterSystem(centerFlameParticleSystem);
            ParticleSystem.UnregisterSystem(rightFlameParticleSystem);
            leftFlameParticleSystem = null;
            centerFlameParticleSystem = null;
            rightFlameParticleSystem = null;
        }

        private void InitializeParticleSystems()
        {
            var particleAge = 300;
            var cooldown = 150;
            var fadePercentage = 0.75f;
            var colors = new[] { Color.Red, Color.OrangeRed, Color.Orange };

            leftFlameParticleSystem = new(
                x: tileX * Constants.TileWidth + (0.35f * Constants.TileWidth),
                y: tileY * Constants.TileHeight + (0.10f * Constants.TileWidth),
                particleAge: particleAge,
                particleSpawnCooldown: cooldown,
                fadePercentage: fadePercentage,
                particleCount: 50,
                particleSpeed: new Vector2(0.01f, -0.1f),
                particleVariation: new Vector2(10, 2),
                particleTexture: ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TexturePuffParticle),
                particleColor: colors
            );

            centerFlameParticleSystem = new(
                x: tileX * Constants.TileWidth + (0.45f * Constants.TileWidth),
                y: tileY * Constants.TileHeight + (0.10f * Constants.TileWidth),
                particleAge: particleAge,
                particleSpawnCooldown: cooldown,
                fadePercentage: fadePercentage,
                particleCount: 50,
                particleSpeed: new Vector2(0.01f, -0.1f),
                particleVariation: new Vector2(10, 2),
                particleTexture: ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TexturePuffParticle),
                particleColor: colors
            );

            rightFlameParticleSystem = new(
                x: tileX * Constants.TileWidth + (0.55f * Constants.TileWidth),
                y: tileY * Constants.TileHeight + (0.10f * Constants.TileWidth),
                particleAge: particleAge,
                particleSpawnCooldown: cooldown,
                fadePercentage: fadePercentage,
                particleCount: 50,
                particleSpeed: new Vector2(0.01f, -0.1f),
                particleVariation: new Vector2(10, 2),
                particleTexture: ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TexturePuffParticle),
                particleColor: colors
            );

            RenderPool.Instance.RegisterRenderable(leftFlameParticleSystem);
            RenderPool.Instance.RegisterRenderable(centerFlameParticleSystem);
            RenderPool.Instance.RegisterRenderable(rightFlameParticleSystem);
            ParticleSystem.RegisterSystem(leftFlameParticleSystem);
            ParticleSystem.RegisterSystem(centerFlameParticleSystem);
            ParticleSystem.RegisterSystem(rightFlameParticleSystem);
        }
    }
}