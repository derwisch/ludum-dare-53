using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Elements
{
    internal abstract class CraftingPlant : StaticElement
    {
        protected readonly List<Recipe> recipes = new();

        protected WareType? lastCraftedWare;

        public CraftingPlant(Tile parent) : base(parent)
        {
            WareHandler = new WareHandler(1, 1, Array.Empty<Side>(), new[] { Side.Bottom }, parent);
        }
        public override int ZIndex => Constants.LayerBuildables;

        public override void Update(GameTime gameTime)
        {
            WareHandler.UpdateSlots();
            if (WareHandler.HasStorageSpace)
            {
                StaticElement leftInput = GameState.Current.World[tileX + -1, tileY]?.Building;
                StaticElement rightInput = GameState.Current.World[tileX + 1, tileY]?.Building;

                if (leftInput != null && rightInput != null)
                {
                    var leftWareTuple = leftInput?.WareHandler?.GetOutput(Side.Right);
                    var rightWareTuple = rightInput?.WareHandler?.GetOutput(Side.Left);

                    var leftIndex = leftWareTuple?.index;
                    var rightIndex = rightWareTuple?.index;

                    if (leftWareTuple?.element is Ware leftWare
                        && rightWareTuple?.element is Ware rightWare
                        && leftWare != rightWare
                        && recipes.Any(x => x.Inputs.Contains(leftWare.Type)
                                         && x.Inputs.Contains(rightWare.Type)))
                    {
                        var recipe = recipes.First(x => x.Inputs.Contains(leftWare.Type) && x.Inputs.Contains(rightWare.Type));

                        Ware result = new(recipe.Output);

                        lastCraftedWare = recipe.Output;

                        WareHandler.AddStorage(result);
                        leftInput.WareHandler.RemoveOutput(leftIndex.Value);
                        rightInput.WareHandler.RemoveOutput(rightIndex.Value);

                        RenderPool.Instance.RegisterRenderable(result);
                    }
                }
            }
        }
    }
}