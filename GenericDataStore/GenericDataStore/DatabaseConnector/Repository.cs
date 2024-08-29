using GenericDataStore.Filtering;
using GenericDataStore.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GenericDataStore.DatabaseConnector
{
    public class Repository
    {
        IDataConnector sQLConnector;
        public List<Type> types = new List<Type>();
        public List<ClassDescription> classDescriptions = new List<ClassDescription>();
        public string sqltype { get; set; }
        public Repository(string connectstring,string type)
        {
            if (type.ToLower() == "mysql")
            {
                sQLConnector = new MySqlConnector(connectstring);
                sqltype = "mysql";
            }
            else if (type.ToLower() == "sql server")
            {
                sQLConnector = new SQLConnector(connectstring);
                sqltype = "sql server";
            }
        }

        public List<DatabaseTableRelations>? databaseTableRelations()
        {
            try
            {
                return sQLConnector.GetConnections();

            }
            catch (Exception)
            {

                return null;
            }
        }
        public void Create()
        {
            // Create the database
        }
        public ClassDescription GetPropertyes(string tablename)
        {
            var types = sQLConnector.GetAllTable(new List<string>() { tablename });
            List<ClassDescription> classDescriptions = new List<ClassDescription>();
            if(types.Count == 0)
            {
                return null;
            }
            foreach (var t in types)
            {
                classDescriptions.Add(new ClassDescription(t));
            }



            return classDescriptions[0];


        }

        public List<string> GetAllTableName()
        {
            var all = sQLConnector.GetAllTableName();
            return all;
        }

        //public bool CreateUserIdField(string tablename, Guid userid)
        //{
        //    return sQLConnector.CreateUserIdField(tablename, userid);
        //}

        public List<DataObject> GetAllDataFromTable(ObjectType objtype, RootFilter? filter = null, bool chart = false, bool onlyfirstx = false)
        {
            return sQLConnector.GetAllDataFromTable(objtype, filter, chart,onlyfirstx);
        }

        public int GetCount(ObjectType objtype, RootFilter? filter = null)
        {
            return sQLConnector.GetCount(objtype,filter);
        }

        public bool CreateTable(ObjectType objtype, string idtype)
        {
            if(idtype == "int")
            {
                idtype = "  int NOT NULL IDENTITY(1,1) ";
            }
            else
            {
                idtype = "  uniqueidentifier NOT NULL DEFAULT NEWID() ";
            }
            return sQLConnector.CreateTable(objtype,idtype);
        }

        public bool AddChild(ObjectType objtype,ObjectType parent, string idtype, bool virtualconnection = false)
        {
            string parenttype = "uniqueidentifier";
            if (idtype == "int")
            {
                idtype = "  int NOT NULL IDENTITY(1,1) ";
                parenttype = "int";
            }
            else
            {
                idtype = "  uniqueidentifier NOT NULL DEFAULT NEWID() ";
                parenttype = "uniqueidentifier";
            }
            return sQLConnector.AddChild(objtype,parent,idtype,parenttype,virtualconnection);
        }

        public bool RemoveColumn(string tablename, string columnname)
        {
            return sQLConnector.RemoveColumn(tablename, columnname);
        }

        public bool AddColumn(string tablename, string columnname, string columntype)
        {
            return sQLConnector.AddColumn(tablename, columnname, columntype);
        }

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype)
        {
            return sQLConnector.UpdateColumn(tablename, columnname, newcolumnname, columntype);
        }

        public bool UpdateColumnType(string tablename, string columnname, string columntype)
        {
            return sQLConnector.UpdateColumnType(tablename, columnname, columntype);
        }

        //public bool UpdateColumnNullable(string tablename, string columnname, bool nullable)
        //{
        //    return sQLConnector.UpdateColumnNullable(tablename, columnname, nullable);
        //}

        public bool DropTable(string tablename)
        {
            return sQLConnector.DropTable(tablename);
        }

        private string GetValueString(Field field, DataObject obj)
        {
            var res = obj.Value.FirstOrDefault(x => x.Name == field.Name).ValueString;
            if(res == null || res == "")
            {
                return "''";
            }
            else
            if(field.Type == "date")
            {
                if(res == "NA")
                {
                    return "''";
                }
                return $"'{res.Split('.')[2] + "." + res.Split('.')[1] + "." + res.Split('.')[0]}'";
            }
            else if (field.Type == "numeric")
            {
                try
                {
                    var num = double.Parse(res);
                }
                catch (Exception)
                {

                    return "''";
                }
                return res.Replace(",",".");
            }
            else if (field.Type == "boolean")
            {
                try
                {
                    var num = bool.Parse(res);
                }
                catch (Exception)
                {

                    return "''";
                }
                return $"{(res == "true" || res == "True" ? 1 : 0)}";
             }
            else
            {
                return $"'{res.Replace("'","''")}'";
            }
        }

        public bool UpdateValue(ObjectType typ, DataObject obj)
        {
            Dictionary<string,string> keyValuePairs = new Dictionary<string, string>();
            foreach (var item in typ.Field.Where(x => x.Name != "AppUserId" && x.Name != "DataObjectId" && x.Type != "id" && !x.Type.Contains("calculated")))
            {
                var valuesstring = GetValueString(item, obj);
                if(valuesstring != "''")
                {
                    keyValuePairs.Add(item.Name, valuesstring);
                }
            }
            var idfields = typ.Field.Where(y => y.Type == "id").ToList();
            var ids = obj.Value.Where(x =>  idfields.Any(y => y.Name == x.Name)).ToDictionary(pair => pair.Name, pair => pair.ValueString);


            return sQLConnector.UpdateValues(typ.Name,keyValuePairs,ids);
        }

        public bool CreateValue(ObjectType typ, DataObject obj)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var item in typ.Field.Where(x => x.Name != "AppUserId" && x.Name != "DataObjectId" && x.Type != "id" && !x.Type.Contains("calculated")))
            {
                var valuesstring = GetValueString(item, obj);
                if (valuesstring != "''")
                {
                    if(sqltype == "mysql")
                    {
                        keyValuePairs.Add("`" + item.Name + "`", valuesstring);
                    }
                    else if (sqltype == "sql server")
                    {
                        keyValuePairs.Add("[" + item.Name + "]", valuesstring);
                    }
                }
            }
            return sQLConnector.InsertVlaues(typ.Name, keyValuePairs);
        }

        public DataObject GetByDataObjectId(ObjectType typ, Dictionary<string, string> keyValuePairs)
        {
            return sQLConnector.GetByDataObjectId(typ.Name, keyValuePairs);
        }

        public bool DeleteValue(ObjectType typ, Dictionary<string, string> keyValuePairs)
        {
            return sQLConnector.DeleteValue(typ.Name, keyValuePairs);
        }

        public bool CreateRelation(string tablename1, string tablename2, string columnname1, string columnname2, string idtype, bool virtualconnection = false)
        {
            string parenttype = "uniqueidentifier";
            if (idtype == "int")
            {
                idtype = "  int NOT NULL IDENTITY(1,1) ";
                parenttype = "int";
            }
            else
            {
                idtype = "  uniqueidentifier NOT NULL DEFAULT NEWID() ";
                parenttype = "uniqueidentifier";
            }
            return sQLConnector.CreateRealtion(tablename1, columnname1, tablename2, columnname2,parenttype, virtualconnection);
        }

        public bool RemoveRelation(string tablename1, string tablename2, string columnname1, string columnname2)
        {
            return sQLConnector.RemoveRealtion(tablename1, columnname1, tablename2, columnname2);
        }

        public bool RenameTable(string tablename, string newtablename)
        {
            return sQLConnector.RenameTable(tablename, newtablename);
        }

    }
}
