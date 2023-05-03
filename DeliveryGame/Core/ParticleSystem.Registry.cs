using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    public partial class ParticleSystem : IRenderable
    {
        private static readonly List<ParticleSystem> registeredParticleSystems = new();

        public static void UpdateParticleSystems(GameTime gameTime)
        {
            foreach (ParticleSystem particleSystem in registeredParticleSystems)
            {
                particleSystem.Update(gameTime);
            }
        }

        public static void RegisterSystem(ParticleSystem particleSystem)
        {
            registeredParticleSystems.Add(particleSystem);
        }

        public static void UnregisterSystem(ParticleSystem particleSystem)
        {
            registeredParticleSystems.Remove(particleSystem);
        }
    }
}