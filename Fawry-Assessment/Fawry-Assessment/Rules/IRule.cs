
using Fawry_Assessment.Core;

namespace Fawry_Assessment.Rules
{
    public interface IRule
    {
        Violation? Evaluate(Observation observation);

    }
}
