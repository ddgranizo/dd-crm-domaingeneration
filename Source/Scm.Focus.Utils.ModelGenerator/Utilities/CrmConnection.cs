using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Focus.Utils.ModelGenerator.Utilities
{
    public static class CrmConnection
    {
        public static IOrganizationService GetService(string stringConnection)
        {
            CrmServiceClient crmService = new CrmServiceClient(stringConnection);
            IOrganizationService serviceProxy = crmService.OrganizationWebProxyClient 
                ?? (IOrganizationService)crmService.OrganizationServiceProxy;
            return serviceProxy;
        }
    }
}
