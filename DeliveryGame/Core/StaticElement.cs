using DeliveryGame.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Core
{
    public abstract class StaticElement : IRenderable
    {
        protected readonly Tile parent;
        protected readonly int tileX;
        protected readonly int tileY;

        public StaticElement(Tile parent)
        {
            tileX = parent.X;
            tileY = parent.Y;
            this.parent = parent;
        }

        public string DisplayName { get; init; }
        public abstract string Info { get; }
        public bool IsRemoveable { get; init; }

        // Implement some kind of capability system? -> Not nessecary yet
        public WareHandler WareHandler { get; init; }

        public abstract int ZIndex { get; }

        protected virtual Rectangle StaticElementArea
        {
            get
            {
                float width = Constants.TileWidth * Camera.Instance.ZoomFactor;
                float height = Constants.TileHeight * Camera.Instance.ZoomFactor;

                float x = ((tileX * Constants.TileWidth) + Camera.Instance.OffsetX) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportWidth / 2);
                float y = ((tileY * Constants.TileHeight) + Camera.Instance.OffsetY) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportHeight / 2);

                return new()
                {
                    Location = new Point((int)Math.Floor(x), (int)Math.Floor(y)),
                    Size = new Point((int)Math.Ceiling(width), (int)Math.Ceiling(height))
                };
            }
        }

        protected virtual Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Textures[TextureChecker]);

        public virtual void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(TextureProducer.Value, StaticElementArea, Color.White);
        }

        public abstract void Update(GameTime gameTime);

        public abstract void CleanUp();
    }
}