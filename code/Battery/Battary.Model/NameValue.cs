using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model
{
    [Serializable]
    public class NameValue
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public NameValue()
        {

        }
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
