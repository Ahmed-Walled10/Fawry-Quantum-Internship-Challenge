using Fawry_Assessment.Core;

namespace Fawry_Assessment.Rules
{
    public class SeatbeltRule : IRule
    {
        private const decimal Fee = 100m;
        public Violation? Evaluate(Observation observation)
        {
            if (!observation.SeatbeltStatus)
            {
                return new Violation(
                    ruleName: nameof(SeatbeltRule),
                    description: "Seatbelt not fastned",
                    fee: Fee
                );
            }

            return null;
        }
    }
}
