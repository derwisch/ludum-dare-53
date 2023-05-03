using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    public partial class ParticleSystem : IRenderable
    {
        private readonly float fadePercentage;
        private readonly float particleAge;
        private readonly Color[] particleColor;
        private readonly int particleCount;
        private readonly List<Particle> particles = new();
        private readonly float particleSpawnCooldown;
        private readonly Vector2 particleSpeed;
        private readonly Texture2D particleTexture;
        private readonly Vector2 particleVariation;
        private readonly Random random = new();
        private readonly float x;
        private readonly float y;

        private double spawnCooldown = 0;

        public ParticleSystem(
            float x,
            float y,
            float particleAge,
            float particleSpawnCooldown,
            float fadePercentage,
            int particleCount,
            Vector2 particleSpeed,
            Vector2 particleVariation,
            Texture2D particleTexture,
            params Color[] particleColor)
        {
            this.x = x;
            this.y = y;
            this.particleCount = particleCount;
            this.particleSpeed = particleSpeed;
            this.particleAge = particleAge;
            this.particleSpawnCooldown = particleSpawnCooldown;
            this.fadePercentage = fadePercentage;
            this.particleTexture = particleTexture;
            this.particleColor = particleColor;
            this.particleVariation = particleVariation;
        }

        public int ZIndex => Constants.LayerParticles;

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var particle in particles)
            {
                float width = particleTexture.Width * Camera.Instance.ZoomFactor;
                float height = particleTexture.Height * Camera.Instance.ZoomFactor;

                float x = (particle.X + Camera.Instance.OffsetX) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportWidth / 2);
                float y = (particle.Y + Camera.Instance.OffsetY) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportHeight / 2);

                Rectangle particleRect = new()
                {
                    Location = new Point((int)Math.Floor(x), (int)Math.Floor(y)),
                    Size = new Point((int)Math.Ceiling(width), (int)Math.Ceiling(height))
                };

                Color color = particle.Color;

                if (particle.LifeTime < (particleAge * fadePercentage))
                {
                    var factor = particle.LifeTime / (particleAge * fadePercentage);

                    color.A = (byte)(255 * factor);
                }

                spriteBatch.Draw(particleTexture, particleRect, color);
            }
        }

        public void Update(GameTime gameTime)
        {
            spawnCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (particles.Count < particleCount && spawnCooldown <= 0)
            {
                particles.Add(new Particle()
                {
                    X = x,
                    Y = y,
                    LifeTime = particleAge,
                    Color = particleColor[random.Next(particleColor.Length)]
                });

                spawnCooldown = particleSpawnCooldown;
            }

            foreach (var particle in particles)
            {
                double angle = 2.0 * Math.PI * random.NextDouble();

                var randomX = (float)Math.Cos(angle) * particleSpeed.X * particleVariation.X;
                var randomY = (float)Math.Sin(angle) * particleSpeed.Y * particleVariation.Y;

                particle.X += particleSpeed.X + randomX;
                particle.Y += particleSpeed.Y + randomY;

                particle.LifeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            particles.RemoveAll(x => x.LifeTime <= 0);
        }

        private class Particle
        {
            public Color Color;
            public double LifeTime;
            public float X;
            public float Y;
        }
    }
}