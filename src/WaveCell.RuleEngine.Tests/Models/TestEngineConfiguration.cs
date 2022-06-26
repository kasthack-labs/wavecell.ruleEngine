namespace WaveCell.RuleEngine.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using WaveCell.RuleEngine.Core.Interfaces;

    /// <summary>
    /// Shared options for tests.
    /// </summary>
    internal static class TestEngineConfiguration
    {
        // Default catch-all rule
        public static GenericTestRule CatchAllRule { get; } = new(0, default, default, default, default, default, default);

        // Dummy rule for intentionally missing
        public static GenericTestRule DummyRule { get;} = new(-1, "just for proving that the matcher doesn't take the single rule", default, default, default, default, 0);

        // Rule for exact matches
        public static GenericTestRule ExactMatchRule { get; } = new GenericTestRule(2, "text", true, 1, 2, 3.1, 100);

        public static GenericTestRule PriorityOverride { get; } = ExactMatchRule with { Id = 3, Priority = 110 };

        public static GenericTestFilter ExactMatchFilter { get; } = new ("text", true, 1, 2, 3.1);

        //default+dummy set
        public static IReadOnlyList<GenericTestRule> SetDefault { get; } = new GenericTestRule[] {
            CatchAllRule,
            DummyRule
        };

        public static IReadOnlyList<GenericTestRule> SetExact { get; } = new GenericTestRule[] {
            ExactMatchRule,
            DummyRule
        };

        public static Func<GenericTestRule, object?[]> RulePropertyExtractor { get; } =
            rule => new object?[] { rule.StringFilter, rule.BoolFilter, rule.IntFilter, rule.LongFilter, rule.DoubleFilter };
        public static Func<GenericTestFilter, object?[]> FilterPropertyExtractor { get; } =
            filter => (new object?[] { filter.StringFilter, filter.BoolFilter, filter.IntFilter, filter.LongFilter, filter.DoubleFilter });
        public static GenericTestRule[] DuplicateDefaultRules { get; } = new GenericTestRule[] {
            CatchAllRule,
            new GenericTestRule(2, default, default, default, default, default, default),
        };
        //public static GenericTestRule[] SingleDefaultRule { get; } = new GenericTestRule[] { new GenericTestRule(default, default, default, default, default, default, default) };
        public static IRuleEngineBuilder<GenericTestRule, GenericTestFilter> DefaultBuilder { get; } = RuleEngineFactory.Instance.Create(RulePropertyExtractor, FilterPropertyExtractor);
    }
}
