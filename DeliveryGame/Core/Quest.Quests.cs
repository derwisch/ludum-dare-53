using DeliveryGame.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Core
{
    public partial class Quest
    {
        private const string RewardAdditionalBaseExtractors = "extractor-iron-coal";
        private const string RewardAdvancedExtractors = "extractor-silicon-oil";
        private const string RewardAssemblers = "assembler";
        private const string RewardCopperExtractors = "extractor-copper";
        private const string RewardSmelteries = "smeltery";
        private const string RewardFasterBelts = "belts";
        private const string RewardEmpty = "none";

        private static readonly Dictionary<string, Action> questRewards = new()
        {
            { RewardAdditionalBaseExtractors, QuestRewardAdditionalBaseExtractors },
            { RewardCopperExtractors, QuestRewardCopperExtractors },
            { RewardAdvancedExtractors, QuestRewardAdvancedExtractors },
            { RewardSmelteries, QuestRewardSmelteries },
            { RewardAssemblers, QuestRewardAssemblers },
            { RewardFasterBelts, QuestRewardFasterBelts },
            { RewardEmpty, QuestRewardEmpty },
        };

        static Quest()
        {
            QuestQueue.Enqueue(new(RewardCopperExtractors, (50, WareType.IronBar)));
            QuestQueue.Enqueue(new(RewardAdditionalBaseExtractors, (50, WareType.CopperBar)));
            QuestQueue.Enqueue(new(RewardAdvancedExtractors, (100, WareType.IronBar), (100, WareType.CopperBar)));
            QuestQueue.Enqueue(new(RewardSmelteries, (10, WareType.Gear), (10, WareType.CopperCoil), (10, WareType.Circuit)));
            QuestQueue.Enqueue(new(RewardAssemblers, (100, WareType.Gear), (100, WareType.CopperCoil), (100, WareType.Circuit)));
            QuestQueue.Enqueue(new(RewardEmpty, (5, WareType.Motor)));
            QuestQueue.Enqueue(new(RewardEmpty, (5, WareType.Computer)));
            QuestQueue.Enqueue(new(RewardFasterBelts, (5, WareType.Robot)));
            QuestQueue.Enqueue(new(RewardEmpty, (200, WareType.Motor), (200, WareType.Computer), (200, WareType.Robot)));
            QuestQueue.Enqueue(new(RewardEmpty));
        }

        public static Queue<Quest> QuestQueue { get; } = new();
        public static List<Tile> RewardAssemblerTiles { get; } = new();
        public static List<Tile> RewardSmelteryTiles { get; } = new();

        private static void QuestRewardEmpty()
        {
            // Do nothing
        }

        private static void QuestRewardAdditionalBaseExtractors()
        {
            foreach (var tile in GameState.Current.World.Tiles.Where(x => x.Type == TileType.DepositCoal && x.Building == null))
            {
                tile.SetBuilding(new Extractor(tile));
            }
            foreach (var tile in GameState.Current.World.Tiles.Where(x => x.Type == TileType.DepositIron && x.Building == null))
            {
                tile.SetBuilding(new Extractor(tile));
            }
        }

        private static void QuestRewardAdvancedExtractors()
        {
            foreach (var tile in GameState.Current.World.Tiles.Where(x => x.Type == TileType.DepositSilicon && x.Building == null))
            {
                tile.SetBuilding(new Extractor(tile));
            }
            foreach (var tile in GameState.Current.World.Tiles.Where(x => x.Type == TileType.DepositOil && x.Building == null))
            {
                tile.SetBuilding(new Extractor(tile));
            }
        }

        private static void QuestRewardAssemblers()
        {
            foreach (var tile in RewardAssemblerTiles)
            {
                tile.SetBuilding(new Assembler(tile));
            }
        }

        private static void QuestRewardCopperExtractors()
        {
            foreach (var tile in GameState.Current.World.Tiles.Where(x => x.Type == TileType.DepositCopper && x.Building == null))
            {
                tile.SetBuilding(new Extractor(tile));
            }
        }
        private static void QuestRewardSmelteries()
        {
            foreach (var tile in RewardSmelteryTiles)
            {
                tile.SetBuilding(new Smeltery(tile));
            }
        }

        private static void QuestRewardFasterBelts()
        {
            GameState.Current.ConveyorCooldown = 100;
        }
    }
}
