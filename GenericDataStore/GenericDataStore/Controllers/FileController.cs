using GenericDataStore.DatabaseConnector;
using GenericDataStore.InputModels;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public FileController(ILogger<FileController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
        }

        private readonly UserManager<AppUser> userManager;

        protected ILogger<FileController> Logger { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("SaveFile")]
        [Authorize(Policy = "Full")]

        public async Task<IActionResult> SaveFile(string id, string field, Guid table)
        {

            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if(user.AllowedDataCount <= 5000000)
                {
                    return BadRequest("Premium feature");
                }
                var file = this.Request.Form.Files[0];
                var pathToSave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SaveFile").Value;

                if (file.Length > 0)
                {
                    var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == table);
                    type.Field = this.DbContext.Field.Where(x => x.ObjectTypeId == table).ToList();
                    var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == type.DatabaseConnectionPropertyId);
                    Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
                    Dictionary<string,string> iddict = new Dictionary<string, string>();
                    iddict.Add(type.Field.FirstOrDefault(x => x.Type == "id").Name, id);
                    var obj = Repository.GetByDataObjectId(type, iddict);
                    FileService.RemoveFile(obj.Value.Where(x => x.Name == field).ToList());

                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string tempext = fileName.Split('.').LastOrDefault();
                    string tempfilename = fileName.Replace(tempext, string.Empty);
                    tempfilename = tempfilename.Remove(tempfilename.Length - 1);
                    fileName = Path.Combine(table.ToString(), field + id + "." + tempext);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    obj.Value.FirstOrDefault(x => x.Name == field).ValueString = fileName;
                    var value = Repository.UpdateValue(type, obj);

                    return Ok();
                }
                else
                {
                    return BadRequest("No file");
                }
            }
            catch (Exception)
            {
                return BadRequest("error");
            }
        }


        [HttpGet("Download")]
        public async Task<IActionResult> Download(string id)
        {

            var pathToSave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SaveFile").Value;


            var fullPath = Path.Combine(pathToSave, id);


            if (!System.IO.File.Exists(fullPath))
            {
                return Ok();
            }

            var memory = new MemoryStream();
            await using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            var provider = new FileExtensionContentTypeProvider();
            string contentType = string.Empty;

            if (!provider.TryGetContentType(fullPath, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(memory, contentType);
        }


        [HttpPost("Import")]
        [Produces("application/json")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Import(List<JsonElement> file, Guid id, Guid? parent = null)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            // var file = this.Request.Form.Files[0];
            if (file == null)
            {
                return BadRequest("No file");
            }
            if (file.Count == 0)
            {
                return BadRequest("Empty file");
            }
            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);




            var database = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == type.DatabaseConnectionPropertyId);
            var alldatacount = 0;
            var allexternaldatacount = 0;
            var dbids = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == user.Id).ToList();
            foreach (var item in dbids)
            {
                var repo = new Repository(item.ConnectionString, item.DatabaseType);
                var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item.DatabaseConnectionPropertyId).ToList();
                foreach (var item2 in lists)
                {
                    allexternaldatacount += repo.GetCount(item2);
                }

            }

            var internaldatabases = DbContext.DatabaseConnectionProperty.Where(x => x.Default == true).ToList();
            foreach (var item in internaldatabases)
            {
                var repo = new Repository(item.ConnectionString, item.DatabaseType);
                var lists = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == item.DatabaseConnectionPropertyId && x.AppUserId == user.Id).ToList();
                foreach (var item2 in lists)
                {
                    alldatacount += repo.GetCount(item2);

                }
            }
            if (user.AllowedDataCount <= alldatacount + file.Count && database.Default == true)
            {
                return BadRequest("You reached the maximum number of internal row: " + user.AllowedDataCount + ". Increase your limits.");
            }
            if (user.AllowedExternalDataCount <= allexternaldatacount + file.Count && database.Default != true)
            {
                return BadRequest("You reached the maximum number of external row: " + user.AllowedExternalDataCount + ". Increase your limits.");
            }




            var fields = this.DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();
            int optidx = 0;
            foreach (var line in file)
            {
                DataObject dataObject = new DataObject();
                //if (parent != null)
                //{
                //    dataObject.ParentDataObjectId = parent;
                //}
                JsonSerializerOptions jsonoptions = new JsonSerializerOptions()
                {
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode
                };
                var linevalue = line.Deserialize<dynamic>(jsonoptions);

                if (linevalue != null)
                {
                    foreach (var field in fields)
                    {
                        string? stringvalue = linevalue[field.Name]?.ToString();
                        if (stringvalue != null)
                        {
                            if (field.Type == "option")
                            {
                                var options = this.DbContext.Option.Where(x => x.FieldId == field.FieldId).ToList();
                                if (!options.Any(x => x.OptionName == stringvalue.ToString()) && !field.Option.Any(x => x.OptionName == stringvalue.ToString()))
                                {
                                    Option newoption = new Option();
                                    newoption.OptionName = stringvalue.ToString();
                                    newoption.OptionValue = optidx++;
                                    field.Option.Add(newoption);
                                }
                                Value value = new Value();
                                value.Name = field.Name;
                                value.ObjectTypeId = type.ObjectTypeId;
                                value.ValueString = stringvalue.ToString();
                                dataObject.Value.Add(value);
                            }
                            else if (field.Type == "date")
                            {
                                Value value = new Value();
                                value.Name = field.Name;
                                value.ObjectTypeId = type.ObjectTypeId;
                                try
                                {
                                    value.ValueString = DateTime.FromOADate(int.Parse(stringvalue.ToString())).ToString();
                                }
                                catch (Exception)
                                {

                                    value.ValueString = stringvalue.ToString();
                                }
                                dataObject.Value.Add(value);
                            }
                            else if (field.Type == "boolean")
                            {
                                Value value = new Value();
                                value.Name = field.Name;
                                value.ObjectTypeId = type.ObjectTypeId;
                                value.ValueString = stringvalue.ToString();
                                dataObject.Value.Add(value);
                            }
                            else
                            {
                                Value value = new Value();
                                value.Name = field.Name;
                                value.ObjectTypeId = type.ObjectTypeId;
                                value.ValueString = stringvalue.ToString();
                                dataObject.Value.Add(value);
                            }
                        }
                        else
                        {
                            Value value = new Value();
                            value.Name = field.Name;
                            value.ObjectTypeId = type.ObjectTypeId;
                            value.ValueString = null;
                            dataObject.Value.Add(value);
                        }
                    }
                }
                dataObject.ObjectTypeId = type.ObjectTypeId;
                var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == type.DatabaseConnectionPropertyId);
                Repository Repository = new Repository(db.ConnectionString,db.DatabaseType);
                Repository.CreateValue(type, dataObject);
                this.DbContext.SaveChanges();

            }
       
                return this.Ok();

        }

        [HttpPost("ImportCreate")]
        [Produces("application/json")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> ImportCreate(ImportCreateModel modell)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (modell == null)
            {
                return BadRequest("No file");
            }
            if (modell.File.Count == 0)
            {
                return BadRequest("Empty file");
            }
            //datacount

            ObjectType type = modell.ObjectType;
            if (type.Name == "" || type.Name == null)
            {
                return BadRequest("Name is undefinied");
            }
            if (type.DatabaseConnectionPropertyId == null || type.DatabaseConnectionPropertyId == Guid.Empty)
            {
                return BadRequest("Choose a database");
            }
            if (type.Name.Contains("/"))
            {
                return BadRequest("Name can't contain /");
            }
            if (type.Field.GroupBy(x => x.Name).ToList().Any(x => x.Count() > 1))
            {
                return BadRequest("Can't have fields with the same name");
            }
            if (type.Field.Any(x => x.Name == ""))
            {
                return BadRequest("Can't have field with empty name");
            }
            if (this.DbContext.ObjectType.Any(x => x.Name == type.Name && x.ObjectTypeId != type.ObjectTypeId))
            {
                return BadRequest("List name already exist");

            }
            var alllistcount = DbContext.ObjectType.Where(x => x.AppUserId == user.Id).Count();
            if (user.AllowedListCount <= alllistcount)
            {
                return BadRequest("You reached the maximum number of list: " + user.AllowedListCount + ". Increase your limits.");
            }


            type.AppUserId = user.Id;
            type.CreationDate = DateTime.Now;
            type.TableName = type.Name;


            this.DbContext.ObjectType.Add(type);
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == type.DatabaseConnectionPropertyId);

            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            Repository.CreateTable(type, db?.DefaultIdType ?? "");
            if (!type.Field.Any(x => x.Type == "id"))
            {
                type.Field.Add(new Field
                {
                    Name = type.Name + "Id",
                    Type = "id",
                    ObjectTypeId = type.ObjectTypeId
                });
            }
            this.DbContext.SaveChanges();
            return await this.Import(modell.File, type.ObjectTypeId);

        }

    }
}
