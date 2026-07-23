# Fawry Quantum Radar

## 1. What this project does

This is a small console application that simulates a traffic radar system.
It takes a stream of **observations** (a car passing the radar at a given
speed, with a given seatbelt status and car type), checks each observation
against a set of **rules**, and — if any rule is broken — generates a
**fine** for that plate, made up of one or more **violations**.

At the end, it prints two reports:

- **All fines grouped by plate number**, with the total amount owed.
- **A count of how many times each rule was violated**, across all cars.

The rules implemented are:

| Rule | Condition | Fee |
|---|---|---|
| `PrivateSpeedRule` | Private car speed > 80 | 300 EGP |
| `TruckSpeedRule` | Truck speed > 60 | 300 EGP |
| `BusSpeedRule` | Bus speed > 40 | 500 EGP |
| `SeatbeltRule` | Seatbelt not fastened (any car type) | 100 EGP |

## 2. Project structure

```
Fawry-Assessment/
├── Program.cs                 # Entry point: builds rules, feeds observations, prints reports
├── Core/
│   └── QuantumRadar.cs         # The orchestrator: evaluates observations, stores fines, builds reports
├── Model/
│   ├── CarType.cs              # Enum: Private, Truck, Bus
│   ├── Observation.cs          # A single radar reading (plate, time, type, speed, seatbelt)
│   ├── Violation.cs            # A single broken rule (name, description, fee)
│   └── Fine.cs                 # A collection of violations for one plate, with a total
└── Rules/
    ├── IRule.cs                 # Contract every rule must satisfy
    ├── PrivateSpeedRule.cs
    ├── TruckSpeedRule.cs
    ├── BusSpeedRule.cs
    └── SeatbeltRule.cs
```

## 3. Key design decisions — and why

### 3.1 Rules are pluggable objects behind an `IRule` interface, not `if/switch` logic

Instead of writing one method that checks `CarType` with a big
`switch`/`if-else` chain and different speed limits inline, each rule is its
own class that implements:

```csharp
public interface IRule
{
    Violation? Evaluate(Observation observation);
}
```

**Why I did it this way:**
- Each rule is self-contained: it owns its own speed limit and fee as
  private constants, and decides on its own whether it fires.
- Adding a new rule (e.g. a "no phone while driving" rule, or a motorcycle
  speed rule) means **adding a new class**, not touching existing code.
  This is the **Open/Closed Principle** (open for extension, closed for
  modification) — `QuantumRadar` never needs to change when a new rule is
  introduced.
- It's an application of the **Strategy pattern**: `QuantumRadar` doesn't
  know or care *how* a rule decides a violation — it just calls
  `Evaluate()` and reacts to the result. The evaluation strategy is
  swappable.

**Why I didn't do it the "simple" way (one big method with `if/else` on `CarType`):**
- It would work for 3-4 car types, but every new rule would mean editing a
  shared method, which increases the risk of breaking an existing,
  already-tested rule while adding a new one.
- It would violate the **Single Responsibility Principle** — one method
  would be responsible for speed limits *and* seatbelt logic *and* whatever
  comes next, instead of each concern living in its own class.
- It's harder to unit test in isolation: with separate rule classes, I can
  test `BusSpeedRule` completely independently of `PrivateSpeedRule` or of
  `QuantumRadar` itself.

### 3.2 `QuantumRadar` receives its rules through the constructor (Dependency Injection)

```csharp
public QuantumRadar(IEnumerable<IRule> rules)
{
    _rules = rules?.ToList() ?? new List<IRule>();
}
```

**Why:** `QuantumRadar` depends on the `IRule` **abstraction**, not on any
concrete rule class. This is the **Dependency Inversion Principle**. The
caller (`Program.cs`) decides which rules are active and in what
combination — `QuantumRadar` itself has zero knowledge of
`PrivateSpeedRule`, `BusSpeedRule`, etc. That means:
- I can enable/disable rules per run without touching `QuantumRadar`.
- I can unit test `QuantumRadar` with fake/mock rules, without needing the
  real speed-limit logic.

**Why not a static list hardcoded inside `QuantumRadar`:** that would
tightly couple the orchestrator to every concrete rule, make it impossible
to test in isolation, and would need a code change (and recompilation of
that class) every time the set of active rules changes.

### 3.3 `Violation` and `Fine` are separate, small, immutable-ish models

- A `Violation` is a single broken rule (name, description, fee).
- A `Fine` is *a plate's* violations for one observation, and exposes
  `Total` as a computed property (`Violations.Sum(v => v.Fee)`) instead of
  a stored field.

**Why:** `Total` is **derived data** — it should never be able to drift out
of sync with the underlying violations, so I compute it on demand rather
than storing and updating it manually. This avoids a whole class of bugs
where someone adds a violation but forgets to update a stored total.

I kept `Violation` and `Fine` as separate classes (rather than folding fee
totals directly into `Observation`) to keep **each class responsible for
one thing**: `Observation` describes *what the radar saw*, `Violation`
describes *what rule was broken*, and `Fine` describes *what a plate owes
for one incident*. Mixing these would blur what each object represents.

### 3.4 Reporting logic lives in `QuantumRadar`, using LINQ grouping instead of manual dictionaries

```csharp
_fineHistory
    .GroupBy(f => f.PlateNumber)
    .Select(group => (group.Key, group.Sum(f => f.Total)));
```

**Why:** LINQ's `GroupBy`/`Select` expresses the *intent* ("group fines by
plate, sum their totals") directly, rather than manually building and
maintaining a `Dictionary<string, decimal>` with `if (dict.ContainsKey(...))`
bookkeeping. It's shorter, less error-prone, and reads closer to the
business requirement itself.

I considered maintaining running totals in a `Dictionary` as each
observation is processed (updated incrementally in `ProcessObservation`),
which would be marginally faster for very large inputs. I didn't do that
here because:
- It would mean the "reporting" concern and the "processing" concern are
  interleaved in the same method, instead of being separate, independently
  testable operations.
- The dataset in question is a list of radar observations, not a
  high-frequency stream — computing the report on demand from
  `_fineHistory` is simple, correct, and fast enough, and it keeps
  `ProcessObservation` focused on a single job: evaluate + record.


## 4. Principles this design leans on

- **Single Responsibility Principle (SRP)** — each rule class checks one
  condition; `Fine` totals violations; `QuantumRadar` orchestrates and
  reports; `Program` wires everything together.
- **Open/Closed Principle (OCP)** — new rules can be added without
  modifying `QuantumRadar` or any existing rule.
- **Dependency Inversion Principle (DIP)** — `QuantumRadar` depends on the
  `IRule` abstraction, and rules are supplied from the outside
  (`Program.cs`), not constructed internally.
- **Strategy Pattern** — each `IRule` implementation is an interchangeable
  algorithm for deciding "is this a violation?".
- **Separation of Concerns** — `Model` (data), `Rules` (business logic per
  rule), and `Core` (orchestration/reporting) are kept in separate
  namespaces/folders so each layer can change independently.

## 5. How to run

```bash
cd Fawry-Assessment/Fawry-Assessment
dotnet run
```

This builds the sample observations in `Program.cs`, evaluates them
against all four rules, and prints the per-fine breakdown followed by the
two summary reports.
