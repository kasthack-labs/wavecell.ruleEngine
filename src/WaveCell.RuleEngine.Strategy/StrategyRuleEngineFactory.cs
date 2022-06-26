namespace WaveCell.RuleEngine.Strategy
{
    using WaveCell.RuleEngine.Core.Interfaces;
    using WaveCell.RuleEngine.Core.Models;

    /// <summary>
    /// Creates StrategyRuleEngines with default search parameters configured
    /// </summary>
    public static class StrategyRuleEngineFactory
    {
        public static IRuleEngineBuilder<StrategyRule, StrategyFilter> Create(EngineOptions? options = default)
            => RuleEngine.Core.RuleEngineFactory.Instance.Create<StrategyRule, StrategyFilter>(
                rule => new object?[] { rule.Filter1, rule.Filter2, rule.Filter3, rule.Filter4 },
                filter => new object?[] { filter.Filter1, filter.Filter2, filter.Filter3, filter.Filter4 },
                options
            );
    }
}