using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

    public class ExtendedPropertiesKey
    {
        public string ClassDesc { get; set; }
        public long MajorId { get; set; }
        public long MinorId { get; set; }

        public ExtendedPropertiesKey(string classDesc, long majorId, long minorId)
        {
            ClassDesc = classDesc;
            MajorId = majorId;
            MinorId = minorId;
        }

        public override int GetHashCode()
        {
            return ClassDesc.GetHashCode() ^ MajorId.GetHashCode() ^ MinorId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ExtendedPropertiesKey objExtendedPropertiesKey = (obj as ExtendedPropertiesKey);
            return this.ClassDesc.Equals(objExtendedPropertiesKey.ClassDesc) && (this.MajorId == objExtendedPropertiesKey.MajorId) && (this.MinorId == objExtendedPropertiesKey.MinorId);
        }

        public static ExtendedPropertiesKey FromTable(Table table)
        {
            return new ExtendedPropertiesKey("OBJECT_OR_COLUMN", table.ID, 0);
        }

        public static ExtendedPropertiesKey FromColumn(Column column)
        {
            return new ExtendedPropertiesKey("OBJECT_OR_COLUMN", ((Table)column.Parent).ID, column.ID);
        }

        public static ExtendedPropertiesKey FromStoredProcedure(StoredProcedure storedProcedure)
        {
            return new ExtendedPropertiesKey("OBJECT_OR_COLUMN", storedProcedure.ID, 0);
        }

        public static ExtendedPropertiesKey FromDatabase()
        {
            return new ExtendedPropertiesKey("DATABASE", 0, 0);
        }

    }