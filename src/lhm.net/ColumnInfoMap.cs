
namespace lhm.net
{
    public class ColumnInfoMap
    {
        public ColumnInfoMap(ColumnInfo originColumns, ColumnInfo destinationColumns)
        {
            OriginColumns = originColumns;
            DestinationColumns = destinationColumns;
        }

        public ColumnInfo OriginColumns { get; private set; }

        public ColumnInfo DestinationColumns { get; private set; }
    }
}
