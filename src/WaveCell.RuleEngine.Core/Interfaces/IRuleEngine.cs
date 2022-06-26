namespace WaveCell.RuleEngine.Core.Interfaces
{
    /// <summary>
    /// Basic rule engine interface
    /// </summary>
    /// <typeparam name="TRule">Rule type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    public interface IRuleEngine<TRule, TFilter>
    {
        /// <summary>
        /// Matches a rule using filter. If the rule is not found, `null` is returned.
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>Matched rule or null</returns>
        TRule? FindRule(TFilter filter);
    }
}