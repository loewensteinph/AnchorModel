namespace Anchor.Model.Core.BusinessLogic
{
    public class ForeignKey
    {
        public string ConstraintSuffix { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public string ReferencedColumnName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedTableType { get; set; }
        public bool IsIdentifier { get; set; }
    }
}