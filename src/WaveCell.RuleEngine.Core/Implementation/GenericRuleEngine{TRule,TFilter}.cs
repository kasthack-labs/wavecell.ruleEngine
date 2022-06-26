namespace WaveCell.RuleEngine.Core.Implementation
{
    using WaveCell.RuleEngine.Core.Interfaces;
    using WaveCell.RuleEngine.Core.Models;

    /// <summary>
    /// Generic rule engine
    /// </summary>
    /// <typeparam name="TRule">Rule type</typeparam>
    /// <typeparam name="TFilter">Filter type</typeparam>
    internal class GenericRuleEngine<TRule, TFilter> : IRuleEngine<TRule, TFilter>
        where TRule : class, IPrioritized
    {
        private readonly Func<TFilter, object?[]> filterPropertyExtractor;
        private readonly RuleNode rootNode;
        private int? keyCount;

        /// <summary>
        /// Create a rule engine.
        /// </summary>
        /// <param name="rules">Rule collection. Nulls are not allowed</param>
        /// <param name="rulePropertyExtractor">Delegate that extr</param>
        /// <param name="filterPropertyExtractor"></param>
        /// <param name="options"></param>
        public GenericRuleEngine(
            IEnumerable<TRule> rules,
            Func<TRule, object?[]> rulePropertyExtractor,
            Func<TFilter, object?[]> filterPropertyExtractor,
            EngineOptions? options = default)
        {
            ArgumentNullException.ThrowIfNull(rules);
            ArgumentNullException.ThrowIfNull(rulePropertyExtractor);
            ArgumentNullException.ThrowIfNull(filterPropertyExtractor);
            options ??= new();

            this.filterPropertyExtractor = filterPropertyExtractor;
            this.rootNode = new RuleNode();
            this.BuildLookup(rules, rulePropertyExtractor, options);
        }

        // this part should probably moved to the builder for reusability?
        private void BuildLookup(IEnumerable<TRule> rules, Func<TRule, object?[]> rulePropertyExtractor, EngineOptions options)
        {
            var ruleCount = 0;
            int? propertyCount = default;
            var catchAllCheckPassed = !options.RequireCatchAllRule;

            foreach (var rule in rules)
            {
                if (rule is null)
                {
                    throw new ArgumentNullException($"{nameof(rule)}[{ruleCount}]", "Rule loader has encountered a null property");
                }

                var properties = rulePropertyExtractor(rule);

                if (properties is null)
                {
                    throw new InvalidOperationException("Rule property extractor returned null instead of an array of properties. " +
                        $"Error occured while processing rule('{rule}', index {ruleCount})");
                }

                if (properties.Length == 0)
                {
                    throw new InvalidOperationException("Rule property extractor returned no properties. " +
                        $"Error occured while processing rule('{rule}', index {ruleCount})");
                }

                if (propertyCount is not null && propertyCount != properties.Length)
                {
                    throw new InvalidOperationException(
                        $"Rule property extractor returned different number of properties for different rules({propertyCount} and {properties.Length}). " +
                        $"Error occured while processing rule('{rule}', index {ruleCount})");
                }
                else
                {
                    propertyCount = properties.Length;
                }

                catchAllCheckPassed = catchAllCheckPassed || properties.All(a => a is null);

                var replacedOlder = rootNode.InsertRule(rule, properties);
                if (replacedOlder && !options.AllowDuplicateRules)
                {
                    throw new ArgumentException($"Duplicated rules found. First duplicate: rule('{rule}', index {ruleCount})");
                }

                ruleCount++;
            }

            if (options.RequireAtLeastOneRule && ruleCount == 0)
            {
                throw new ArgumentException(
                    $"No rules were provided for initialization. Check `{nameof(rules)}` or set {nameof(options)}.{nameof(options.RequireAtLeastOneRule)} to `false`",
                    nameof(rules));
            }

            if (!catchAllCheckPassed)
            {
                throw new ArgumentException($"Not catch-all rule found. Check `{nameof(rules)}` or set {nameof(options)}.{nameof(options.RequireCatchAllRule)} to `false`.");
            }

            this.keyCount = propertyCount;
        }
        /// <inheritdoc/>
        public TRule? FindRule(TFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var keys = this.filterPropertyExtractor(filter);
            if (keys is null)
            {
                throw new InvalidOperationException($"Filter property extractor returned null. Filter: {filter}");
            }
            if (keys.Length != this.keyCount)
            {
                throw new InvalidOperationException($"Property count mismatch: filter property extractor returned {keys.Length} properties while the rule engine has {keyCount} properties. Filter: {filter}");
            }
            return (TRule?)this.GetMatchingNodes(new[] { this.rootNode }, keys).FirstOrDefault()?.Rule;
        }

        private IEnumerable<RuleNode> GetMatchingNodes(IEnumerable<RuleNode> nodes, Span<object?> keys)
        {
            if (keys.Length > 0)
            {
                var key = keys[0];
                return GetMatchingNodes(
                    nodes.SelectMany(node => node.GetMatchingNodes(key)), keys[1..]);
            }
            else
            {
                return AsEnumerable(
                    nodes.MaxBy(a => a.Rule!.Priority)!
                );
            }
        }

        private static IEnumerable<T> AsEnumerable<T>(T item) where T : notnull
        {
            yield return item;
        }
    }
}