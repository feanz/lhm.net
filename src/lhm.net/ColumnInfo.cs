namespace lhm.net
{
    public class ColumnInfo 
    {
        public string Catalog { get; set; }

        public string Schema { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }

        public int MaxLength { get; set; }

        public int OrdinalPostion { get; set; }

        public bool IsNullable { get; set; }

        public bool IsIdentity { get; set; }

        protected bool Equals(ColumnInfo other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ColumnInfo) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}