namespace WaveCell.RuleEngine.Core.Extensions.Csv
{
    using System.Globalization;

    /// <summary>
    /// Csv deserializer that supports &lt;AY&gt; records
    /// </summary>
    internal static class AnyCsvSerializer
    {
        /// <summary>
        /// Loads records from CSV while interpreting &lt;ANY&gt; as null
        /// </summary>
        /// <typeparam name="TRule"></typeparam>
        /// <param name="reader"></param>
        public static TRule[] GetRecords<TRule>(TextReader reader)
        {
            TRule[]? records;
            using (var csv = new AnyCsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<TRule>().ToArray();
            }
            return records;
        }
    }
}