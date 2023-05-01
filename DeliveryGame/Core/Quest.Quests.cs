using DeliveryGame.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Core
{
    internal partial class Quest
    {
        private const string RewardAdditionalBaseExtractors = "extractor-iron-coal";
        private const string RewardAdvancedExtractors = "extractor-silicon-oil";
        private const string RewardAssemblers = "assembler";
        private const string RewardCopperExtractors = "extractor-copper";
        private const string RewardSmelteries = "smeltery";

        private static readonly Dictionary<string, Action> questRewards = new()
        {
            { RewardAdditionalBaseExtractors, QuestRewardAdditionalBaseExtractors },
            { RewardCopperExtractors, QuestRewardCopperExtractors },
            { RewardAdvancedExtractors, QuestRewardAdvancedExtractors },
            { RewardSmelteries, QuestRewardSmelteries },
            { RewardAssemblers, QuestRewardAssemblers }
        };

        static Quest()
        {
            QuestQueue.Enqueue(new(RewardCopperExtractors, (5, WareType.IronBar)));
            QuestQueue.Enqueue(new(RewardAdditionalBaseExtractors, (5, WareType.CopperBar)));
            QuestQueue.Enqueue(new(RewardAdvancedExtractors, (10, WareType.IronBar), (10, WareType.CopperBar)));
            QuestQueue.Enqueue(new(RewardSmelteries, (10, WareType.Gear), (10, WareType.CopperCoil), (10, WareType.Circuit)));
            QuestQueue.Enqueue(new(RewardAssemblers, (2, WareType.Motor)));
            QuestQueue.Enqueue(new(RewardAssemblers, (2, WareType.Computer)));
            QuestQueue.Enqueue(new(RewardAssemblers, (2, WareType.Robot)));
        }

        public static Queue<Quest> QuestQueue { get; } = new();
        public static List<Tile> RewardAssemblerTiles { get; } = new();
        public static List<Tile> RewardSmelteryTiles { get; } = new();

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
    }
}
