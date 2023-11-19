using Newtonsoft.Json;
using SNPM.MVVM.Models.Interfaces;

namespace SNPM.MVVM.Models
{
    public class Parameter : IParameter
    {
        public Parameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
