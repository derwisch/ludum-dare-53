using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DeliveryGame.Core
{
    public class AnimatedTexture
    {
        private readonly Texture2D[] frames;
        private int frameIndex = 0;

        public AnimatedTexture(int frameTime, Texture2D[] frames)
        {
            this.frames = frames;

            GameState.Current.GlobalUpdate += gameTime =>
            {
                frameIndex = (int)(gameTime.TotalGameTime.TotalMilliseconds / frameTime) % frames.Length;
            };
        }

        public Texture2D Reversed => frames[frames.Length - frameIndex - 1];
        public static implicit operator Texture2D(AnimatedTexture animatedTexture) => animatedTexture.frames[animatedTexture.frameIndex];
        public static AnimatedTexture Load(ContentManager content, int frameTime, string baseName, int frameCount = 4, bool reverse = false)
        {
            var range = Enumerable.Range(1, 4);

            if (reverse)
                range = range.Reverse();

            return new(frameTime, range.Select(x => $"{baseName}{x}")
                                       .Select(x => content.Load<Texture2D>(x))
                                       .ToArray());        }
    }
}
