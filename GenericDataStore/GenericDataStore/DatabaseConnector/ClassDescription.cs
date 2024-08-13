using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GenericDataStore.DatabaseConnector
{
    public class ClassDescription
    {
        public string ClassName { get; set; }
        public List<PropertyDescription> Properties { get; set; }

        public Type Type { get; set; }
        public ClassDescription(Type t)
        {
            Type = t;
            Properties = new List<PropertyDescription>();
            ClassName = t.Name;
            foreach (var property in t.GetProperties())
            {
                bool key = false;
                var attr = property.GetCustomAttributes(true);
                foreach (var a in attr)
                {
                    if (a is GenericDataStore.DatabaseConnector.PrimaryDbKeyAttribute)
                    {
                        key = true;
                    }
                }
                Properties.Add(new PropertyDescription
                {
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType,
                    Key = key
                });
            }

        }

    }

    public class PropertyDescription
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool Key { get; set; }
        public PropertyDescription()
        {
        }
    }
}
