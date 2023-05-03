using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Elements
{
    public class Hub : StaticElement
    {
        public Hub(Tile parent) : base(parent)
        {
            WareHandler = new(1, 1, Constants.AllSides, Array.Empty<Side>(), parent);
            DisplayName = "Delivery Hub";
            IsRemoveable = false;
        }

        public override string Info
        {
            get
            {
                return "This is the hub. \nDeliver the requested wares \nhere by conveyor belt.";
            }
        }

        public override int ZIndex => Constants.LayerBuildables;

        protected override Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Textures[TextureHub]);
        public override void Update(GameTime gameTime)
        {
            WareHandler.Update();

            if (WareHandler.Output.FirstOrDefault() is Ware ware)
            {
                GameState.Current.Deliver(ware);
                WareHandler.RemoveOutput(0);
            }
        }

        public override void CleanUp()
        {
        }
    }
}