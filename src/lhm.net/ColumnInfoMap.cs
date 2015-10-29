
namespace lhm.net
{
    public class ColumnInfoMap
    {
        public ColumnInfoMap(ColumnInfo originColumnInfo, ColumnInfo destinationColumnInfo)
        {
            OriginColumnInfo = originColumnInfo;
            DestinationColumnInfo = destinationColumnInfo;
        }

        public ColumnInfo OriginColumnInfo { get; private set; }

        public ColumnInfo DestinationColumnInfo { get; private set; }
    }
}
