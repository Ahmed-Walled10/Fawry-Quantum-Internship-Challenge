
using Fawry_Assessment.Core;

namespace Fawry_Assessment.Rules
{
    public class TruckSpeedRule :IRule
    {
        private const int SpeedLimit = 60;
        private const decimal Fee = 300m;

        public Violation? Evaluate(Observation observation)
        {
            if (observation.CarType == CarType.Truck && observation.Speed > SpeedLimit)
            {
                return new Violation(
                    ruleName: nameof(TruckSpeedRule),
                    description: $"speed of {observation.Speed} exceeded max allowed {SpeedLimit}",
                    fee: Fee
                );
            }

            return null;
        }
    }
}
