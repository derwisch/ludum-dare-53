using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DeliveryGame.Core
{
    public interface IRenderable
    {
        int ZIndex { get; }

        void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime);
    }
}