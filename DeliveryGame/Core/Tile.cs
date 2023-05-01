using DeliveryGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    internal class Tile : IRenderable
    {
        private static readonly Lazy<Dictionary<TileType, Texture2D>> tileTextures = new(() => new Dictionary<TileType, Texture2D>()
        {
             { TileType.Grass, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureGrass) },
             { TileType.DepositCoal, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDepositCoal) },
             { TileType.DepositIron, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDepositIron) },
             { TileType.DepositCopper, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDepositCopper) },
             { TileType.DepositSilicon, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDepositSilicon) },
             { TileType.DepositOil, ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDepositOil) }
        });

        private StaticElement building;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public StaticElement Building => building;
        public TileType Type { get; set; }
        public int X { get; private init; }
        public int Y { get; private init; }
        public int ZIndex => 0;

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle rect = GetRectangle();
            Color color = GameState.Current.HoveredTile == this ? Color.Gray : Color.White;

            spriteBatch.Draw(tileTextures.Value[Type], rect, color);
        }

        public override string ToString()
        {
            return $"{Type}@({X}|{Y})";
        }

        internal void ClearBuilding()
        {
            RenderPool.Instance.UnregisterRenderable(building);
            building.CleanUp();
            building = null;
        }

        internal void SetBuilding(StaticElement building)
        {
            this.building = building;
            RenderPool.Instance.RegisterRenderable(building);
        }

        internal void Update(GameTime gameTime)
        {
            var tileArea = GetRectangle();
            var mouseState = InputState.Instance.MouseState;

            if (tileArea.Contains(mouseState.Position))
            {
                GameState.Current.HoveredTile = this;
            }
            building?.Update(gameTime);
        }

        private Rectangle GetRectangle()
        {
            float width = Constants.TileWidth * Camera.Instance.ZoomFactor;
            float height = Constants.TileHeight * Camera.Instance.ZoomFactor;

            float x = ((X * Constants.TileWidth) + Camera.Instance.OffsetX) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportWidth / 2);
            float y = ((Y * Constants.TileHeight) + Camera.Instance.OffsetY) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportHeight / 2);

            return new()
            {
                Location = new Point((int)Math.Floor(x), (int)Math.Floor(y)),
                Size = new Point((int)Math.Ceiling(width), (int)Math.Ceiling(height))
            };
        }
    }
}