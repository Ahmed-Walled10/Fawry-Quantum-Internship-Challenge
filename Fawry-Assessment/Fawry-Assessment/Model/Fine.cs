using System.Text;

namespace Fawry_Assessment.Core
{
    public class Fine
    {
        public string PlateNumber { get; }
        public List<Violation> Violations { get; }
        public decimal Total => Violations.Sum(v => v.Fee);

        public Fine(string plateNumber, List<Violation> violations)
        {
            PlateNumber = plateNumber;
            Violations = violations ?? new List<Violation>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Traffic fine for car {PlateNumber}");
            sb.AppendLine($"Total amount: {Total} EGP");
            sb.AppendLine("Violations:");

            foreach (var violation in Violations)
            {
                sb.AppendLine($"- {violation.Description} : {violation.Fee} EGP");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
