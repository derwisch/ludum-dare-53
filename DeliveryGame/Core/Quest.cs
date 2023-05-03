using DeliveryGame.Elements;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Core
{
    public partial class Quest
    {
        private readonly (int count, WareType type)[] requestedWares;
        private readonly string rewardKey;
        public Quest(string rewardKey, params (int count, WareType type)[] wares)
        {
            this.rewardKey = rewardKey;
            requestedWares = wares;
            foreach (var (_, type) in wares)
            {
                DeliveredWares[type] = 0;
            }
        }

        public bool Completed => requestedWares.All(x => DeliveredWares[x.type] == x.count);
        public Dictionary<WareType, int> DeliveredWares { get; } = new();
        public IEnumerable<(int count, WareType type)> RequestedWares => requestedWares;
        public bool Requires(WareType type) => requestedWares.Any(x => x.type == type) && DeliveredWares[type] < requestedWares.FirstOrDefault(x => x.type == type).count;

        public void TriggerReward()
        {
            questRewards[rewardKey]();
        }
    }
}