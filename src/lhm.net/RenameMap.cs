namespace lhm.net
{
    public class RenameMap
    {
        public RenameMap(string oldColumnName, string newColumnName)
        {
            OldColumnName = oldColumnName;
            NewColumnName = newColumnName;
        }

        public string OldColumnName { get; private set; }
        public string NewColumnName { get; private set; }
    }
}