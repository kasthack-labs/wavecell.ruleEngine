namespace WaveCell.RuleEngine.Core.Tests.Models
{
    using WaveCell.RuleEngine.Core.Models;

    public record GenericTestRule(int Id, string? StringFilter, bool? BoolFilter, int? IntFilter, long? LongFilter, double? DoubleFilter, int Priority) : IPrioritized
    {
        double IPrioritized.Priority => this.Priority;
    }
}
