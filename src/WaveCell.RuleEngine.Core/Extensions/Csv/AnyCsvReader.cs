namespace WaveCell.RuleEngine.Core.Extensions.Csv
{
    using System.Globalization;

    using CsvHelper;

    /// <summary>
    /// Facade for CsvReader that replaces &lt;ANY&gt; with nulls
    /// </summary>
    internal class AnyCsvReader : CsvReader
    {
        private const string nullField = "<ANY>";

        /// <inheritdoc/>
        public AnyCsvReader(TextReader reader, CultureInfo culture, bool leaveOpen = false) : base(reader, culture, leaveOpen)
        {
            this.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(""); //https://stackoverflow.com/a/65805248/17594255
        }

        /// <inheritdoc/>
        public override string GetField(int index)
        {
            var result = base.GetField(index);
            if (result == nullField)
            {
                return String.Empty;
            }
            return result;
        }
    }
}