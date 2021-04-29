using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Focus.Utils.ModelGenerator.Utilities
{
    public static class MetadataProvider
    {

        public static List<OptionSetMetadataBase> GetGlobalEnumsMetadata(IOrganizationService service)
        {
            RetrieveAllOptionSetsRequest req = new RetrieveAllOptionSetsRequest()
            {
                RetrieveAsIfPublished = true,
            };
            RetrieveAllOptionSetsResponse res = (RetrieveAllOptionSetsResponse)service.Execute(req);
            return res.OptionSetMetadata.Select(k => { return (OptionSetMetadataBase)k; }).ToList();
        }

        public static EntityMetadata GetEntityMetadata(IOrganizationService service, string entityLogicalName)
        {
            RetrieveEntityRequest req = new RetrieveEntityRequest()
            {
                LogicalName = entityLogicalName,
                RetrieveAsIfPublished = true,
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.All
            };
            RetrieveEntityResponse res = (RetrieveEntityResponse)service.Execute(req);
            return res.EntityMetadata;
        }

        public static EntityMetadata[] GetEntitiesMetadata(IOrganizationService service)
        {
            RetrieveAllEntitiesRequest req = new RetrieveAllEntitiesRequest()
            {
                RetrieveAsIfPublished = true,
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity
            };
            RetrieveAllEntitiesResponse res = (RetrieveAllEntitiesResponse)service.Execute(req);
            return res.EntityMetadata;
        }

        public static List<string> GetRolesNames(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression("role");
            qe.ColumnSet = new ColumnSet("name");
            var roles = service.RetrieveMultiple(qe);
            return roles.Entities
                .Select(k => k.GetAttributeValue<string>("name"))
                .GroupBy(k => k)
                .Select(k => k.Key)
                .ToList();
        }
    }
}
