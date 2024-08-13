using GenericDataStore.Models;

namespace GenericDataStore.Services
{
    public class FileService
    {
        public static void RemoveFile(List<Value> value)
        {
            var pathToSave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SaveFile").Value;

            foreach (var item in value)
            {
                if(item.ValueString != null && item.ValueString != "")
                {
                    var fullPath = Path.Combine(pathToSave, item?.ValueString);
                    FileInfo fileinfo = new FileInfo(fullPath);
                    if (fileinfo.Exists)
                    {
                        fileinfo.Delete();
                    }
                }

            }
        }
        private static long GetDirectorySize(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }
    }
}
