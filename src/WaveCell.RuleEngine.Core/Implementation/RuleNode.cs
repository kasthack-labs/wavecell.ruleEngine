namespace WaveCell.RuleEngine.Core.Implementation
{
    using WaveCell.RuleEngine.Core.Models;

    /// <summary>
    /// Lookup node for <see cref="GenericRuleEngine{TRule, TFilter}"/>
    /// </summary>
    internal class RuleNode
    {
        private readonly Dictionary<object, RuleNode> valueNodes = new();
        private RuleNode? catchAllNode;

        public IPrioritized? Rule { get; private set; }

        public IEnumerable<RuleNode> GetMatchingNodes(object? key)
        {
            if (key is not null && this.valueNodes.TryGetValue(key, out var node))
            {
                yield return node;
            }

            if (this.catchAllNode is not null)
            {
                yield return this.catchAllNode;
            }
        }

        /// <summary>
        /// Inserts rule. If there's a conflict, rule with the highest priority is kept
        /// </summary>
        /// <typeparam name="TRule">Rule type</typeparam>
        /// <param name="rule">rule</param>
        /// <param name="properties">Extracted properties</param>
        public bool InsertRule<TRule>(TRule rule, Span<object?> properties)
            where TRule : class, IPrioritized
        {
            if (properties.Length > 0)
            {
                var key = properties[0];
                var node = GetOrCreateNode(key);
                return node.InsertRule(rule, properties[1..]);
            }
            else
            {
                var currentRule = this.Rule;
                bool keyExists = currentRule is not null;
                if (!keyExists || currentRule!.Priority <= rule.Priority)
                {
                    this.Rule = rule;
                }

                return keyExists;
            }
        }

        private RuleNode GetOrCreateNode(object? key) => key switch
        {
            null => this.catchAllNode ??= new(),
            _ => this.valueNodes.GetValueOrDefault(key) ?? (this.valueNodes[key] = new()),
        };
    }
}