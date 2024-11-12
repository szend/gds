using GenericDataStore.Filtering;
using GenericDataStore.InputModels;
using GenericDataStore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using Npgsql;

namespace GenericDataStore.DatabaseConnector
{
    public class SQLConnector : IDataConnector
    {
        public string ConnectionString { get; set; }
        public SQLConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool DeleteValue(string tablename, Dictionary<string, string> ids)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                
                string oString = @"DELETE FROM [" + tablename + "] WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "["+x+"]" + " = " + (int.TryParse(y, out int n) == true ? y : "'"+y+"'" )+ ""));
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }


        public bool UpdateValues(string tablename, Dictionary<string,string> fieldvalues, Dictionary<string, string> ids)
        {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    string oString = @"UPDATE  [" + tablename + "] SET " + string.Join(", ", fieldvalues.Keys.Zip(fieldvalues.Values, (x, y) => "[" + x + "]" + " = " + y + "")) + " WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "[" + x + "]" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            
            return true;
        }

        public bool InsertVlaues(string tablename, Dictionary<string, string> fieldvalues)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"INSERT INTO [" + tablename + "] (" + string.Join(",",   fieldvalues.Keys  ) + ") VALUES (" + string.Join(",", fieldvalues.Values) + ")";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;

        }

        public DataObject GetByDataObjectId(string tablename, Dictionary<string, string> ids)
        {

            DataObject obj = new DataObject();
            

                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    string oString = @"SELECT * FROM [" + tablename + "] WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "[" + x + "]" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
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
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                try
                {
                    SqlCommand oCmd = new SqlCommand(query, myConnection);
                    myConnection.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
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
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string dbname = ConnectionString.Split(';').FirstOrDefault(x => x.Contains("Initial Catalog") || x.Contains("Database")).Split('=')[1];
                string oString = @"
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='" + dbname + "'";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
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

            string dbname = ConnectionString.Split(';').FirstOrDefault(x => x.Contains("Initial Catalog") || x.Contains("Database")).Split('=')[1];

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
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"SELECT * FROM [" + objtype.TableName + "]";

                if (filter != null && filter.ValueFilters != null  && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "[" + x.Field + "]"  + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field),x.Operator,x.Value)));

                }
                if (filter != null  && filter.ValueSortingParams != null && filter.ValueSortingParams.Any())
                {
                    oString += " Order By " + "[" + filter.ValueSortingParams.FirstOrDefault().Field + "]";
                    if (filter.ValueSortingParams.FirstOrDefault().Order == 1)
                    {
                        oString += " DESC";
                    }
                }
                if(filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == false)
                {
                    if(filter == null  || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any())
                    {
                        oString += " Order By " + "[" + objtype.Field.FirstOrDefault(x => x.Type == "id").Name + "]";
                    }
                    oString += @" OFFSET " + filter.ValueSkip + " ROWS FETCH NEXT " + filter.ValueTake + " ROWS ONLY";
                }
                else if (filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == true && onlyfirstx == true)
                {
                    if (filter == null || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any())
                    {
                        oString += " Order By " + "[" + objtype.Field.FirstOrDefault(x => x.Type == "id").Name + "]";
                    }
                    oString += @" OFFSET " + filter.ValueSkip + " ROWS FETCH NEXT " + filter.ValueTake + " ROWS ONLY";
                }

                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {


                    while (oReader.Read())
                    {
                        DataObject obj = new DataObject();
                        int idx = 0;
                        foreach (var item in objtype.Field)
                        {
                            try
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
                            }
                            catch (Exception)
                            {

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
                    return "= " + GetValueString(field,res);
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
                    if(field.Type == "boolean")
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
            if (res == null || res.ToString() == "" || field == null || field.Type == "" || field.Type == null)
            {
                return "''";
            }
            else
if (field?.Type == "date")
            {
                if (res == "NA")
                {
                    return "''";
                }
                return $"CONVERT(datetime,'{res.ToString().Split('/')[0] + "." + (int.Parse(res.ToString().Split('/')[1]) + 1).ToString() + "." + res.ToString().Split('/')[2].Split(' ')[0]}')";

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
                return $"{(res.ToString() == "true" || res.ToString() == "True" ? 1 : 0)}";
            }
            else
            {
                return $"'{res}'";
            }
        }
        public string CreateClass(string name)
        {
            string result = "";
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"declare @TableName sysname = '" + name + @"'" + @"
declare @Result varchar(max) = 'public class ' + @TableName + '
{'

select @Result = @Result + KeySign + NameSing + '
    public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; }
'
from
(
    select 
       replace(replace(col.name, '-', '_'),' ','_') ColumnName,
        column_id ColumnId,
        case typ.name 
            when 'bigint' then 'long'
            when 'float (53)' then 'double'
            when 'binary' then 'byte[]'
            when 'bit' then 'bool'
            when 'char' then 'string'
            when 'date' then 'DateTime'
            when 'datetime' then 'DateTime'
            when 'datetime2' then 'DateTime'
            when 'datetimeoffset' then 'DateTimeOffset'
            when 'decimal' then 'decimal'
            when 'float' then 'double'
            when 'image' then 'byte[]'
            when 'int' then 'int'
            when 'money' then 'decimal'
            when 'nchar' then 'string'
            when 'ntext' then 'string'
            when 'numeric' then 'decimal'
            when 'nvarchar' then 'string'
            when 'real' then 'float'
            when 'smalldatetime' then 'DateTime'
            when 'smallint' then 'short'
            when 'smallmoney' then 'decimal'
            when 'text' then 'string'
            when 'time' then 'TimeSpan'
            when 'timestamp' then 'long'
            when 'tinyint' then 'byte'
            when 'uniqueidentifier' then 'Guid'
            when 'varbinary' then 'byte[]'
            when 'varchar' then 'string'
            else 'UNKNOWN_' + typ.name
        end ColumnType,
        case 
            when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') 
            then '?' 
            else '' 
        end NullableSign,
        case 
			when col.column_id = 1 then ' [PrimaryDbKey] '
			else ''
		end KeySign,
        case 
			when 1 = 1 then ' [FieldName(" + "\"" + "' + col.name +"  + "'\"" +@")] '
		end NameSing
    from sys.columns col
        join sys.types typ on
            col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id
    where object_id = object_id(@TableName)
) t
order by ColumnId

set @Result = @Result  + '
}'

Select @Result
";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        result = oReader.GetString(0);
                    }

                    myConnection.Close();
                }
            }
            result = result.Replace("public class", "public partial class");
            return result;
        }
        public int GetCount(ObjectType objtype, RootFilter? filter = null)
        {

            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"SELECT COUNT(*) FROM [" + objtype.TableName + "]";
                if (filter != null && filter.ValueFilters != null && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "[" + x.Field + "]" + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field), x.Operator, x.Value)));
                }
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
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
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"
SELECT
    fk.name 'FK Name',
    tp.name 'Parent table',
    cp.name, cp.column_id,
    tr.name 'Refrenced table',
    cr.name, cr.column_id
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN 
    sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN 
    sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
INNER JOIN 
    sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN 
    sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
ORDER BY
    tp.name, cp.column_id
";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        DatabaseTableRelations databaseTableRelation = new DatabaseTableRelations();
                        databaseTableRelation.FKName = oReader.GetString(0);
                        databaseTableRelation.ParentTable = oReader.GetString(4);
                        databaseTableRelation.ParentPropertyName = oReader.GetString(5);
                        databaseTableRelation.ParentColumnId = oReader.GetInt32(6);
                        databaseTableRelation.ChildTable = oReader.GetString(1);
                        databaseTableRelation.ChildPropertyName = oReader.GetString(2);
                        databaseTableRelation.ChildTableId = oReader.GetInt32(3);
                        databaseTableRelations.Add(databaseTableRelation);
                    }

                    myConnection.Close();
                }
            }

            return databaseTableRelations;
        }

        public bool CreateTable(ObjectType objtype,string idtype)
        {

            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                var pkey = objtype.Field.Where(x => x.Type == "id").Count() > 0 ? objtype.Field.Where(x => x.Type == "id").Select(x => x.Name) : new List<string> { objtype.TableName + "Id" };
                string oString = @"CREATE TABLE [" + objtype.Name + "] (" + string.Join(",", pkey.Select(x => "[" + x + "]" + idtype)) + " , " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => "[" + x.Name + "] " + GetType(x.Type))) + ", PRIMARY KEY ("+ string.Join(",", pkey.Select(x => "["+x+"]")) + "))";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool AddChild(ObjectType objtype, ObjectType parent, string idtype, string parentidtype, bool virtualconnection = false)
        {

            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                if(virtualconnection == false)
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = @"CREATE TABLE [" + objtype.Name + "] (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) + ", PRIMARY KEY (" + pkey + "), FOREIGN KEY (" + parent.Name + "Id) REFERENCES " + parent.Name + "(" + parent.Field.FirstOrDefault(x => x.Type == "id").Name + "))";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
                else
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = @"CREATE TABLE [" + objtype.Name + "] (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " +
                        string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) +
                        ", PRIMARY KEY (" + pkey + "), )";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }

            }
            return true;
        }

        public bool RemoveColumn(string tablename, string columnname)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE [" + tablename + "] DROP COLUMN [" + columnname +"]";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool AddColumn(string tablename, string columnname, string columntype)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE [" + tablename + "] ADD [" + columnname + "] " + GetType(columntype);
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"EXEC  sp_rename '" + tablename+ "." + columnname+"', '" + newcolumnname+"', 'COLUMN'";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumnType(string tablename, string columnname, string columntype)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE [" + tablename + "] ALTER COLUMN [" + columnname + "] " + GetType(columntype);
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool DropTable(string tablename)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"DROP TABLE [" + tablename + "]";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
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
            if(virtualrelation == false)
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    string oString = @"ALTER TABLE [" + childtable + "] ADD CONSTRAINT FK_" + parenttable + "_" + childtable + " FOREIGN KEY (" + childcolumn + ") REFERENCES " + parenttable + "(" + parentcolumn + ")";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            }

            return true;
        }

        public bool RemoveRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE [" + childtable + "] DROP CONSTRAINT FK_" + parenttable + "_" + childtable;
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool RenameTable(string oldname, string newname)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string oString = @"EXEC sp_rename '" + oldname + "', '" + newname + "'";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
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
                return "float(53)";
            }
            else if (originaltype == "date")
            {
                return "DateTime";
            }
            else if (originaltype == "boolean")
            {
                return "bit";
            }
            else if (originaltype == "id")
            {
                return "uniqueidentifier";
            }
            else if (originaltype == "uniqueidentifier")
            {
                return "uniqueidentifier";
            }
            else if (originaltype == "int")
            {
                return "int";
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
