namespace lhm.net
{
    public class IndexDef
    {
        public IndexDef(string columnName, IndexOrder order)
        {
            ColumnName = columnName;
            Order = order;
        }
        public string ColumnName { get; }
        public IndexOrder Order { get; }

        public override string ToString()
        {
            return $"{ColumnName} {Order}";
        }
    }
}
