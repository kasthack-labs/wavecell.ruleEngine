namespace WaveCell.RuleEngine.Core.Benchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    using WaveCell.RuleEngine.Core.Interfaces;
    using WaveCell.RuleEngine.Strategy;

    public class RuleEngineBenchmark
    {
        private readonly StrategyFilter[] filters;
        private readonly IRuleEngine<StrategyRule, StrategyFilter> ruleEngine;

        public RuleEngineBenchmark()
        {
            //worst case: we have TONS of wildcards and 16 matches for any existing key
            var keys = Enumerable.Range(0, 6).Select(a => (char)('A' + a)).Select(a => new string(a, 3)).Append(null).ToArray();
            var id = 0;
            var rules = keys.SelectMany(f1 => keys.SelectMany(f2 => keys.SelectMany(f3 => keys.Select(f4 => new StrategyRule(++id, 0, f1, f2, f3, f4, (-id).ToString()))))).ToArray();
            this.filters = rules.Select(a=>new StrategyFilter(a.Filter1, a.Filter2, a.Filter3, a.Filter4)).ToArray();
            this.ruleEngine = StrategyRuleEngineFactory.Create().WithRules(rules);
        }

        [Benchmark]
        public void FindRule()
        {
            for (int i = 0; i < 10_000; i++)
            {
                ruleEngine.FindRule(filters[Random.Shared.Next(filters.Length)]);
            }
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}