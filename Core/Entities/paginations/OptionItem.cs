using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.paginations
{
    public sealed class OptionItem
    {
        public long Value { get; set; }
        public string Label { get; set; } = "";
        public string? ExtraInfo { get; set; }
    }

    public sealed class OptionItemToken
    {
        public string Value { get; set; } = ""; 
        public string Label { get; set; } = "";
    }


}
