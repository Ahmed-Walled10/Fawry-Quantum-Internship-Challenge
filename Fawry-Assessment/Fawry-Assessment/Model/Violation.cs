
 namespace Fawry_Assessment.Core
{
    public class Violation
    {
        public string RuleName { get; }
        public string Description { get; }
        public decimal Fee { get; }

        public Violation(string ruleName, string description, decimal fee)
        {
            RuleName = ruleName;
            Description = description;
            Fee = fee;
        }

    }
}
