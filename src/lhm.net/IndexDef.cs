namespace lhm.net
{
    public class IndexDef
    {
        public IndexDef(string columnName, IndexOrder order)
        {
            ColumnName = columnName;
            Order = order;
        }
        public string ColumnName { get; private set; }
        public IndexOrder Order { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", ColumnName, Order);
        }
    }
}
