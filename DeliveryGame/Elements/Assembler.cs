using DeliveryGame.Core;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DeliveryGame.Elements
{
    internal class Assembler : CraftingPlant
    {
        public Assembler(Tile parent) : base(parent)
        {
            // Basic
            recipes.Add(new(WareType.Gear, WareType.IronBar, WareType.IronBar));
            recipes.Add(new(WareType.CopperCoil, WareType.CopperBar, WareType.CopperBar));
            recipes.Add(new(WareType.Circuit, WareType.Plastic, WareType.Silicon));
            // Advanced
            recipes.Add(new(WareType.Motor, WareType.Gear, WareType.CopperCoil));
            recipes.Add(new(WareType.Computer, WareType.Circuit, WareType.CopperCoil));
            recipes.Add(new(WareType.Robot, WareType.Circuit, WareType.Gear));

            DisplayName = "Assembly Plant";
            IsRemoveable = false;
        }

        public override string Info
        {
            get
            {
                var ware = UI.UserInterface.GetWareDisplayName(lastCraftedWare);

                if (String.IsNullOrWhiteSpace(ware))
                {
                    ware = "<nothing yet>";
                }

                return $"Currently producing: \n{ware}";
            }
        }

        protected override Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureAssembler));
        internal override void CleanUp()
        {
        }
    }
}