namespace WaveCell.RuleEngine.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using WaveCell.RuleEngine.Core.Extensions;
    using WaveCell.RuleEngine.Core.Interfaces;
    using WaveCell.RuleEngine.Core.Models;
    using WaveCell.RuleEngine.Core.Tests.Models;

    public class RuleEngineFactoryTests
    {
        //This probably should be a whole class with tests per every condition, but I don't want to artificially inflate the number of tests
        [Fact]
        public void RuleValidationWorks()
        {
            var defaultBuilder = TestEngineConfiguration.DefaultBuilder;

            // produces something
            Assert.NotNull(defaultBuilder.WithRules(TestEngineConfiguration.SetDefault));

            // throws on nulls for args
            Assert.Throws<ArgumentNullException>(() => defaultBuilder.WithRules(default!));

            Assert.Throws<ArgumentNullException>(() =>
                RuleEngineFactory.Instance.Create<GenericTestRule, GenericTestFilter>(default!, TestEngineConfiguration.FilterPropertyExtractor)
                    .WithRules(TestEngineConfiguration.SetDefault));
            Assert.Throws<ArgumentNullException>(() =>
                RuleEngineFactory.Instance.Create<GenericTestRule, GenericTestFilter>(TestEngineConfiguration.RulePropertyExtractor, default!)
                    .WithRules(TestEngineConfiguration.SetDefault));

            // throws on null row
            Assert.Throws<ArgumentNullException>(() => defaultBuilder.WithRules(new GenericTestRule[] { null! }));

            //throws on invalid rule property extractors

            //extractor returns nulls
            Assert.Throws<InvalidOperationException>(() =>
                RuleEngineFactory.Instance.Create<GenericTestRule, GenericTestFilter>(
                    _ => default!,
                    TestEngineConfiguration.FilterPropertyExtractor).WithRules(TestEngineConfiguration.SetDefault));
            //extractor returns no properties
            Assert.Throws<InvalidOperationException>(() =>
                RuleEngineFactory.Instance.Create<GenericTestRule, GenericTestFilter>(_ => Array.Empty<object>(), TestEngineConfiguration.FilterPropertyExtractor)
                    .WithRules(TestEngineConfiguration.SetDefault));
            //number of extracted properties is inconsistent
            Assert.Throws<InvalidOperationException>(() =>
                RuleEngineFactory.Instance.Create<GenericTestRule, GenericTestFilter>(rule => new object[rule.Id], TestEngineConfiguration.FilterPropertyExtractor)
                    .WithRules(TestEngineConfiguration.DuplicateDefaultRules));

            // throws on empty ruleset by default
            Assert.Throws<ArgumentException>(() => defaultBuilder.WithRules(Array.Empty<GenericTestRule>()));

            // doesn't throw on empty ruleset if allowed
            RuleEngineFactory.Instance.Create(TestEngineConfiguration.RulePropertyExtractor, TestEngineConfiguration.FilterPropertyExtractor, new(RequireAtLeastOneRule: false))
                .WithRules(Array.Empty<GenericTestRule>());

            // allows duplicates by default
            defaultBuilder.WithRules(TestEngineConfiguration.DuplicateDefaultRules);

            //throws on duplicates when disallowed
            Assert.Throws<ArgumentException>(() =>
                RuleEngineFactory.Instance.Create(TestEngineConfiguration.RulePropertyExtractor, TestEngineConfiguration.FilterPropertyExtractor, new(AllowDuplicateRules: false))
                    .WithRules(TestEngineConfiguration.DuplicateDefaultRules));

            // throws if there's no catch-all rule when required
            Assert.Throws<ArgumentException>(() =>
                RuleEngineFactory.Instance.Create(TestEngineConfiguration.RulePropertyExtractor, TestEngineConfiguration.FilterPropertyExtractor, new(RequireCatchAllRule: true))
                    .WithRules(new GenericTestRule[] {
                        new(default, "test",default,default,default,default,default)
                    }));
        }

        [Fact]
        public void CsvAdapterWorks()
        {
            //csv adapter
            Assert.Equal(
                TestEngineConfiguration.ExactMatchRule,
                TestEngineConfiguration
                    .DefaultBuilder
                        .WithCsvStreamRules(new StringReader("""
Id,StringFilter,BoolFilter,IntFilter,LongFilter,DoubleFilter,Priority
1,<ANY>,<ANY>,<ANY>,<ANY>,<ANY>,0
2,text,true,1,2,3.1,100
"""))
                .FindRule(TestEngineConfiguration.ExactMatchFilter)!);
        }

        [Fact]
        public void JsonAdapterWorks()
        {
            Assert.Equal(
                TestEngineConfiguration.ExactMatchRule,
                TestEngineConfiguration
                    .DefaultBuilder
                    .WithJsonStreamRules(new StringReader("""
[

    { "Id" : 1, "StringFilter" : null, "BoolFilter" : null, "IntFilter" : null, "LongFilter" : null, "DoubleFilter" : null, "Priority" : 0 },
    { "Id" : 2, "StringFilter" : "text", "BoolFilter" : true, "IntFilter" : 1, "LongFilter" : 2, "DoubleFilter" : 3.1, "Priority" : 100 },
]
"""))
                .FindRule(TestEngineConfiguration.ExactMatchFilter)!);
        }
    }
}
