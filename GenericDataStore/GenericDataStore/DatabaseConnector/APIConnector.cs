using GenericDataStore.Filtering;
using GenericDataStore.Migrations;
using GenericDataStore.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenericDataStore.DatabaseConnector
{
    public class APIConnector : IDataConnector
    {
        public string ConnectionString { get; set; }

        public APIConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool AddChild(ObjectType objtype, ObjectType parent, string idtype, string parentidtype, bool virtualconnection = false)
        {
            throw new NotImplementedException();
        }

        public bool AddColumn(string tablename, string columnname, string columntype)
        {
            throw new NotImplementedException();
        }

        public List<string> CreateClass(string json, List<DatabaseTableRelations> relations, string name)
        {
            return GenerateClassesFromJson(json, name, relations);
        }

        public List<string> GenerateClassesFromJson(string json, string className, List<DatabaseTableRelations> relations)
        {
            var jobject = JToken.Parse(json);
            var classDefinitions = new List<string>();
            if (jobject.Type == JTokenType.Array)
            {
                var itemtype = className;
                var nestedClassDefinition = GenerateClassFromJArray((JArray)jobject, itemtype, classDefinitions, relations);
            }
            else
            {
                classDefinitions.Add(GenerateClassFromJObject((JObject)jobject, className, classDefinitions, relations));
            }
            return classDefinitions;
        }

        private string GenerateClassFromJArray(JArray jArray, string className, List<string> classDefinitions, List<DatabaseTableRelations> relations)
        {
            if (jArray.Count > 0)
            { var firstElementType = GetCSharpType(jArray.First.Type);
                var properties = new List<string>();
                foreach (var item in jArray)
                {
                    if (item.Type == JTokenType.Object) {
                        var nestedClassDefinition = GenerateClassFromJObject((JObject)item, className, classDefinitions, relations);
                        classDefinitions.Add(nestedClassDefinition); break;
                    }
                }

            } return string.Empty;
        }


        public string GenerateClassFromJObject(JObject jobject, string className, List<string> classDefinitions, List<DatabaseTableRelations> relations)
        {
            var properties = new List<string>();

            foreach (var property in jobject.Properties())
            {
                string type;

                if (property.Value.Type == JTokenType.Array)
                {
                    var itemType = GetCSharpType(property.Value.First.Type);
                    if (itemType == "object")
                    {
                        type = property.Name; // Alosztály generálás
                        var nestedClassDefinition = GenerateClassFromJArray((JArray)property.Value, type, classDefinitions, relations);
                        relations.Add(new DatabaseTableRelations
                        {
                            ParentTable = className,
                            ParentPropertyName = "CalculatedRowIndex",
                            ChildTable = type,
                            ChildPropertyName = "CalculatedRowIndex",
                            Virtual = true,
                            FKName = "FK_" + className + "_" + type,

                        });
                        classDefinitions.Add(nestedClassDefinition);
                    }
                    type = itemType + "[]";
                }
                else if (property.Value.Type == JTokenType.Object)
                {
                    type = property.Name; // Alosztály generálás
                    var nestedClassDefinition = GenerateClassFromJObject((JObject)property.Value, type, classDefinitions, relations);
                    relations.Add(new DatabaseTableRelations
                    {
                        ParentTable = className,
                        ParentPropertyName = "CalculatedRowIndex",
                        ChildTable = type,
                        ChildPropertyName = "CalculatedRowIndex",
                        Virtual = true,
                        FKName = "FK_" + className + "_" + type
                    });
                    classDefinitions.Add(nestedClassDefinition);
                }
                else
                {
                    type = GetCSharpType(property.Value.Type);
                    properties.Add($"  [JsonProperty(\"{ property.Name}\")]  public {type} {ClassTypeBuilder.MakePropertyNameCorrect(property.Name)} {{ get; set; }}");
                }

            }

            return $"public class {className}\n{{\n{string.Join("\n", properties)}\n}}";
        }


        private static string GetCSharpType(JTokenType tokenType)
        {
            return tokenType switch
            {
                JTokenType.String => "string",
                JTokenType.Date => "DateTime",
                JTokenType.Integer => "double",
                JTokenType.Float => "double",
                JTokenType.Boolean => "bool",
                JTokenType.Null => "object",
                _ => "object"
            };
        }

        public bool CreateRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn, string idtype, bool virtualrelation = false)
        {
            throw new NotImplementedException();
        }

        public bool CreateTable(ObjectType objtype, string idtype)
        {
            throw new NotImplementedException();
        }

        public bool DeleteValue(string tablename, Dictionary<string, string> ids)
        {
            throw new NotImplementedException();
        }

        public bool DropTable(string tablename)
        {
            throw new NotImplementedException();
        }

        public string ExecuteQuery(string query)
        {
            throw new NotImplementedException();
        }
        public List<string> GetNodesByName(string jsonString, string nodeName, int parentIndex)
        {
            var result = new List<string>();
            try 
            { 
                var jsonObject = JObject.Parse(jsonString);
                FindNodes(jsonObject, nodeName, parentIndex, result);
            } 
            catch (Exception ex)
            {
                return new List<string>();
            }
            return result;
        }
        public List<string> GetNodesByName(string jsonString, string nodeName)
        {
            var result = new List<string>();

            try
            {
                var jsonObject = JToken.Parse(jsonString);
                FindNodes(jsonObject, nodeName, result);
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }

        private void FindNodes(JToken token, string nodeName, int parentIndex, List<string> result, int currentIndex = 0)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {  
                    if (currentIndex == parentIndex && property.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(property.Value.ToString()); 
                    }
                    FindNodes(property.Value, nodeName, parentIndex, result, currentIndex);
                }
            } 
            else if (token.Type == JTokenType.Array)
            { 
                foreach (var item in token.Children()) 
                {
                    FindNodes(item, nodeName, parentIndex, result, currentIndex);
                    currentIndex++;  
                }
            }
        }

        private void FindNodes(JToken token, string nodeName, List<string> result)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(property.Value.ToString());
                    }
                    FindNodes(property.Value, nodeName, result);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    FindNodes(item, nodeName, result);
                }
            }
        }
        public int? GetNodeIndexByReference(string jsonString, string nodeName, JToken referenceNode) 
        {
            try
            {
                var jsonObject = JObject.Parse(jsonString);
                return FindNodeIndexByReference(jsonObject, nodeName, referenceNode);
            } 
            catch (Exception ex)
            {
                return null;
            }
        }
        private int? FindNodeIndexByReference(JToken token, string nodeName, JToken referenceNode)
        {
            if (token.Type == JTokenType.Object)
            {
                int index = 0; foreach (var property in token.Children<JProperty>())
                { 
                    if (property.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (property.Value.ToString() == referenceNode.ToString())
                        {
                            return index;
                             }
                    }
                    var foundIndex = FindNodeIndexByReference(property.Value, nodeName, referenceNode);
                    if (foundIndex.HasValue)
                    {
                        return foundIndex;
                    } index++;
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                int index = 0;
                foreach (var item in token.Children())
                {
                    var foundIndex = FindNodeIndexByReference(item, nodeName, referenceNode);
                    if (foundIndex.HasValue)
                    {
                        return foundIndex;
                    }
                    index++;
                }
            }
            return null;
        } 



         public List<DataObject> GetAllDataFromTableApi(ObjectType objtype, IMemoryCache memoryCache, RootFilter? filter, bool chart = false, bool onlyfirstx = false)
        {
            List<DataObject>? dataObjects;
            if(filter.ValueFilters.FirstOrDefault(x => x.Field == "CalculatedRowIndex") != null)
            {
                dataObjects = new List<DataObject>();
                var data = GetStringData();
                if (data != null)
                {
                    List<DatabaseTableRelations> relations = new List<DatabaseTableRelations>();
                    var resultClassString = CreateClass(data, relations, "Root");
                    resultClassString = resultClassString.Where(x => x != string.Empty).ToList();
                    var result = resultClassString.Where(x => GetClassNameFromString(x) == objtype.Name).FirstOrDefault();
                    var res = ClassTypeBuilder.Create(result, objtype.Name);
                    int parentrow = int.Parse(filter.ValueFilters.FirstOrDefault(x => x.Field == "CalculatedRowIndex")?.Value.ToString());

                    List<string> childnodelist  = GetNodesByName(data, objtype.Name, parentrow);
                    filter.ValueFilters.Remove(filter.ValueFilters.FirstOrDefault(x => x.Field == "CalculatedRowIndex"));
                    List<string> nodelist = GetNodesByName(data, objtype.Name);


                    if (childnodelist.Count > 0)
                    {
                        int startindex = 0;


                        foreach (var node in childnodelist)
                        {
                            var jobjectnode = JToken.Parse(node);

                            if (jobjectnode.Type == JTokenType.Array)
                            {
                                if (node != null)
                                {
                                    var datalist = JsonConvert.DeserializeObject<List<object>>(node);
                                    foreach (var item in datalist)
                                    {

                                        if (item != null)
                                        {
                                            var root = (item as JObject).Root.ToString();


                                            var dobj = JsonConvert.DeserializeObject(root, res);
                                            DataObject dataObject = new DataObject();
                                            foreach (var field in objtype.Field)
                                            {
                                                var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                                dataObject.Value.Add(new Value
                                                {
                                                    Name = field.Name,
                                                    ValueString = value?.ToString(),
                                                    ObjectTypeId = objtype.ObjectTypeId,

                                                });
                                            }
                                            var valueindex = 0;
                                            for (int i = nodelist.IndexOf(node); i > 0; i--)
                                            {
                                                valueindex += GetNodesByName(data, objtype.Name, i).Count;
                                            }
                                            dataObject.Value.Add(new Value()
                                            {
                                                Name = "CalculatedRowIndex",
                                                ValueString = (datalist.IndexOf(item) + valueindex).ToString(),
                                                ObjectTypeId = objtype.ObjectTypeId,
                                            });

                                            dataObjects.Add(dataObject);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (node != null)
                                {
                                    var item = JsonConvert.DeserializeObject<object>(node);
                                    if (item != null)
                                    {
                                        var root = (item as JObject).Root.ToString();
                                        var dobj = JsonConvert.DeserializeObject(root, res);
                                        DataObject dataObject = new DataObject();
                                        foreach (var field in objtype.Field)
                                        {
                                            var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                            dataObject.Value.Add(new Value
                                            {
                                                Name = field.Name,
                                                ValueString = value?.ToString(),
                                                ObjectTypeId = objtype.ObjectTypeId,

                                            });
                                        }
                                        dataObject.Value.Add(new Value()
                                        {
                                            Name = "CalculatedRowIndex",
                                            ValueString = nodelist.IndexOf(node).ToString(),
                                            ObjectTypeId = objtype.ObjectTypeId,
                                        });
                                        dataObjects.Add(dataObject);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        var jobject = JToken.Parse(data);

                        if (jobject.Type == JTokenType.Array)
                        {
                            if (data != null)
                            {
                                var datalist = JsonConvert.DeserializeObject<List<object>>(data);
                                foreach (var item in datalist)
                                {

                                    if (item != null)
                                    {
                                        var root = (item as JObject).Root.ToString();
                                        var dobj = JsonConvert.DeserializeObject(root, res);
                                        DataObject dataObject = new DataObject();
                                        foreach (var field in objtype.Field)
                                        {
                                            var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                            dataObject.Value.Add(new Value
                                            {
                                                Name = field.Name,
                                                ValueString = value?.ToString(),
                                                ObjectTypeId = objtype.ObjectTypeId,

                                            });
                                        }
                                        dataObjects.Add(dataObject);
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (data != null)
                            {
                                var item = JsonConvert.DeserializeObject<object>(data);
                                if (item != null)
                                {
                                    var root = (item as JObject).Root.ToString();
                                    var dobj = JsonConvert.DeserializeObject(root, res);
                                    DataObject dataObject = new DataObject();
                                    foreach (var field in objtype.Field)
                                    {
                                        var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                        dataObject.Value.Add(new Value
                                        {
                                            Name = field.Name,
                                            ValueString = value?.ToString(),
                                            ObjectTypeId = objtype.ObjectTypeId,

                                        });
                                    }
                                    dataObjects.Add(dataObject);
                                }
                            }
                        }

                    }

                }


                if (filter.ValueFilters.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueFilter(dataObjects, filter);
                }

                objtype.Count = dataObjects.Count;


                if (filter.ValueSortingParams.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueSort(dataObjects, filter);
                }
                if (filter.ValueSkip > 0)
                {
                    dataObjects = dataObjects.Skip(filter.ValueSkip).ToList();
                }
                if (filter.ValueTake > 0)
                {
                    dataObjects = dataObjects.Take(filter.ValueTake).ToList();
                }


            }
            else if (!memoryCache.TryGetValue(objtype.ObjectTypeId, out dataObjects))
            {
                dataObjects = new List<DataObject>();
                var data = GetStringData();
                if (data != null)
                {
                    List<DatabaseTableRelations> relations = new List<DatabaseTableRelations>();
                    var resultClassString = CreateClass(data, relations, objtype.Name);
                    resultClassString = resultClassString.Where(x => x != string.Empty).ToList();
                    var result = resultClassString.Where(x => GetClassNameFromString(x) == objtype.Name).FirstOrDefault();
                    var res = ClassTypeBuilder.Create(result, objtype.Name);
                    List<string> nodelist = GetNodesByName(data, objtype.Name);
                    

                    if (nodelist.Count > 0)
                    {
                        foreach (var node in nodelist)
                        {
                            var jobjectnode = JToken.Parse(node);

                            if (jobjectnode.Type == JTokenType.Array)
                            {
                                if (node != null)
                                {
                                    var datalist = JsonConvert.DeserializeObject<List<object>>(node);
                                    foreach (var item in datalist)
                                    {

                                        if (item != null)
                                        {
                                            var root = (item as JObject).Root.ToString();
  

                                            var dobj = JsonConvert.DeserializeObject(root, res);
                                            DataObject dataObject = new DataObject();
                                            foreach (var field in objtype.Field)
                                            {
                                                var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                                dataObject.Value.Add(new Value
                                                {
                                                    Name = field.Name,
                                                    ValueString = value?.ToString(),
                                                    ObjectTypeId = objtype.ObjectTypeId,

                                                });
                                            }
                                            dataObjects.Add(dataObject);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (node != null)
                                {
                                    var item = JsonConvert.DeserializeObject<object>(node);
                                    if (item != null)
                                    {
                                        var root = (item as JObject).Root.ToString();
                                        var dobj = JsonConvert.DeserializeObject(root, res);
                                        DataObject dataObject = new DataObject();
                                        foreach (var field in objtype.Field)
                                        {
                                            var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                            dataObject.Value.Add(new Value
                                            {
                                                Name = field.Name,
                                                ValueString = value?.ToString(),
                                                ObjectTypeId = objtype.ObjectTypeId,

                                            });
                                        }
                                        dataObjects.Add(dataObject);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        var jobject = JToken.Parse(data);

                        if (jobject.Type == JTokenType.Array)
                        {
                            if (data != null)
                            {
                                var datalist = JsonConvert.DeserializeObject<List<object>>(data);
                                foreach (var item in datalist)
                                {

                                    if (item != null)
                                    {
                                        var root = (item as JObject).Root.ToString();
                                        var dobj = JsonConvert.DeserializeObject(root, res);
                                        DataObject dataObject = new DataObject();
                                        foreach (var field in objtype.Field)
                                        {
                                            var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                            dataObject.Value.Add(new Value
                                            {
                                                Name = field.Name,
                                                ValueString = value?.ToString(),
                                                ObjectTypeId = objtype.ObjectTypeId,

                                            });
                                        }
                                        dataObjects.Add(dataObject);
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (data != null)
                            {
                                var item = JsonConvert.DeserializeObject<object>(data);
                                if (item != null)
                                {
                                    var root = (item as JObject).Root.ToString();
                                    var dobj = JsonConvert.DeserializeObject(root, res);
                                    DataObject dataObject = new DataObject();
                                    foreach (var field in objtype.Field)
                                    {
                                        var value = dobj?.GetType().GetProperties().FirstOrDefault(x => ClassTypeBuilder.MakePropertyNameCorrect(x.Name) == field.Name)?.GetValue(dobj);
                                        dataObject.Value.Add(new Value
                                        {
                                            Name = field.Name,
                                            ValueString = value?.ToString(),
                                            ObjectTypeId = objtype.ObjectTypeId,

                                        });
                                    }
                                    dataObjects.Add(dataObject);
                                }
                            }
                        }

                    }

                }

                int idx = 0;
                foreach (var item in dataObjects)
                {
                    item.Value.Add(new Value()
                    {
                        Name = "CalculatedRowIndex",
                        ValueString = idx.ToString(),
                        ObjectTypeId = objtype.ObjectTypeId,
                    });
                    idx++;
                }

                memoryCache.Set(objtype.ObjectTypeId, dataObjects,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));



                if (filter.ValueFilters.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueFilter(dataObjects, filter);
                }

                objtype.Count = dataObjects.Count;


                if (filter.ValueSortingParams.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueSort(dataObjects, filter);
                }
                if (filter.ValueSkip > 0)
                {
                    dataObjects = dataObjects.Skip(filter.ValueSkip).ToList();
                }
                if (filter.ValueTake > 0)
                {
                    dataObjects = dataObjects.Take(filter.ValueTake).ToList();
                }
            }
            else
            {

                if (filter.ValueFilters.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueFilter(dataObjects, filter);
                }
                objtype.Count = dataObjects.Count;
                if (filter.ValueSortingParams.Count > 0)
                {
                    dataObjects = CreateQuery<DataObject>.ValueSort(dataObjects, filter);
                }
                if (filter.ValueSkip > 0)
                {
                    dataObjects = dataObjects.Skip(filter.ValueSkip).ToList();
                }
                if (filter.ValueTake > 0)
                {
                    dataObjects = dataObjects.Take(filter.ValueTake).ToList();
                }
            }

            objtype.Field.Add(new Field()
            {
                Name = "CalculatedRowIndex",
                Type = "id",
                ObjectTypeId = objtype.ObjectTypeId,
                Position = objtype.Field.Count + 1,


            });

            return dataObjects;
        }

        public int GetCount(ObjectType objtype, RootFilter? filter = null)
        {
            var data = GetStringData();
            var nodelist = GetNodesByName(data, objtype.Name);
            if (nodelist.Count > 0)
            {
                foreach (var node in nodelist)
                {
                    var jobjectnode = JToken.Parse(node);
                    if (jobjectnode.Type == JTokenType.Array)
                    {
                        var datalist = JsonConvert.DeserializeObject<List<object>>(node);
                        return datalist.Count;
                    }
                    else
                    {
                        return 1;
                    }
                }

            }
            else
            {
                var jobject = JToken.Parse(data);

                if (jobject.Type == JTokenType.Array)
                {
                    if (data != null)
                    {
                        var dataObject = JsonConvert.DeserializeObject<List<object>>(data);
                        return dataObject.Count;
                    }
                }
                else
                {
                    if (data != null)
                    {
                        return 1;
                    }
                }
            }


            return 0;
        }

        string GetStringData()
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(ConnectionString).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return data;
            }
            return null;
        }

        public List<Type> GetAllTableApi(List<DatabaseTableRelations> relations, string name)
        {

            var data = GetStringData();
            if (data != null)
            {
                List<Type> types = new List<Type>();
                name = name.Replace(" ", "_");
                var resultClassString = CreateClass(data, relations, name);
                foreach (var result in resultClassString)
                {
                    if (result != string.Empty)
                    {
                        var res = ClassTypeBuilder.Create(result, GetClassNameFromString(result));
                        if (res != null)
                        {
                            types.Add(res);
                        }
                    }

                }

                return types;
            }
            return null;
        }

        string GetClassNameFromString(string classdescription)
        {
            var start = classdescription.IndexOf("class") + 5;
            var end = classdescription.IndexOf("{");
            return classdescription.Substring(start, end - start).Trim();

        }

        public List<string> GetAllTableName()
        {
            throw new NotImplementedException();
        }

        public DataObject GetByDataObjectId(string tablename, Dictionary<string, string> keyValuePairs)
        {
            throw new NotImplementedException();
        }

        public List<DatabaseTableRelations> GetConnections()
        {
            throw new NotImplementedException();
        }


        public bool InsertVlaues(string tablename, Dictionary<string, string> fieldvalues)
        {
            throw new NotImplementedException();
        }

        public bool RemoveColumn(string tablename, string columnname)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRealtion(string parenttable, string parentcolumn, string childtable, string childcolumn)
        {
            throw new NotImplementedException();
        }

        public bool RenameTable(string oldname, string newname)
        {
            throw new NotImplementedException();
        }

        public bool UpdateColumn(string tablename, string columnname, string newcolumnname, string columntype)
        {
            throw new NotImplementedException();
        }

        public bool UpdateColumnType(string tablename, string columnname, string columntype)
        {
            return true;
        }

        public bool UpdateValues(string tablename, Dictionary<string, string> fieldvalues, Dictionary<string, string> ids)
        {
            throw new NotImplementedException();
        }

        public List<Type> GetAllTable(List<string> tablesname)
        {
            throw new NotImplementedException();
        }

        public List<DataObject> GetAllDataFromTable(ObjectType objtype, RootFilter? filter, bool chart = false, bool onlyfirstx = false)
        {
            throw new NotImplementedException();
        }
    }
}
