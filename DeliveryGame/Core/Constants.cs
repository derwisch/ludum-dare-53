using DeliveryGame.Elements;

namespace DeliveryGame.Core
{
    internal class Constants
    {
        public const int ScrollSpeed = 10;
        public const int ScrollBorderWidth = 18;

        public const int ButtonWidth = 48;
        public const int ButtonHeight = 48;

        public const int LayerBuildables = 1;
        public const int LayerWares = 2;
        public const int LayerParticles = 1000;
        public const int LayerUI = 5000;

        public const double ConveyorCooldown = 500;
        public static readonly Side[] AllSides = new[] { Side.Left, Side.Top, Side.Right, Side.Bottom };

        public const int TileWidth = 64;
        public const int TileHeight = 64;

        public const int MapWidth = 50;
        public const int MapHeight = 25;

        public const int CoalDeposits = 10;
        public const int IronDeposits = 5;
        public const int CopperDeposits = 3;
        public const int SiliconDeposits = 2;
        public const int OilDeposits = 2;

        public const int Smelteries = 8;
        public const int Assemblers = 8;
        public const int StartingCoalExtractors = 1;
        public const int StartingIronExtractors = 1;
    }
}