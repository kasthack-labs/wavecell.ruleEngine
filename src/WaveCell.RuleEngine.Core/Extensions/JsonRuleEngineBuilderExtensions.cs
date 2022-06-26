namespace WaveCell.RuleEngine.Core.Extensions
{
    using System.Reflection;
    using System.Text.Json;

    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    using WaveCell.RuleEngine.Core.Extensions.Csv;
    using WaveCell.RuleEngine.Core.Interfaces;

    /// <summary>
    /// Allows loading rules from JSON sources.
    /// </summary>
    public static class JsonngineBuilderExtensions
    {
        private static readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = false,
        };

        /// <summary>
        /// Loads rules from a CSV file.
        /// </summary>
        /// <typeparam name="TRule">Rule type</typeparam>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <param name="builder">Builder to use for configuring the resulting rule engine</param>
        /// <param name="csvPath">Path to the source file with rule data</param>
        /// <returns>Configured rule engine</returns>
        public static IRuleEngine<TRule, TFilter> WithJsonFileRules<TRule, TFilter>(
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
        public static IRuleEngine<TRule, TFilter> WithJsonStreamRules<TRule, TFilter>(
            this IRuleEngineBuilder<TRule, TFilter> builder,
            TextReader reader)
        {
            //csv API uses textreader while STJ only supports byte streams and strings
            //I'm doing this to keep public APIs consistent
            var text = reader.ReadToEnd();
            var records = JsonSerializer.Deserialize<TRule[]>(text, jsonOptions)!;
            return builder.WithRules(records);
        }
    }
}