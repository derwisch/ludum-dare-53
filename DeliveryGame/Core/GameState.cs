using DeliveryGame.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Core
{
    public class GameState
    {
        public event Action<GameTime> GlobalUpdate;

        public static GameState Current { get; private set; } = new GameState();
        public double ConveyorCooldown { get; set; } = 300;

        public Quest CurrentQuest { get; set; }

        public bool DirectionArrowsVisible { get; set; } = true;

        public bool HasPlayerWon { get; private set; }

        public Tile HoveredTile { get; set; } = null;

        public bool IsGamePaused { get; set; }

        public BuildableType? SelectedBuildable { get; set; } = null;

        public StaticElement SelectedBuilding { get; set; } = null;

        public World World { get; set; }

        private GameMain game;
        public void Initialize(GameMain game)
        {
            this.game = game;
        }

        public void InvokeGlobalUpdate(GameTime gameTime)
        {
            GlobalUpdate?.Invoke(gameTime);
        }

        public void Quit()
        {
            game.Exit();
        }

        public void Deliver(Ware ware)
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

            if (CurrentQuest?.Requires(ware.Type) ?? false)
            {
                CurrentQuest.DeliveredWares[ware.Type]++;

                if (CurrentQuest.Completed)
                {
                    CurrentQuest.TriggerReward();
                    CurrentQuest = null;
                    ContentLibrary.SoundEffects[SoundEffectSuccess].Play(0.3f, 0, 0);
                }
            }
        }
    }
}