using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DeliveryGame.UI
{
    public class Window : IRenderable
    {
        private static readonly Lazy<SpriteFont> font = new(() => ContentLibrary.Instance.Font);

        private static readonly Lazy<SpriteFont> titleFont = new(() => ContentLibrary.Instance.TitleFont);

        private readonly Lazy<Texture2D> windowTexture = new(() => ContentLibrary.Textures[ContentLibrary.Keys.TextureWindow]);

        public Window(Texture2D texture = null)
        {
            InputState.Instance.LeftClick += MouseLeftClick;

            if (texture != null)
            {
                windowTexture = new(() => texture);
            }
        }

        public event Action Closed;

        public bool IsVisible { get; private set; }
        public (int x, int y) Offset { get; set; }

        public string Text { get; set; } = "";
        
        public string Title { get; set; } = "";

        public Rectangle WindowArea => new()
        {
            X = Camera.Instance.ViewportWidth - windowTexture.Value.Width + Offset.x,
            Y = Offset.y,
            Width = windowTexture.Value.Width,
            Height = windowTexture.Value.Height
        };

        public int ZIndex => Constants.LayerUIWindows;

        private Vector2 TextPosition => new()
        {
            X = Camera.Instance.ViewportWidth - windowTexture.Value.Width + Offset.x + 10,
            Y = Offset.y + 28
        };

        private Vector2 TitlePosition => new()
        {
            X = Camera.Instance.ViewportWidth - windowTexture.Value.Width + Offset.x + 5,
            Y = Offset.y + 2
        };

        private Rectangle WindowCloseArea => new()
        {
            X = Camera.Instance.ViewportWidth + Offset.x - 18,
            Y = Offset.y,
            Width = 18,
            Height = 18
        };

        public void Hide()
        {
            RenderPool.Instance.UnregisterRenderable(this);
            IsVisible = false;
        }

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(windowTexture.Value, WindowArea, Color.White);
            spriteBatch.DrawString(titleFont.Value, Title, TitlePosition, Color.Black);
            spriteBatch.DrawString(font.Value, Text, TextPosition, Color.Black);
        }

        public void Show()
        {
            RenderPool.Instance.RegisterRenderable(this);
            IsVisible = true;
        }

        private void MouseLeftClick()
        {
            if (WindowCloseArea.Contains(InputState.Instance.MouseState.Position))
            {
                Hide();
                Closed?.Invoke();
            }
        }
    }
}