using Fawry_Assessment.Core;

namespace Fawry_Assessment.Rules
{
    public class PrivateSpeedRule : IRule
    {
        private const int SpeedLimit = 80;
        private const decimal Fee = 300m;

        public Violation? Evaluate(Observation observation)
        {
            if (observation.CarType == CarType.Private && observation.Speed > SpeedLimit)
            {
                return new Violation(
                    ruleName: nameof(PrivateSpeedRule),
                    description: $"speed of {observation.Speed} exceeded max allowed {SpeedLimit}",
                    fee: Fee
                );
            }

            return null;
        }
    }
}
