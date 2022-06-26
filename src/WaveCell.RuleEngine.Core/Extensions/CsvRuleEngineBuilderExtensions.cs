namespace WaveCell.RuleEngine.Core.Extensions
{
    using System.Reflection;

    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    using WaveCell.RuleEngine.Core.Extensions.Csv;
    using WaveCell.RuleEngine.Core.Interfaces;

    /// <summary>
    /// Allows loading rules from CSV sources while replacing fields containing &lt;ANY&gt; with wildcards.
    /// </summary>
    public static class CsvRuleEngineBuilderExtensions
    {
        /// <summary>
        /// Loads rules from a CSV file.
        /// </summary>
        /// <typeparam name="TRule">Rule type</typeparam>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <param name="builder">Builder to use for configuring the resulting rule engine</param>
        /// <param name="csvPath">Path to the source file with rule data</param>
        /// <returns>Configured rule engine</returns>
        public static IRuleEngine<TRule, TFilter> WithCsvFileRules<TRule, TFilter>(
            this IRuleEngineBuilder<TRule, TFilter> builder,
            string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            return builder.WithCsvStreamRules(reader);
        }

        /// <summary>
        /// Loads rules from a CSV stream.
        /// </summary>
        /// <typeparam name="TRule">Rule type</typeparam>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <param name="builder">Builder to use for configuring the resulting rule engine</param>
        /// <param name="reader">Textreader with rule data</param>
        /// <returns>Configured rule engine</returns>
        public static IRuleEngine<TRule, TFilter> WithCsvStreamRules<TRule, TFilter>(
            this IRuleEngineBuilder<TRule, TFilter> builder,
            TextReader reader)
        {
            var records = AnyCsvSerializer.GetRecords<TRule>(reader);
            return builder.WithRules(records);
        }
    }
}