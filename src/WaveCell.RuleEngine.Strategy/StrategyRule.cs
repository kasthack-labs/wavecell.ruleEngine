namespace WaveCell.RuleEngine.Strategy
{
    using WaveCell.RuleEngine.Core.Models;

    public record StrategyRule(int RuleId, int Priority, string? Filter1, string? Filter2, string? Filter3, string? Filter4, string OutputValue) : IPrioritized
    {
        // Currently, we support unified numeric priorities by converting everything to `double` instead of implementing IComparable<> for them
        // and providing fancy generic interfaces for DTOs.
        // This allows to keep DTO interfaces simple, but it might give you unexpected results, if you're trying to be smart, and use large int64 values close to one another.
        double IPrioritized.Priority => this.Priority;
    };
}