using System;
using System.Collections;
using System.Collections.Generic;

namespace DeliveryGame.Core
{
    internal class RenderPool : IEnumerable<IRenderable>
    {
        public static RenderPool Instance => instance.Value;
        private static readonly Lazy<RenderPool> instance = new(() => new());

        private readonly List<IRenderable> renderables = new();

        private RenderPool() { }

        public IEnumerator<IRenderable> GetEnumerator()
        {
            return renderables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return renderables.GetEnumerator();
        }

        public void RegisterRenderable(IRenderable renderable)
        {
            renderables.Add(renderable);
        }

        public void UnregisterRenderable(IRenderable renderable)
        {
            renderables.Remove(renderable);
        }
    }
}