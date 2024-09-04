using GenericDataStore.Filtering;
using GenericDataStore.Models;
using MySqlConnector;

namespace GenericDataStore.DatabaseConnector
{
    public interface IDataConnector
    {
        public string ConnectionString { get; set; }

        public bool DeleteValue(string tablename, Dictionary<string, string> ids);



        public bool UpdateValues(string tablename, Dictionary<string, string> fieldvalues, Dictionary<string, string> ids);
        public bool InsertVlaues(string tablename, Dictionary<string, string> fieldvalues);

        public DataObject GetByDataObjectId(string tablename, Dictionary<string, string> keyValuePairs);

        public string ExecuteQuery(string query);

        public List<string> GetAllTableName();

        public List<Type> GetAllTable(List<string> tablesname);
        public List<DataObject> GetAllDataFromTable(ObjectType objtype, RootFilter? filter, bool chart = false, bool onlyfirstx = false);
        public string CreateClass(string name);

        public int GetCount(ObjectType objtype, RootFilter? filter = null);

        public List<DatabaseTableRelations> GetConnections();

        public bool CreateTable(ObjectType objtype, string idtype);


        public bool AddChild(ObjectType objtype, ObjectType parent, string idtype, string parentidtype, bool virtualconnection = false);


        public bool RemoveColumn(string tablename, string columnname);


        public bool AddColumn(string tablename, string columnname, string columntype);

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype);


        public bool UpdateColumnType(string tablename, string columnname, string columntype);

        public bool DropTable(string tablename);

        public bool CreateRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn, string idtype, bool virtualrelation = false);

        public bool RemoveRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn);

        public bool RenameTable(string oldname, string newname);

    }
}
