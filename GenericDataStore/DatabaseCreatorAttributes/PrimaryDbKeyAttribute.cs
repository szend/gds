namespace DatabaseCreatorAttributes
{
    public class PrimaryDbKeyAttribute : System.Attribute
    {
        public PrimaryDbKeyAttribute()
        {


        }
    }

    public class ForeignDbKeyAttribute : System.Attribute
    {
        public string ConnectedTableName { get; set; }
        public ForeignDbKeyAttribute(string connecttable)
        {
            ConnectedTableName = connecttable;
        }
    }
}