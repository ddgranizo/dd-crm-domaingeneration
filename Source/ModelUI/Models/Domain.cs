using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Models
{
    public class Domain
    {

        public string DomainName { get; set; }
        public string DomainPath { get; set; }
        public bool HasInfrastructure { get { return !string.IsNullOrEmpty(InfrastructurePath); } }
        public string InfrastructurePath { get; set; }
        public Domain(string domainName, string domainPath)
        {
            this.DomainName = domainName;
            this.DomainPath = domainPath;

        }

        public Domain(string domainName, string domainPath, string infrastructurePath)
        {
            this.DomainName = domainName;
            this.DomainPath = domainPath;
            this.InfrastructurePath = infrastructurePath;
        }


        public override string ToString()
        {
            return this.DomainName;
        }
    }
}
