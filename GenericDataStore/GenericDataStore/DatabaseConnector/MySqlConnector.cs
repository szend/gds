﻿using GenericDataStore.Filtering;
using GenericDataStore.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.ML.Transforms;
using MySqlConnector;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace GenericDataStore.DatabaseConnector
{
    public class MySqlConnector : IDataConnector
    {
        public string ConnectionString { get; set; }
        public MySqlConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool DeleteValue(string tablename, Dictionary<string, string> ids)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {

                string oString = @"DELETE FROM `" + tablename + "` WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "`" + x + "`" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }


        public bool UpdateValues(string tablename, Dictionary<string, string> fieldvalues, Dictionary<string, string> ids)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"UPDATE  `" + tablename + "` SET " + string.Join(",", fieldvalues.Keys.Zip(fieldvalues.Values, (x, y) => "`" + x + "`" + " = " + y + "")) + " WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "`" + x + "`" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }

            return true;
        }

        public bool InsertVlaues(string tablename, Dictionary<string, string> fieldvalues)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"INSERT INTO `" + tablename + "` (" + string.Join(",", fieldvalues.Keys) + ") VALUES (" + string.Join(",", fieldvalues.Values) + ")";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;

        }

        public DataObject GetByDataObjectId(string tablename, Dictionary<string, string> ids)
        {

            DataObject obj = new DataObject();
  
                using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
                {
                    string oString = @"SELECT * FROM `" + tablename + "` WHERE " + string.Join(" AND ", ids.Keys.Zip(ids.Values, (x, y) => "`" + x + "`" + " = " + (int.TryParse(y, out int n) == true ? y : "'" + y + "'") + ""));
                    MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                    myConnection.Open();
                    using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    MySqlCommand oCmd = new MySqlCommand(query, myConnection);
                    myConnection.Open();
                    using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string dbname = ConnectionString.Split(';').FirstOrDefault(x => x.Contains("Initial Catalog") || x.Contains("Database")).Split('=')[1];
                string oString = "SHOW TABLES";
                //                string oString = @"
                //SELECT TABLE_NAME
                //FROM INFORMATION_SCHEMA.TABLES
                //WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='" + dbname + "'";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"SELECT * FROM `" + objtype.TableName + "`";

                if (filter != null && filter.ValueFilters != null && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "`" + x.Field + "`" + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field), x.Operator, x.Value)));

                }
                if (filter != null && filter.ValueSortingParams != null && filter.ValueSortingParams.Any())
                {
                    oString += " Order By " + "`" + filter.ValueSortingParams.FirstOrDefault().Field + "`";
                    if (filter.ValueSortingParams.FirstOrDefault().Order == 1)
                    {
                        oString += " DESC";
                    }
                }
                if (filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == false)
                {
                    if ((filter == null || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any()) && objtype.Field.FirstOrDefault(x => x.Type == "id") != null)
                    {
                        oString += " Order By " + "`" + objtype.Field.FirstOrDefault(x => x.Type == "id")?.Name + "`";
                    }
                    oString += @" LIMIT " + filter.ValueTake + " OFFSET " + filter.ValueSkip;
                }
                else if (filter != null && filter.ValueSkip != null && filter.ValueTake != null && filter.ValueTake > 0 && chart == true && onlyfirstx == true)
                {
                    if ((filter == null || filter.ValueSortingParams == null || !filter.ValueSortingParams.Any()) && objtype.Field.FirstOrDefault(x => x.Type == "id") != null)
                    {
                        oString += " Order By " + "`" + objtype.Field.FirstOrDefault(x => x.Type == "id")?.Name + "`";
                    }
                    oString += @" LIMIT " + filter.ValueTake + " OFFSET " + filter.ValueSkip;
                }

                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
                if (res == "NA")
                {
                    return "''";
                }
                return $" STR_TO_DATE('{res.ToString().Split('/')[0] + "." + (int.Parse(res.ToString().Split('/')[1]) + 1).ToString() + "." + res.ToString().Split('/')[2].Split(' ')[0]}','%m.%d.%Y')";

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
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string? oString = @"
                SET @table := '" + name + @"';
                SET group_concat_max_len = 2048;
                SELECT 
                    CONCAT('public class ', @table, '\n{\n', GROUP_CONCAT(DISTINCT  a.property_ SEPARATOR '\n'), '\n}') class_
                FROM 
                    (SELECT
                        CONCAT(
                            CASE
                            WHEN COLUMN_KEY = 'PRI' THEN CONCAT(' [PrimaryDbKey] ')
                            ELSE ''
                            END,
                      CONCAT(' [FieldName(" + "\"" + "',COLUMN_NAME ," + "'\"" + @")] '), '\tpublic ',
                        CASE 
                            WHEN DATA_TYPE = 'bigint' THEN CONCAT('long',IF(IS_NULLABLE = 'NO' , '', '?'))  
                            WHEN DATA_TYPE = 'BINARY' THEN CONCAT('byte[]',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'bit' THEN CONCAT('bool',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'char' THEN CONCAT('string')
                            WHEN DATA_TYPE = 'date' THEN CONCAT('DateTime',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'datetime' THEN CONCAT('DateTime',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'datetime2' THEN CONCAT('DateTime',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'datetimeoffset' THEN CONCAT('DateTimeOffset',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'decimal' THEN CONCAT('decimal',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'double' THEN CONCAT('double',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'enum' THEN CONCAT('enum',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'float' THEN CONCAT('float',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'image' THEN CONCAT('byte[]',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'int' THEN CONCAT('int',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'money' THEN CONCAT('decimal',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'nchar' THEN CONCAT('char',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'ntext' THEN CONCAT('string')
                            WHEN DATA_TYPE = 'numeric' THEN CONCAT('decimal',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'nvarchar' THEN CONCAT('string')
                            WHEN DATA_TYPE = 'real' THEN CONCAT('double',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'smalldatetime' THEN CONCAT('DateTime',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'smallint' THEN CONCAT('short',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'smallmoney' THEN CONCAT('decimal',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'text' THEN CONCAT('string')
                            WHEN DATA_TYPE = 'time' THEN CONCAT('TimeSpan',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'timestamp' THEN CONCAT('DateTime',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'tinyint' THEN CONCAT('bool',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'uniqueidentifier' THEN CONCAT('Guid',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'varbinary' THEN CONCAT('byte[]',IF(IS_NULLABLE = 'NO' , '', '?'))
                            WHEN DATA_TYPE = 'varchar' THEN CONCAT('string')
                            WHEN DATA_TYPE = 'longtext' THEN CONCAT('string')
                            ELSE CONCAT('_UNKNOWN_',IF(IS_NULLABLE = 'NO' , '', '?'))
                        END, ' ', 
                        COLUMN_NAME, ' {get; set;}') AS property_
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE table_name = @table) a
                ;
                ";



                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                using (MySqlDataReader oReader = oCmd.ExecuteReader())
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

            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"SELECT COUNT(*) FROM `" + objtype.TableName + "`";
                if (filter != null && filter.ValueFilters != null && filter.ValueFilters.Any())
                {
                    oString += " WHERE " + string.Join(" AND ", filter.ValueFilters.Select(x => "`" + x.Field + "`" + " "
                    + GetOperatorAndValueString(objtype.Field.FirstOrDefault(y => y.Name == x.Field), x.Operator, x.Value)));
                }
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"

SELECT 
  `TABLE_SCHEMA`,              
  `TABLE_NAME`,                    
  `COLUMN_NAME`,                
     
  `REFERENCED_TABLE_NAME`,               
  `REFERENCED_COLUMN_NAME`                 
FROM
  `INFORMATION_SCHEMA`.`KEY_COLUMN_USAGE`
WHERE
  `TABLE_SCHEMA` = SCHEMA()                
  AND `REFERENCED_TABLE_NAME` IS NOT NULL;
";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                using (MySqlDataReader oReader = oCmd.ExecuteReader())
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
            //if(idtype == "  int NOT NULL IDENTITY(1,1) ")
            //{
            //    idtype = "  int NOT NULL AUTO_INCREMENT ";
            //}
            //else if(idtype == "  uniqueidentifier NOT NULL DEFAULT NEWID() ")
            //{
            //    idtype = "  char(36) NOT NULL DEFAULT (uuid()) ";
            //}

            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                var pkey = objtype.Field.Where(x => x.Type == "id").Count() > 0 ? objtype.Field.Where(x => x.Type == "id").Select(x => x.Name) : new List<string> { objtype.TableName + "Id" };
                string oString = @"CREATE TABLE `" + objtype.Name + "` (" + string.Join(",", pkey.Select(x => "`" + x + "`" + idtype)) + " , " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => "`" + x.Name + "` " + GetType(x.Type))) + ", PRIMARY KEY (" + string.Join(",", pkey.Select(x => "`" + x + "`")) + "))";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
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
            //    idtype = "  int NOT NULL AUTO_INCREMENT ";
            //    parentidtype = " int ";
            //}
            //else if (idtype == "  uniqueidentifier NOT NULL DEFAULT NEWID() ")
            //{
            //    idtype = "  char(36) NOT NULL DEFAULT (uuid()) ";
            //    parentidtype = " char(36) ";
            //}

            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                if (virtualconnection == false)
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = @"CREATE TABLE `" + objtype.Name + "` (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " + string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) + ", PRIMARY KEY (" + pkey + "), FOREIGN KEY (" + parent.Name + "Id) REFERENCES " + parent.Name + "(" + parent.Field.FirstOrDefault(x => x.Type == "id").Name + "))";
                    MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
                else
                {
                    string pkey = objtype.Field.Where(x => x.Type == "id").Count() == 1 ? objtype.Field.Where(x => x.Type == "id").FirstOrDefault().Name : objtype.TableName + "Id";
                    string oString = @"CREATE TABLE `" + objtype.Name + "` (" + pkey + idtype + " , " + parent.Name + "Id " + parentidtype + ", " +
                        string.Join(",", objtype.Field.Where(x => x.Type != "id").Select(x => x.Name + " " + GetType(x.Type))) +
                        ", PRIMARY KEY (" + pkey + "), )";
                    MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
            return true;
        }

        public bool RemoveColumn(string tablename, string columnname)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE `" + tablename + "` DROP COLUMN `" + columnname + "`";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool AddColumn(string tablename, string columnname, string columntype)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE `" + tablename + "` ADD `" + columnname + "` " + GetType(columntype);
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE `" + tablename + "` RENAME COLUMN " + columnname + " TO " + newcolumnname + "";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool UpdateColumnType(string tablename, string columnname, string columntype)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE `" + tablename + "` MODIFY  `" + columnname + "` " + GetType(columntype);
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

    
        public bool DropTable(string tablename)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"DROP TABLE `" + tablename + "`";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
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
                using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
                {
                    string oString = @"ALTER TABLE `" + childtable + "` ADD CONSTRAINT FK_" + parenttable + "_" + childtable + " FOREIGN KEY (`" + childcolumn + "`) REFERENCES `" + parenttable + "` (`" + parentcolumn + "`)";
                    MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                    myConnection.Open();
                    oCmd.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
            return true;
        }

        public bool RemoveRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"ALTER TABLE `" + childtable + "` DROP CONSTRAINT FK_" + parenttable + "_" + childtable;
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            return true;
        }

        public bool RenameTable(string oldname, string newname)
        {
            using (MySqlConnection myConnection = new MySqlConnection(ConnectionString))
            {
                string oString = @"RENAME TABLE `" + oldname + "` TO `" + newname + "`";
                MySqlCommand oCmd = new MySqlCommand(oString, myConnection);
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
                return "double";
            }
            else if (originaltype == "date")
            {
                return "Date";
            }
            else if (originaltype == "boolean")
            {
                return "tinyint";
            }
            else if (originaltype == "id")
            {
                return "char(36)";
            }
            else if (originaltype == "int")
            {
                return "int";
            }
            else if (originaltype == "char(36)")
            {
                return "char(36)";
            }
            else
            {
                return "varchar(255)";
            }
        }

        public List<Type> GetAllTableApi(List<DatabaseTableRelations> relations,string name)
        {
            throw new NotImplementedException();
        }

        public List<DataObject> GetAllDataFromTableApi(ObjectType objtype, IMemoryCache memoryCache, RootFilter? filter, bool chart = false, bool onlyfirstx = false)
        {
            throw new NotImplementedException();
        }
    }
}
