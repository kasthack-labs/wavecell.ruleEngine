/*
 * Rule engine usage sample.
 * 
 */

using WaveCell.RuleEngine.Core;
using WaveCell.RuleEngine.Core.Implementation;
using WaveCell.RuleEngine.Core.Extensions;
using WaveCell.RuleEngine.Core.Models;
using WaveCell.RuleEngine.Strategy;

// Simple example from the assigment

var strategyEngine = StrategyRuleEngineFactory.Create().WithCsvFileRules("SampleData.csv");
var filter = new StrategyFilter("AAA", "BBB", "CCC", "DDD");
Console.WriteLine($"Searching using filter: {filter}");
Console.WriteLine($"Found {strategyEngine.FindRule(filter)}");
return;

//###############################################

//Extensible generic engine usage / configuration

var engineBuilder = RuleEngineFactory.Instance.Create<StrategyRule, StrategyFilter>(
    // Delegate that extracts lookup keys from the provided rules.
    // null keys are interpreted as a wildcard that acceptes any value.
    // Custom comparers are not supported, so you might want to normalize the keys here.
    // The delegate _must_ always return arrays of the same length.
    rule => new object?[] { rule.Filter1, rule.Filter2, rule.Filter3, rule.Filter4 },
    // Delegate that extracts lookup keys from the provided rules.
    // The delegate _must_ return the same number of keys as the previous one.
    // Custom comparers are not supported, so you might want to normalize the keys here.
    filter => new object?[] { filter.Filter1, filter.Filter2, filter.Filter3, filter.Filter4 },

    // You can configure basic rule engine behavior
    new EngineOptions(
        // If AllowDuplicateRules is set to `false`, rule engine will throw if there're rules with identical filtering properties,
        // otherwise the rule with the highest priority will be kept
        AllowDuplicateRules: true,
        // If RequireCatchAllRule is set to `true`, rule engine will throw unless there's a rule with wildcard values for all keys.
        RequireCatchAllRule: false,
        // If RequireAtLeastOneRule is set to `true`, rule engine will throw if no rules were provided during initialization.
        RequireAtLeastOneRule: true));

// construct an engine using a ruleset and preconfigured options
var engine = engineBuilder
    // Rule engine allows using collections as a source of rules.
    // It also provides adapters for reading rules from CSV and JSON files / streams out of the box.
    // CSV adapter treats magic "<ANY>" string as null for compatibility with the existing tools.
    .WithCsvStreamRules(new StringReader("""
RuleId,Priority,Filter1,Filter2,Filter3,Filter4,OutputValue
1,80,AAA,<ANY>,CCC,DDD,8
2,10,<ANY>,<ANY>,AAA,<ANY>,1
3,70,BBB,<ANY>,CCC,<ANY>,7
4,100,AAA,BBB,CCC,<ANY>,10
5,50,CCC,AAA,<ANY>,CCC,5
6,0,<ANY>,<ANY>,<ANY>,<ANY>,0
"""));

// csv file:
engineBuilder.WithCsvFileRules(@"something.csv");

// json file:
engineBuilder.WithJsonFileRules(@"something.json");

// collection:

engineBuilder.WithRules(new StrategyRule[] {
    new StrategyRule(1,100, "AAA","BBB", "CCC", "DDD", "<value>")
});

Console.WriteLine(engine.FindRule(new("AAA", "BBB", "CCC", "AAA")));