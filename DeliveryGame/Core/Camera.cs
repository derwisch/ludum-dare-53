using Microsoft.Xna.Framework.Graphics;
using System;

namespace DeliveryGame.Core
{
    public class Camera
    {
        public static Camera Instance => instance.Value;
        private static readonly Lazy<Camera> instance = new(() => new());

        private const int minimumOffsetX = -(Constants.TileWidth * Constants.MapWidth);
        private const int minimumOffsetY = -(Constants.TileHeight * Constants.MapHeight);

        private float offsetX;
        private float offsetY;

        private Camera()
        {
            OffsetX = 0;
            OffsetY = 0;
            ZoomFactor = 1;
        }

        public float OffsetX
        {
            get => offsetX;
            set
            {
                offsetX = value;
                if (offsetX > 0)
                    offsetX = 0;
                if (offsetX < minimumOffsetX)
                    offsetX = minimumOffsetX;
            }
        }

        public float OffsetY
        {
            get => offsetY; set
            {
                offsetY = value;
                if (offsetY > 0)
                    offsetY = 0;
                if (offsetY < minimumOffsetY)
                    offsetY = minimumOffsetY;
            }
        }

        public int ViewportHeight { get; private set; }
        public int ViewportWidth { get; private set; }
        public float ZoomFactor { get; set; }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            Update(graphicsDevice);

            OffsetX = -ViewportWidth / 2;
            OffsetY = -ViewportHeight / 2;
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            ViewportWidth = graphicsDevice.Viewport.Width;
            ViewportHeight = graphicsDevice.Viewport.Height;
        }
    }
}