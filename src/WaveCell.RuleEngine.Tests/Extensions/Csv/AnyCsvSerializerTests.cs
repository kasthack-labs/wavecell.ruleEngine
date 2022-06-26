namespace WaveCell.RuleEngine.Tests.Extensions.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using WaveCell.RuleEngine.Core.Extensions.Csv;
    using WaveCell.RuleEngine.Core.Tests.Models;

    public class AnyCsvSerializerTests
    {
        private TRule[] GetRecords<TRule>(string sample)
        {
            using var reader = new StringReader(sample);
            return AnyCsvSerializer.GetRecords<TRule>(reader);
        }

        [Fact]
        public void BasicSerializationWorks()
        {
            var expected = new GenericTestRule[] {
                    new (1,null,null,null,null,null, 0),
                    new (2,"text",true,1,2,3.1,100),
                };

            var actual = GetRecords<GenericTestRule>("""
Id,StringFilter,BoolFilter,IntFilter,LongFilter,DoubleFilter,Priority
1,<ANY>,<ANY>,<ANY>,<ANY>,<ANY>,0
2,text,true,1,2,3.1,100
""");

            Assert.Equal(expected, actual);
        }
    }
}
