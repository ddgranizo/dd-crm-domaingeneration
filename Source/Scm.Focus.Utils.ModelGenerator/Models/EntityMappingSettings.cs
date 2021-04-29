using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Focus.Utils.ModelGenerator.Models
{
    public class EntityMappingSettings
    {
        public string PluralNamespaceName { get; set; }
        public string EntityDomainName { get; set; }
        public bool? TrimPrefix { get; set; }
        public bool? Capitalize { get; set; }
        public List<string> TrimPrefixes { get; set; }
        public string OutputFile { get; set; }
        public string GlobalEnumsOutputFile { get; set; }
        public string GlobalEntitiesDefinitionOutputFile { get; set; }
        public string GlobalRolesDefinitionOutputFile { get; set; }
        public Dictionary<string, string> Mapping { get; set; }
        public Dictionary<string, string> GlobalEnums { get; set; }
        public EntityMappingSettings()
        {
            Mapping = new Dictionary<string, string>();
            GlobalEnums = new Dictionary<string, string>();
            TrimPrefixes = new List<string>();
        }
    }
}