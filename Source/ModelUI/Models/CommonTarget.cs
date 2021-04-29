using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Models
{
    public class CommonTarget
    {

        public string GlobalEnumsOutputFile { get; set; }
        public bool Capitalize { get; set; }
        public bool TrimPrefix { get; set; }
        public List<string> TrimPrefixes { get; set; }
        public Dictionary<string, string> GlobalEnums { get; set; }
        public Dictionary<string, string> Mapping { get; set; }
        public CommonTarget()
        {

        }
    }
}
