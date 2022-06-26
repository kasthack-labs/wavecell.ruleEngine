# wavecell.ruleEngine

[![license](https://img.shields.io/github/license/kasthack/wavecell.ruleEngine.svg)](LICENSE)
[![.NET Status](https://github.com/kasthack/wavecell.ruleEngine/workflows/.NET/badge.svg)](https://github.com/kasthack/wavecell.ruleEngine/actions?query=workflow%3A.NET)

## What

Test assignment for WaveCell / 8x8.

Rule engine that matches rules by filters, supports wildcards, all kinds of types, different data sources, etc.

Check out `doc` folder for the description.

## Contents of the repository

src folder contains a .NET solution with 6 projects:
* `src`
    * `WaveCell.RuleEngine.Core` -- generic extensible engine
    * `WaveCell.RuleEngine.Strategy` -- implementation for StrategyRule as described in the task
* `test`
    * `WaveCell.RuleEngine.Core.Tests` -- tests for the base engine
    * `WaveCell.RuleEngine.Strategy.Text` -- test for the task-specific engine
    * `WaveCell.RuleEngine.Core.Benchmark` -- performance benchmark
* `example`
    * `WaveCell.RuleEngine.Example` -- API usage examples

## Implementation notes

### Implemented features

* Generic APIs
* Multiple filters per rule
* Rule sources: CSV, JSON

### Performance:

* Loading rules:
    * Time complexity: `O(N*K)` where `N` is the number of rules and `K` is the number of rules
    * Memory complexity is `O(N)`
* Matching
    * Time complexity
        * Best case: `O(K)`
         * Worst case: `O(min(2^K, N))` for a crafted set of rules with exact matches and wildcards for every key, that would produce `2^K` matches.
     * Memory complexity
         * `O(K)`. There're options to get rid of allocations but that would hurt maintainability.
* Benchmark results:
    * Rule engine can process at least 50k qps on a single while having 2400 rules with 16+ matches for every filter

### Future improvements

* Implement null values for rules. Currently, we interpret nulls as wildcards, but there might be a legitimate case when we would be looking for `null` as a literal.
* Implement internal generic keys. Currently, rule engine internally relies on `.GetHashCode()` and `.Equals()` used by `Dictionary<X,Y>` while casting keys to `object`. This leads to
    * Missing support for custom comparers
    * Additional boxing
* Generally reduce the number of allocations.
     * Current implementation creates a bunch of iterators by using LINQ for the sake of readability. This could be a bottleneck for high throughput rule matching.
     * Generic implementation is expected to allocate an array of keys extracted from the filter DTO. This can be avoided as well.
* Some refactoring won't hurt.