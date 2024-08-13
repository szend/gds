using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using System.Net.NetworkInformation;
//using DatabaseCreatorAttributes;

namespace GenericDataStore.DatabaseConnector
{
    public static class ClassTypeBuilder
    {
        public static Type Create(string classstring, string name)
        {
            try
            {
                string codeToCompile = "using System; using GenericDataStore.DatabaseConnector; namespace GenericDataStoreGenerated { " + classstring + " }";
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
                string assemblyName = Path.GetRandomFileName();
                var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(PrimaryDbKeyAttribute).GetTypeInfo().Assembly.Location,
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
                        return null;

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
            catch (Exception)
            {

                return null;
            }

        }
    
    }

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



