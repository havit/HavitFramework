using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;

    public class DbStoredProcedure
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string EntityName { get; set; }

    public string GeneratedFile { get; set; }

        public StoredProcedure StoredProcedure { get; set; }
    }