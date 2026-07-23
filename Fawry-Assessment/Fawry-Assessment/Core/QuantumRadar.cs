using Fawry_Assessment.Core;
using Fawry_Assessment.Rules;

namespace Fawry_Assessment.Model
{
    public class QuantumRadar
    {
        private readonly List<IRule> _rules;
        private readonly List<Fine> _fineHistory = new();
        public QuantumRadar(IEnumerable<IRule> rules)
        {
            _rules = rules?.ToList() ?? new List<IRule>();
        }



        public void ProcessObservation(Observation observation)
        {
            var violations = new List<Violation>();

            foreach (var rule in _rules)
            {
                var violation = rule.Evaluate(observation);
                if (violation != null)
                {
                    violations.Add(violation);
                }
            }

            if (violations.Any())
            {
                var fine = new Fine(observation.PlateNumber, violations);
                _fineHistory.Add(fine);

                Console.WriteLine(fine.ToString());
                Console.WriteLine(); 
            }
        }

        public IEnumerable<(string PlateNumber, decimal TotalAmount)> GetAllFines()
        {
            return _fineHistory
                .GroupBy(f => f.PlateNumber)
                .Select(group => (
                    PlateNumber: group.Key,
                    TotalAmount: group.Sum(f => f.Total)
                ));
        }
        public void PrintAllFines()
        {
            Console.WriteLine("--- All Fines Summary ---");
            foreach (var record in GetAllFines())
            {
                Console.WriteLine($"Plate: {record.PlateNumber} | Total Amount: {record.TotalAmount} EGP");
            }
            Console.WriteLine();
        }

        public IEnumerable<(string RuleName, int Count)> GetViolatedRulesWithCounts()
        {
            return _fineHistory
                .SelectMany(f => f.Violations)
                .GroupBy(v => v.RuleName)
                .Select(group => (
                    RuleName: group.Key,
                    Count: group.Count()
                ));
        }

        public void PrintViolatedRulesWithCounts()
        {
            Console.WriteLine("--- Violated Rules Count ---");
            foreach (var record in GetViolatedRulesWithCounts())
            {
                Console.WriteLine($"Rule: {record.RuleName} | Count: {record.Count}");
            }
            Console.WriteLine();
        }
    }
}
