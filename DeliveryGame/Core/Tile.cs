using DeliveryGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Core
{
    public class Tile : IRenderable
    {
        private static readonly Lazy<Dictionary<TileType, Texture2D>> tileTextures = new(() => new Dictionary<TileType, Texture2D>()
        {
             { TileType.Grass, ContentLibrary.Textures[TextureGrass] },
             { TileType.DepositCoal, ContentLibrary. Textures[TextureDepositCoal] },
             { TileType.DepositIron, ContentLibrary.Textures[TextureDepositIron] },
             { TileType.DepositCopper, ContentLibrary.Textures[TextureDepositCopper] },
             { TileType.DepositSilicon, ContentLibrary.Textures[TextureDepositSilicon] },
             //{ TileType.DepositOil, ContentLibrary.Textures[TextureDepositOil] },
        });
        private static readonly Lazy<AnimatedTexture> oilTileAnimation = new(() => ContentLibrary.Animations[TextureDepositOil]);

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

            if (Type == TileType.DepositOil)
            {
                spriteBatch.Draw(oilTileAnimation.Value, rect, color);
            }
            else
            {
                spriteBatch.Draw(tileTextures.Value[Type], rect, color);
            }
        }

        public override string ToString()
        {
            return $"{Type}@({X}|{Y})";
        }

        public void ClearBuilding()
        {
            RenderPool.Instance.UnregisterRenderable(building);
            building.CleanUp();
            building = null;
        }

        public void SetBuilding(StaticElement building)
        {
            this.building = building;
            RenderPool.Instance.RegisterRenderable(building);
        }

        public void Update(GameTime gameTime)
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