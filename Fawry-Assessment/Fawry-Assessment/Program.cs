using Fawry_Assessment.Core;
using Fawry_Assessment.Model;
using Fawry_Assessment.Rules;

namespace Fawry_Assessment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rules = new List<IRule>
            {
                new PrivateSpeedRule(),
                new TruckSpeedRule(),
                new BusSpeedRule(),
                new SeatbeltRule()
            };

            var radar = new QuantumRadar(rules);

            var observations = new List<Observation>
            {
                new Observation("ABC1000", DateTime.Now, CarType.Bus, 70, true),

                new Observation("XYZ9999", DateTime.Now, CarType.Private, 65, false),

                new Observation("TRK5555", DateTime.Now, CarType.Truck, 75, true),

                new Observation("ABC1234", DateTime.Now, CarType.Private, 94, false),

                new Observation("ABC1234", DateTime.Now.AddMinutes(15), CarType.Private, 85, true)
            };

            Console.WriteLine("=== PROCESSING RADAR OBSERVATIONS ===\n");
            foreach (var obs in observations)
            {
                radar.ProcessObservation(obs);
            }

            Console.WriteLine("=== REPORT 1: ALL FINES BY PLATE NUMBER ===");
            radar.PrintAllFines();

            Console.WriteLine("=== REPORT 2: VIOLATED RULES COUNTS ===");
            radar.PrintViolatedRulesWithCounts();
        }
    }
}