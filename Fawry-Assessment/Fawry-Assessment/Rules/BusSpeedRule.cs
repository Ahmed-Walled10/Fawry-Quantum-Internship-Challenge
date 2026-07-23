using Fawry_Assessment.Core;

namespace Fawry_Assessment.Rules
{
    public class BusSpeedRule : IRule
    {
        private const int SpeedLimit = 40;
        private const decimal Fee = 500m;

        public Violation? Evaluate(Observation observation)
        {
            if (observation.CarType == CarType.Bus && observation.Speed > SpeedLimit)
            {
                return new Violation(
                    ruleName: nameof(BusSpeedRule),
                    description: $"speed of {observation.Speed} exceeded max allowed {SpeedLimit}",
                    fee: Fee
                );
            }

            return null;
        }
    }
}
