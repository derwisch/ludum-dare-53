using DeliveryGame.Elements;
using System.Linq;

namespace DeliveryGame.Core
{
    internal class GameState
    {
        public static GameState Current { get; private set; } = new GameState();

        public Quest CurrentQuest { get; set; }
        public bool DirectionArrowsVisible { get; set; } = true;
        public bool HasPlayerWon { get; private set; }
        public Tile HoveredTile { get; set; } = null;
        public bool IsGamePaused { get; set; }
        public BuildableType? SelectedBuildable { get; set; } = null;
        public StaticElement SelectedBuilding { get; set; } = null;
        public World World { get; set; }
        internal void Deliver(Ware ware)
        {
            if (CurrentQuest == null)
            {
                if (Quest.QuestQueue.Any())
                {
                    CurrentQuest = Quest.QuestQueue.Dequeue();
                }
                else
                {
                    HasPlayerWon = true;
                }
            }

            if (CurrentQuest.Requires(ware.Type))
            {
                CurrentQuest.DeliveredWares[ware.Type]++;

                if (CurrentQuest.Completed)
                {
                    CurrentQuest.TriggerReward();
                    CurrentQuest = null;
                }
            }
        }
    }
}