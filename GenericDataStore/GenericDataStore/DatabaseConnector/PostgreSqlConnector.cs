using GenericDataStore.Filtering;
using GenericDataStore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using Npgsql;

namespace GenericDataStore.DatabaseConnector
{
    public class PostgreSqlConnector : IDataConnector
    {
        public string ConnectionString { get; set; }

        public PostgreSqlConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool DeleteValue(string tablename, Dictionary<string, string> ids)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {

                string oString = "DELETE FROM \"" + tablename + "\" WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "\"" + x + "\"" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }


        public bool UpdateValues(string tablename, Dictionary<string, string> fieldvalues, Dictionary<string, string> ids)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "UPDATE  \"" + tablename + "\" SET " + string.Join(",", fieldvalues.Keys.Zip(fieldvalues.Values, (x, y) => "\"" + x + "\"" + " = " + y + "")) + " WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "\"" + x + "\"" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }

            return true;
        }

        public bool InsertVlaues(string tablename, Dictionary<string, string> fieldvalues)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "INSERT INTO \"" + tablename + "\" (" + string.Join(",", fieldvalues.Keys) + ") VALUES (" + string.Join(",", fieldvalues.Values) + ")";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;

        }

        public DataObject GetByDataObjectId(string tablename, Dictionary<string, string> ids)
        {

            DataObject obj = new DataObject();

            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "SELECT * FROM \"" + tablename + "\" WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "\"" + x + "\"" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        obj.Value = new List<Value>();
                        for (int i = 0; i < oReader.FieldCount; i++)
                        {
                            if (oReader.GetName(i) != "DataObjectId" && oReader.GetName(i) != "AppUserId")
                            {
                                Value value = new Value()
                                {
                                    Name = oReader.GetName(i),
                                    ValueString = oReader[i].ToString(),
                                    ObjectTypeId = obj.ObjectTypeId,
                                    ValueId = Guid.NewGuid()
                                };
                                obj.Value.Add(value);
                            }
                        }
                    }

                    myConnection.Close();
                }
            }


            return obj;
        }
        public string ExecuteQuery(string query)
        {
            string res = "";
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    NpgsqlCommand oCmd = new NpgsqlCommand(query, myConnection);
                    myConnection.Open();
                    using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            for (int i = 0; i < oReader.FieldCount; i++)
                            {
                                res = res + "| " + oReader[i].ToString() + " |";
                            }
                            res += "\n";
                        }

                        myConnection.Close();
                    }
                }
                catch (Exception e)
                {
                    res = e.Message;

                }

            }
            return res ?? "";
        }

        public List<string> GetAllTableName()
        {
            List<string> tables = new List<string>();
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string dbname = ConnectionString.Split(';').FirstOrDefault(x => x.Contains("Initial Catalog") || x.Contains("Database")).Split('=')[1];
                string oString = "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema';";
                //                string oString = @"
                //SELECT TABLE_NAME
                //FROM INFORMATION_SCHEMA.TABLES
                //WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='" + dbname + "'";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {
                    int idx = 0;

                    while (oReader.Read())
                    {
                        string classname = oReader[0].ToString();

                        if (classname != null)
                        {
                            tables.Add(classname);
                        }
                        idx++;
                    }

                    myConnection.Close();
                }
            }
            return tables;
        }

        public List<Type> GetAllTable(List<string> tablesname)
        {
            List<Type> types = new List<Type>();
            int idx = 0;
            foreach (var item in tablesname)
            {
                string resultClassString = CreateClass(item);
                var resultClassType = ClassTypeBuilder.Create(resultClassString, item);
                if (resultClassType != null)
                {
                    types.Add(resultClassType);
                }
                idx++;

            }



            return types;
        }

        public List<DataObject> GetAllDataFromTable(ObjectType objtype, RootFilter? filter, bool chart, bool onlyfirstx = false)
        {
            List<DataObject> list = new List<DataObject>();
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "SELECT * FROM \"" + objtype.TableName + "\"";

                if (filter != null && filter.ValueFilters != null && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "\"" + x.Field + "\"" + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field), x.Operator, x.Value)));

                }
                if (filter != null && filter.ValueSortingParams != null && filter.ValueSortingParams.Any())
                {
                    oString += " Order By " + "\"" + filter.ValueSortingParams.FirstOrDefault().Field + "\"";
                    if (filter.ValueSortingParams.FirstOrDefault().Order == 1)
                    {
                        oString += " DESC";
                    }
                }
                if (filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == false)
                {
                    if (filter == null || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any())
                    {
                        oString += " Order By " + "\"" + objtype.Field.FirstOrDefault(x => x.Type == "id").Name + "\"";
                    }
                    oString += @" LIMIT " + filter.ValueTake + " OFFSET " + filter.ValueSkip;
                }
                else if (filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == true && onlyfirstx == true)
                {
                    if (filter == null || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any())
                    {
                        oString += " Order By " + "\"" + objtype.Field.FirstOrDefault(x => x.Type == "id").Name + "\"";
                    }
                    oString += @" LIMIT " + filter.ValueTake + " OFFSET " + filter.ValueSkip;
                }

                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {


                    while (oReader.Read())
                    {
                        DataObject obj = new DataObject();
                        int idx = 0;
                        foreach (var item in objtype.Field)
                        {
                            if (item.Type.Contains("calculated"))
                            {
                                Value value = new Value()
                                {
                                    Name = item.Name,
                                    ValueString = null,

                                    ObjectTypeId = obj.ObjectTypeId,
                                    ValueId = Guid.NewGuid()
                                };
                                obj.Value.Add(value);
                            }
                            else
                            {
                                var fieldvalue = oReader[item.Name];
                                if (fieldvalue != DBNull.Value && item.Name != "AppUserId" && item.Name != "DataObjectId")
                                {
                                    Value value = new Value()
                                    {
                                        Name = item.Name,
                                        ValueString = fieldvalue.ToString(),

                                        ObjectTypeId = obj.ObjectTypeId,
                                        ValueId = Guid.NewGuid()
                                    };
                                    obj.Value.Add(value);
                                }
                                else
                                {
                                    Value value = new Value()
                                    {
                                        Name = item.Name,
                                        ValueString = null,

                                        ObjectTypeId = obj.ObjectTypeId,
                                        ValueId = Guid.NewGuid()
                                    };
                                    obj.Value.Add(value);
                                }
                            }

                            idx++;
                        }
                        list.Add(obj);
                    }

                    myConnection.Close();
                }
            }
            return list;

        }

        private string GetOperatorAndValueString(Field field, string o, object res)
        {
            switch (o.ToLower())
            {
                case "equals":
                    return "= " + GetValueString(field, res);
                    break;
                case "notequals":
                    return "<> " + GetValueString(field, res);
                    break;

                case "lt":
                    return "< " + GetValueString(field, res);

                    break;

                case "lte":
                    return "<= " + GetValueString(field, res);

                    break;

                case "gt":
                    return "> " + GetValueString(field, res);

                    break;

                case "gte":
                    return ">= " + GetValueString(field, res);

                    break;

                case "contains":
                    if (field.Type == "boolean")
                    {
                        return "= " + GetValueString(field, res);
                    }
                    return "LIKE '%" + res + "%' ";

                    break;
                case "notcontains":
                    return "NOT LIKE '%" + res + "%' ";
                    break;


                case "startswith":
                    return "LIKE '" + res + "%' ";
                    break;

                case "endswith":
                    return "LIKE '%" + res + "' ";
                    break;

                case "dateis":
                    return "= " + GetValueString(field, res);

                    break;

                case "dateisnot":
                    return "<> " + GetValueString(field, res);

                    break;

                case "datebefore":
                    return "< " + GetValueString(field, res);

                    break;
                case "dateafter":
                    return "> " + GetValueString(field, res);
                    break;
                default:
                    return "=" + GetValueString(field, res);
                    break;

            }
            return "=" + GetValueString(field, res);
        }

        private object GetValueString(Field field, object res)
        {
            if (res == null || res.ToString() == "")
            {
                return "''";
            }
            else
if (field.Type == "date")
            {
                if (res == "NA" || res == null)
                {
                    return "''";
                }
                return $"'{res.ToString().Split('/')[0] + "/" + (int.Parse(res.ToString().Split('/')[1]) + 1).ToString() + "/" + res.ToString().Split('/')[2].Split(' ')[0]}'";
                //return $"CONVERT(datetime,'{res.ToString().Split('.')[1] + "." + res.ToString().Split('.')[0] + "." + res.ToString().Split('.')[2]}')";
            }
            else if (field.Type == "numeric")
            {
                try
                {
                    var num = double.Parse(res.ToString());
                }
                catch (Exception)
                {

                    return "''";
                }
                return res;
            }
            else if (field.Type == "boolean")
            {
                try
                {
                    var num = bool.Parse(res.ToString());
                }
                catch (Exception)
                {

                    return "''";
                }
                return $"{(res.ToString() == "true" || res.ToString() == "True" ? "true" : "false")}";
            }
            else
            {
                return $"'{res}'";
            }
        }

        public string CreateClass(string name)
        {
            string? oString = @"
SELECT 
case 
when column_name = (SELECT c.column_name 
FROM information_schema.table_constraints tc 
JOIN information_schema.constraint_column_usage AS ccu USING (constraint_schema, constraint_name)  
JOIN information_schema.columns AS c ON c.table_schema = tc.constraint_schema 
  AND tc.table_name = c.table_name AND ccu.column_name = c.column_name 
WHERE constraint_type = 'PRIMARY KEY' and tc.table_name = '"+ name + @"') THEN '[PrimaryDbKey] '
    else ''
    end 
 ||
' [FieldName(" + "\"" + "' + column_name +" + "'\"" + @")] '
||
'public ' ||
  case
    when data_type = 'uuid' THEN 'Guid'
    when data_type = 'character varying' THEN 'string'
    when data_type = 'text' then 'string'
    when data_type = 'json' then 'string'
    when data_type = 'integer' then 'int'
    when data_type = 'boolean' then 'bool'
    when data_type = 'numeric' then 'double'
    when data_type = 'date' then 'DateTime'
    when data_type = 'bigint' then 'long'
    when data_type = 'timestamp with time zone' then 'DateTimeOffset'
    when data_type = 'timestamp without time zone' then 'DateTime'
        when data_type = 'money' then 'decimal'
    when data_type = 'numeric' then 'decimal'
    when data_type = 'jsonb' then 'string'
      when data_type = 'real' then 'float'
    when data_type = 'ARRAY' then
        (case when udt_name = '_text' then 'string'
            when udt_name = '_uuid' then 'Guid'
            when udt_name = '_int4' then 'int'
         else data_type end)
                                                                     || '[]'
    else 'string'
    end
  ||
  ' ' || column_name || ' { get; set; } '

  as sql 
FROM information_schema.columns 
WHERE table_schema = 'public'
  AND table_name   = '" + name + @"';
                ";
            string result = "";
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {



                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        for (int i = 0; i < oReader.FieldCount; i++)
                        {
                            result = result + " " + oReader[i].ToString() + " ";
                        }        
                    }

                    myConnection.Close();
                }
            }
            result = "public class " + name + " { " + result + " }";
            return result;
        }



        public int GetCount(ObjectType objtype, RootFilter? filter = null)
        {

            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "SELECT COUNT(*) FROM \"" + objtype.TableName + "\"";
                if (filter != null && filter.ValueFilters != null && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "\"" + x.Field + "\"" + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field), x.Operator, x.Value)));
                }
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        return oReader.GetInt32(0);
                    }

                    myConnection.Close();
                }
            }
            return 0;
        }
        public List<DatabaseTableRelations> GetConnections()
        {
            List<DatabaseTableRelations> databaseTableRelations = new List<DatabaseTableRelations>();
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = @"
SELECT
tc.constraint_name, tc.table_name, kcu.column_name, 
ccu.table_name AS foreign_table_name,
ccu.column_name AS foreign_column_name 
FROM 
information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE constraint_type = 'FOREIGN KEY'
";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                using (NpgsqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        DatabaseTableRelations databaseTableRelation = new DatabaseTableRelations();
                        databaseTableRelation.FKName = oReader.GetString(0);
                        databaseTableRelation.ParentTable = oReader.GetString(3);
                        databaseTableRelation.ParentPropertyName = oReader.GetString(4);
                        databaseTableRelation.ChildTable = oReader.GetString(1);
                        databaseTableRelation.ChildPropertyName = oReader.GetString(2);
                        databaseTableRelations.Add(databaseTableRelation);
                    }

                    myConnection.Close();
                }
            }

            return databaseTableRelations;
        }


        public bool CreateTable(ObjectType objtype, string idtype)
        {
            //if (idtype == "  int NOT NULL IDENTITY(1,1) ")
            //{
            //    idtype = "  SERIAL PRIMARY KEY ";
            //}
            //else if (idtype == "  uniqueidentifier NOT NULL DEFAULT NEWID() ")
            //{
            //    idtype = "  uuid DEFAULT gen_random_uuid() ";
            //}

            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                var pkey = objtype.Field.Where(x => x.Type == "id").Count() > 0 ? objtype.Field.Where(x => x.Type == "id").Select(x => x.Name) : new List<string> { objtype.TableName + "Id" };
                string oString = "CREATE TABLE \"" + objtype.Name + "\" (" + string.Join(",", pkey.Select(x => "\"" + x + "\"" + idtype)) + " , " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => "\"" + x.Name + "\" " + GetType(x.Type))) + ", PRIMARY KEY (" + string.Join(",", pkey.Select(x => "\"" + x + "\"")) + "))";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }





        public bool AddChild(ObjectType objtype, ObjectType parent, string idtype, string parentidtype, bool virtualconnection = false)
        {
            //if (idtype == "  int NOT NULL IDENTITY(1,1) ")
            //{
            //    idtype = "  SERIAL PRIMARY KEY ";
            //    parentidtype = " serial ";
            //}
            //else if (idtype == "  uniqueidentifier NOT NULL DEFAULT NEWID() ")
            //{
            //    idtype = "  uuid DEFAULT gen_random_uuid() ";
            //    parentidtype = " char(36) ";
            //}

            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                if (virtualconnection == false)
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = "CREATE TABLE \"" + objtype.Name + "\" (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) + ", PRIMARY KEY (" + pkey + "), FOREIGN KEY (" + parent.Name + "Id) REFERENCES " + parent.Name + "(" + parent.Field.FirstOrDefault(x => x.Type == "id").Name + "))";
                    NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
                else
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = "CREATE TABLE \"" + objtype.Name + "\" (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " +
                        string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) +
                        ", PRIMARY KEY (" + pkey + "), )";
                    NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
            return true;
        }

        public bool RemoveColumn(string tablename, string columnname)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + tablename + "\" DROP COLUMN \"" + columnname + "\"";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool AddColumn(string tablename, string columnname, string columntype)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + tablename + "\" ADD \"" + columnname + "\" " + GetType(columntype);
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + tablename + "\" RENAME COLUMN \"" + columnname + "\" TO \"" + newcolumnname + "\"";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumnType(string tablename, string columnname, string columntype)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + tablename + "\" MODIFY  \"" + columnname + "\" " + GetType(columntype);
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }


        public bool DropTable(string tablename)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "DROP TABLE \"" + tablename + "\"";
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }


        public bool CreateRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn, string idtype, bool virtualrelation = false)
        {
            try
            {
                this.AddColumn(childtable, childcolumn, idtype);
            }
            catch (Exception)
            {

            }
            if (virtualrelation == false)
            {
                using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
                {
                    string oString = "ALTER TABLE \"" + childtable + "\" ADD CONSTRAINT FK_" + parenttable + "_" + childtable + " FOREIGN KEY (\"" + childcolumn + "\") REFERENCES " + parenttable + " (\"" + parentcolumn + "\")";
                    NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
            return true;
        }

        public bool RemoveRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + childtable + "\" DROP CONSTRAINT FK_" + parenttable + "_" + childtable;
                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool RenameTable(string oldname, string newname)
        {
            using (NpgsqlConnection myConnection = new NpgsqlConnection(ConnectionString))
            {
                string oString = "ALTER TABLE \"" + oldname + "\" RENAME TO \"" + newname + "\"";

                NpgsqlCommand oCmd = new NpgsqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        private string GetType(string originaltype)
        {
            if (originaltype == "text")
            {
                return "varchar(255)";
            }
            else if (originaltype == "numeric")
            {
                return "numeric";
            }
            else if (originaltype == "date")
            {
                return "date";
            }
            else if (originaltype == "boolean")
            {
                return "bool";
            }
            else if (originaltype == "id")
            {
                return "char(36)";
            }
            else if (originaltype == "foreignkey")
            {
                return "char(36)";
            }
            else if (originaltype == "serial")
            {
                return "serial";
            }
            else if (originaltype == "integer")
            {
                return "integer";
            }
            else if (originaltype == "uuid")
            {
                return "uuid";
            }
            else
            {
                return "varchar(255)";
            }
        }

        public List<Type> GetAllTableApi(List<DatabaseTableRelations> relations, string name)
        {
            throw new NotImplementedException();
        }

        public List<DataObject> GetAllDataFromTableApi(ObjectType objtype, IMemoryCache memoryCache, RootFilter? filter, bool chart = false, bool onlyfirstx = false)
        {
            throw new NotImplementedException();
        }
    } 
}
