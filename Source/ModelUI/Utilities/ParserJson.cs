using ModelUI.Models;
using Newtonsoft.Json;
using Scm.Focus.Utils.ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Utilities
{
    public static class ParserJson
    {





        public static string Stringfy<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }


        public static EntityMappingSettings ParseEntityMappingSetting(string json)
        {
            return GenericParse<EntityMappingSettings>(json);
        }

        public static CommonTarget ParseCommonTarget(string json)
        {
            return GenericParse<CommonTarget>(json);
        }
        public static EntityTarget ParseEntity(string json)
        {
            return GenericParse<EntityTarget>(json);
        }

        private static T GenericParse<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
