using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace GenericDataStore.DatabaseConnector
{
    public static class ClassTypeBuilder
    {
        public static Type Create(string classstring, string name)
        {
            string codeToCompile = "using Newtonsoft.Json;using System; using GenericDataStore.DatabaseConnector; namespace GenericDataStoreGenerated { " + classstring + " }";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
                string assemblyName = Path.GetRandomFileName();
                var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Newtonsoft.Json.JsonConvert).GetTypeInfo().Assembly.Location,
                typeof(PrimaryDbKeyAttribute).GetTypeInfo().Assembly.Location,
                typeof(FieldNameAttribute).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };
                MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
                CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);

                    if (!result.Success)
                    {
                        throw new Exception(classstring);

                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                        Type myTableType = assembly.GetType("GenericDataStoreGenerated." + name);
                        // var finalResult = Activator.CreateInstance(myTableType);
                        return myTableType;
                    }
                }            
            


        }
        public static string MakePropertyNameCorrect(string propertyname)
        {
            if (propertyname.StartsWith("0") || propertyname.StartsWith("1") || propertyname.StartsWith("2")
                || propertyname.StartsWith("3") || propertyname.StartsWith("4") || propertyname.StartsWith("5")
                || propertyname.StartsWith("6") || propertyname.StartsWith("7") || propertyname.StartsWith("8")
                || propertyname.StartsWith("9"))
            {
                propertyname = "_" + propertyname;
            }
            if (propertyname.Contains(" "))
            {
                propertyname = propertyname.Replace(" ", "_");
            }
            if (propertyname.Contains(" "))
            {
                propertyname = propertyname.Replace(" ", "_");
            }
            if (propertyname.Contains("-"))
            {
                propertyname = propertyname.Replace("-", "_");
            }
            if (propertyname.Contains("."))
            {
                propertyname = propertyname.Replace(".", "_");
            }
            if (propertyname.Contains("["))
            {
                propertyname = propertyname.Replace("[", "_").Replace("]", "_");
            }
            if (propertyname.Contains("("))
            {
                propertyname = propertyname.Replace("(", "_").Replace(")", "_");
            }
            if (propertyname.Contains("]"))
            {
                propertyname = propertyname.Replace("]", "_");
            }
            if (propertyname.Contains(")"))
            {
                propertyname = propertyname.Replace(")", "_");
            }
            if (propertyname.Contains(","))
            {
                propertyname = propertyname.Replace(",", "_");
            }
            if (propertyname.Contains(":"))
            {
                propertyname = propertyname.Replace(":", "_");
            }
            if (propertyname.Contains(";"))
            {
                propertyname = propertyname.Replace(";", "_");
            }
            if (propertyname.Contains("<"))
            {
                propertyname = propertyname.Replace("<", "_");
            }
            if (propertyname.Contains(">"))
            {
                propertyname = propertyname.Replace(">", "_");
            }
            if (propertyname.Contains("="))
            {
                propertyname = propertyname.Replace("=", "_");
            }
            if (propertyname.Contains("+"))
            {
                propertyname = propertyname.Replace("+", "_");
            }
            if (propertyname.Contains("*"))
            {
                propertyname = propertyname.Replace("*", "_");
            }
            if (propertyname.Contains("/"))
            {
                propertyname = propertyname.Replace("/", "_");
            }
            if (propertyname.Contains("%"))
            {
                propertyname = propertyname.Replace("%", "_");
            }
            if (propertyname.Contains("&"))
            {
                propertyname = propertyname.Replace("&", "_");
            }
            if (propertyname.Contains("|"))
            {
                propertyname = propertyname.Replace("|", "_");
            }
            if (propertyname.Contains("^"))
            {
                propertyname = propertyname.Replace("^", "_");
            }
            if (propertyname.Contains("~"))
            {
                propertyname = propertyname.Replace("~", "_");
            }


            return propertyname;
        }

    }

    public class PrimaryDbKeyAttribute : System.Attribute
        {
            public PrimaryDbKeyAttribute()
            {


            }
        }

    public class FieldNameAttribute : System.Attribute
    {
        public string Name { get; set; }
        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }


}



