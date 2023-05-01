using DeliveryGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Elements
{
    internal class WareHandler
    {
        private readonly Ware[] output;
        private readonly Tile parentTile;
        private readonly Ware[] storage;
        private Side[] inputSides;
        private Side[] outputSides;
        public WareHandler(int storageCount, int outputCount, Side[] inputSides, Side[] outputSides, Tile parentTile)
        {
            storage = new Ware[storageCount];
            output = new Ware[outputCount];
            this.inputSides = inputSides;
            this.outputSides = outputSides;
            this.parentTile = parentTile;
        }

        public bool HasOutputSpace => output.Any(x => x == null);
        public bool HasStorageSpace => storage.Any(x => x == null);
        public IEnumerable<Ware> Output => output;
        public IEnumerable<Ware> Storage => storage;
        public void AddStorage(Ware element)
        {
            var index = Array.IndexOf(storage, null);
            storage[index] = element;
        }

        public void CleanUp()
        {
            foreach (var element in storage)
            {
                RenderPool.Instance.UnregisterRenderable(element);
            }
            foreach (var element in output)
            {
                RenderPool.Instance.UnregisterRenderable(element);
            }
        }

        public (Ware element, int index) GetOutput(Side side)
        {
            var outputIndex = Array.IndexOf(outputSides, side);
            if (outputIndex == -1)
                return (null, -1);
            return (output[outputIndex], outputIndex);
        }

        public void RemoveOutput(int index)
        {
            RenderPool.Instance.UnregisterRenderable(output[index]);
            output[index] = null;
        }

        public void UpdateInputSides(Side[] sides)
        {
            inputSides = sides;
        }

        public void UpdateOutputSides(Side[] sides)
        {
            outputSides = sides;
        }

        public void UpdateSlots()
        {
            if (storage.Any(x => x != null) && output.Any(x => x == null))
            {
                // TODO: Balance or rotate through input/outputs
                var firstEmptyOutput = Array.IndexOf(output, null);
                var firstStorageElement = storage.First(x => x != null);
                var storageIndex = Array.IndexOf(storage, firstStorageElement);

                output[firstEmptyOutput] = firstStorageElement;
                storage[storageIndex] = null;
            }

            if (HasStorageSpace)
            {
                var (input, side) = GetInputHandler();
                if (input != null)
                {
                    PullFrom(input, side);
                }
            }
        }

        private static bool CanPullFrom(WareHandler other, Side inputSide)
        {
            if (other == null)
                return false;

            return other.HasOutputAt(ReverseSide(inputSide));
        }

        private static Side ReverseSide(Side side)
        {
            return side switch
            {
                Side.Top => Side.Bottom,
                Side.Right => Side.Left,
                Side.Bottom => Side.Top,
                Side.Left => Side.Right,
                _ => default,
            };
        }

        private (WareHandler handler, Side inputSide) GetInputHandler()
        {
            foreach (var inputSide in inputSides)
            {
                (int offsetX, int offsetY) = inputSide switch
                {
                    Side.Top => (0, -1),
                    Side.Right => (1, 0),
                    Side.Bottom => (0, 1),
                    Side.Left => (-1, 0),
                    _ => (0, 0)
                };

                var inputTile = GameState.Current.World[parentTile.X + offsetX, parentTile.Y + offsetY];

                if (inputTile == null)
                    continue;

                var building = inputTile.Building;
                if (building == null)
                    continue;

                var otherHandler = building.WareHandler;
                if (otherHandler == null)
                    continue;

                if (!CanPullFrom(otherHandler, inputSide))
                    continue;

                if (otherHandler.GetOutput(ReverseSide(inputSide)).element == null)
                    continue;

                return (otherHandler, inputSide);
            }
            return (null, default);
        }

        private bool HasOutputAt(Side side) => outputSides.Any(x => x == side);
        private void PullFrom(WareHandler other, Side inputSide)
        {
            var outputSide = ReverseSide(inputSide);
            var (element, index) = other.GetOutput(outputSide);

            AddStorage(element);
            other.output[index] = null;
        }
    }
}