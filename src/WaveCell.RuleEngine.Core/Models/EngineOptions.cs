namespace WaveCell.RuleEngine.Core.Models
{
    /// <summary>
    /// Rule engine options.
    /// </summary>
    /// <param name="AllowDuplicateRules">If AllowDuplicateRules is set to `false`, rule engine will throw if there're rules with identical filtering properties, otherwise the rule with the highest priority will be kept</param>
    /// <param name="RequireCatchAllRule">If RequireCatchAllRule is set to `true`, rule engine will throw unless there's a rule with wildcard values for all keys.</param>
    /// <param name="RequireAtLeastOneRule">If RequireAtLeastOneRule is set to `true`, rule engine will throw if no rules were provided during initialization.</param>
    public record EngineOptions(
        bool AllowDuplicateRules = true,
        bool RequireCatchAllRule = false,
        bool RequireAtLeastOneRule = true
    );
}