namespace WaveCell.RuleEngine.Core
{
    using WaveCell.RuleEngine.Core.Implementation;
    using WaveCell.RuleEngine.Core.Interfaces;
    using WaveCell.RuleEngine.Core.Models;

    /// <summary>
    /// Creates rule engines.
    /// </summary>
    public sealed class RuleEngineFactory
    {
        private RuleEngineFactory() { }

        /// <summary>
        /// Factory instance.
        /// </summary>
        public static RuleEngineFactory Instance { get; } = new();

        /// <summary>
        /// Create a rule engine builder.
        /// </summary>
        /// <typeparam name="TRule">Rule </typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="rulePropertyExtractor"></param>
        /// <param name="filterPropertyExtractor"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IRuleEngineBuilder<TRule, TFilter> Create<TRule, TFilter>(
                    Func<TRule, object?[]> rulePropertyExtractor,
                    Func<TFilter, object?[]> filterPropertyExtractor,
                    EngineOptions? options = default)
                where TRule : class, IPrioritized
            => new RuleEngineBuilder<TRule, TFilter>(rulePropertyExtractor, filterPropertyExtractor, options);

        // there will be more options
        private record RuleEngineBuilder<TRule, TFilter>(
            Func<TRule, object?[]> RulePropertyExtractor,
            Func<TFilter, object?[]> FilterPropertyExtractor,
            EngineOptions? Options) : IRuleEngineBuilder<TRule, TFilter>
                where TRule : class, IPrioritized
        {
            public IRuleEngine<TRule, TFilter> WithRules(IEnumerable<TRule> rules)
                => new GenericRuleEngine<TRule, TFilter>(rules, this.RulePropertyExtractor, this.FilterPropertyExtractor, this.Options);
        }
    }
}