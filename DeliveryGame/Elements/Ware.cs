using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Elements
{
    public class Ware : IRenderable
    {
        public bool IsVisible { get; set; }
        public float X { get; set; }
        public float Y { get; set; }


        private readonly Lazy<Dictionary<WareType, Texture2D>> textures = new(() => new()
        {
            { WareType.Coal, ContentLibrary.Textures[TextureWareCoal] },
            { WareType.IronOre, ContentLibrary.Textures[TextureWareOreIron] },
            { WareType.IronBar, ContentLibrary.Textures[TextureWareBarIron] },
            { WareType.CopperOre, ContentLibrary.Textures[TextureWareOreCopper] },
            { WareType.CopperBar, ContentLibrary.Textures[TextureWareBarCopper] },
            { WareType.Silicon, ContentLibrary.Textures[TextureWareSilicon] },
            { WareType.Oil, ContentLibrary.Textures[TextureWareOil] },
            { WareType.Plastic, ContentLibrary.Textures[TextureWarePlastic] },
            { WareType.Gear, ContentLibrary.Textures[TextureWareGear] },
            { WareType.CopperCoil, ContentLibrary.Textures[TextureWareCopperCoil] },
            { WareType.Circuit, ContentLibrary.Textures[TextureWareCircuit] },
            { WareType.Motor, ContentLibrary.Textures[TextureWareMotor] },
            { WareType.Computer, ContentLibrary.Textures[TextureWareComputer] },
            { WareType.Robot, ContentLibrary.Textures[TextureWareRobot] },
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