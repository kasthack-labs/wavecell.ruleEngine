namespace WaveCell.RuleEngine.Strategy.Tests
{
    using WaveCell.RuleEngine.Core.Extensions;
    using WaveCell.RuleEngine.Core.Interfaces;

    public class StrategyRuleEngineTests
    {
        private readonly IRuleEngine<StrategyRule, StrategyFilter> ruleEngine;

        public StrategyRuleEngineTests()
        {
            const string csv = """
RuleId,Priority,Filter1,Filter2,Filter3,Filter4,OutputValue
1,80,AAA,<ANY>,CCC,DDD,8
2,10,<ANY>,<ANY>,AAA,<ANY>,1
3,70,BBB,<ANY>,CCC,<ANY>,7
4,100,AAA,BBB,CCC,<ANY>,10
5,50,CCC,AAA,<ANY>,CCC,5
6,0,<ANY>,<ANY>,<ANY>,<ANY>,0
""";
            this.ruleEngine = StrategyRuleEngineFactory.Create().WithCsvStreamRules(new StringReader(csv));
        }

        [Theory]
        [MemberData(nameof(GetMatchTestCases))]
        public void MatchingWorksAsDefinedInPdf(StrategyFilter filter, int? expectedMatchedRuleId)
        {
            Assert.Equal(
                expectedMatchedRuleId,
                ruleEngine
                    .FindRule(filter)?.RuleId
            );
        }
        public static IEnumerable<object?[]> GetMatchTestCases =>
            new (StrategyFilter filter, int? ruleId)[] {
                (new("AAA","BBB","CCC","AAA"), 4),
                (new("AAA","BBB","CCC","DDD"), 4),
                (new("AAA","AAA","AAA","AAA"), 2),
                (new("BBB","BBB","BBB","BBB"), 6),
                (new("BBB","CCC","CCC","CCC"), 3),
            }.Select(a => new object?[] { a.filter, a.ruleId });
    }
}