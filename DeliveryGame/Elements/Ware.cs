using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DeliveryGame.Elements
{
    internal class Ware : IRenderable
    {
        public bool IsVisible { get; set; }
        public float X { get; set; }
        public float Y { get; set; }


        private readonly Lazy<Dictionary<WareType, Texture2D>> textures = new(() => new()
        {
            { WareType.Coal, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareCoal) },
            { WareType.IronOre, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareOreIron) },
            { WareType.IronBar, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareBarIron) },
            { WareType.CopperOre, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareOreCopper) },
            { WareType.CopperBar, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareBarCopper) },
            { WareType.Silicon, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareSilicon) },
            { WareType.Oil, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareOil) },
            { WareType.Plastic, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWarePlastic) },
            { WareType.Gear, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareGear) },
            { WareType.CopperCoil, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareCopperCoil) },
            { WareType.Circuit, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareCircuit) },
            { WareType.Motor, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareMotor) },
            { WareType.Computer, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareComputer) },
            { WareType.Robot, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWareRobot) },
        });

        public Ware(WareType type)
        {
            Type = type;
        }

        public WareType Type { get; init; }
        public int ZIndex => Constants.LayerWares;
        public void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsVisible)
                return;

            var texture = textures.Value[Type];

            var origin = texture.Bounds.Center.ToVector2();
            spriteBatch.Draw(texture, GetRectangle(texture), null, Color.White, 0f, origin, SpriteEffects.None, 0);
        }

        public override string ToString()
        {
            return $"{Type}@({X}|{Y})";
        }

        protected virtual Rectangle GetRectangle(Texture2D texture)
        {
            float width = texture.Width * Camera.Instance.ZoomFactor;
            float height = texture.Height * Camera.Instance.ZoomFactor;

            float x = (X + Camera.Instance.OffsetX) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportWidth / 2);
            float y = (Y + Camera.Instance.OffsetY) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportHeight / 2);

            return new()
            {
                Location = new Point((int)Math.Floor(x), (int)Math.Floor(y)),
                Size = new Point((int)Math.Ceiling(width), (int)Math.Ceiling(height))
            };
        }
    }
}