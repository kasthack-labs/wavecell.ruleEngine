namespace WaveCell.RuleEngine.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using WaveCell.RuleEngine.Core.Models;
    using WaveCell.RuleEngine.Core.Tests.Models;

    using static WaveCell.RuleEngine.Core.Tests.Models.TestEngineConfiguration;

    public class RuleEngineTests
    {
        //This probably should be a whole class with tests per every condition, but I don't want to artificially inflate the number of tests

        public static IEnumerable<object[]> GetPropertyExtractors => new Func<GenericTestFilter, object[]>[] {
            //null
            _ => null!,
            //no properties
            _ => Array.Empty<object>(),
            // property count mismatch
            _ => new object[2]
        }.Select(a => new object[] { a }).ToArray();

        [Theory]
        [MemberData(nameof(GetPropertyExtractors))]
        public void FilterPropertyExtractorValidationWorks(Func<GenericTestFilter, object[]> filterPropertyExtractor)
        {
            Assert.Throws<InvalidOperationException>(() => RuleEngineFactory.Instance.Create(
                RulePropertyExtractor,
                filterPropertyExtractor)
                    .WithRules(SetDefault)
                    .FindRule(new(default, default, default, default, default)));
        }

        public static IEnumerable<object?[]> GetMatchTestCases =>
            new (IEnumerable<GenericTestRule> rules, GenericTestFilter filter, GenericTestRule? rule)[] {
                // wildcard match
                (SetDefault, ExactMatchFilter, CatchAllRule),
                //exact match
                (SetExact, ExactMatchFilter, ExactMatchRule),
                //priority
                (new[]{ CatchAllRule, ExactMatchRule, DummyRule}, ExactMatchFilter, ExactMatchRule),
                //override: higher prioprity, duplicate rules
                (new[]{ ExactMatchRule, PriorityOverride, DummyRule}, ExactMatchFilter, PriorityOverride),
                //override: higher prioprity, duplicate rules, different load order
                (new[]{ PriorityOverride, ExactMatchRule, DummyRule}, ExactMatchFilter, PriorityOverride),
                //no match
                (new[]{ DummyRule}, ExactMatchFilter, default!),
            }.Select(a => new object?[] { a.rules, a.filter, a.rule });

        //This probably should be a whole class with tests per every condition, but I don't want to artificially inflate the number of tests
        [Theory]
        [MemberData(nameof(GetMatchTestCases))]
        public void MatchingWorks(IEnumerable<GenericTestRule> rules, GenericTestFilter filter, GenericTestRule? expectedMatchedRule)
        {
            Assert.Equal(
                expectedMatchedRule,
                DefaultBuilder
                    .WithRules(rules)
                    .FindRule(filter)
            );
        }
    }
}
