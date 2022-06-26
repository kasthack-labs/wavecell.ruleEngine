namespace WaveCell.RuleEngine.Core.Models
{
    /// <summary>
    /// Basic prioritized rule DTO interface.
    /// </summary>
    public interface IPrioritized
    {
        /// <summary>
        /// Currently, we support unified numeric priorities by converting everything to `double` instead of implementing IComparable&lt;&gt; for them.
        /// and providing fancy generic interfaces for DTOs.
        /// This allows to keep DTO interfaces simple, but it might give you unexpected results, if you're trying to be smart, and use large int64 values close to one another.
        /// </summary>
        double Priority { get; }
    }
}