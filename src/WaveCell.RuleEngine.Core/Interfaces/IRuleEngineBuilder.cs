namespace WaveCell.RuleEngine.Core.Interfaces
{
    public interface IRuleEngineBuilder<TRule, TFiler>
    {
        /// <summary>
        /// Creates an engine instance initialized with rules
        /// </summary>
        /// <param name="rules">Collection of rules to use</param>
        /// <returns>Engine</returns>
        IRuleEngine<TRule, TFiler> WithRules(IEnumerable<TRule> rules);
    }
}