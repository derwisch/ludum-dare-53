namespace DeliveryGame.Elements
{
    internal class Recipe
    {
        public Recipe(WareType output, params WareType[] inputs)
        {
            Inputs = inputs;
            Output = output;
        }

        public WareType[] Inputs { get; init; }
        public WareType Output { get; init; }
    }
}