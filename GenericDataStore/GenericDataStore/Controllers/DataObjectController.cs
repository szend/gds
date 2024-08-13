using GenericDataStore.DatabaseConnector;
using GenericDataStore.Filtering;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataObjectController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;

        public DataObjectController(ILogger<DataObjectController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
        }

        protected ILogger<DataObjectController> Logger { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        [Authorize(Policy = "Full")]
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] DataObject model)
        {
            var user = await userManager.FindByNameAsync(User?.Identity?.Name);
            var typ = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.ObjectTypeId);
            if (typ == null)
            {
                return BadRequest("Table not found");
            }
            typ.Field = DbContext.Field.Where(x => x.ObjectTypeId == model.ObjectTypeId).ToList();
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == typ.DatabaseConnectionPropertyId);
            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            Repository.UpdateValue(typ,model);

            return this.Ok();
        }
        [Authorize(Policy = "Full")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] DataObject model)
        {
            var user = await userManager.FindByNameAsync(User?.Identity?.Name);
            var typ = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.ObjectTypeId);
            var database = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == typ.DatabaseConnectionPropertyId);
            if (typ == null)
            {
                return BadRequest("Object type not found");
            }
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

            if (user.AllowedDataCount <= alldatacount && database.Default == true)
                {
                    return BadRequest("You reached the maximum number of internal row: " + user.AllowedDataCount + ". Increase your limits.");
                }
                if (user.AllowedExternalDataCount <= allexternaldatacount && database.Default != true)
                {
                    return BadRequest("You reached the maximum number of external row: " + user.AllowedExternalDataCount + ". Increase your limits.");
                }


            typ.Field = DbContext.Field.Where(x => x.ObjectTypeId == model.ObjectTypeId).ToList();
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == typ.DatabaseConnectionPropertyId);
            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            Repository.CreateValue(typ, model);


            return this.Ok();
        }

        [HttpPost("Delete/{typid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Delete(Guid typid, List<KeyValuePair<string,string>> id)
        {
            var typ = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == typid);
            typ.Field = DbContext.Field.Where(x => x.ObjectTypeId == typ.ObjectTypeId).ToList();
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == typ.DatabaseConnectionPropertyId);
            var Repository = new Repository(db.ConnectionString, db.DatabaseType);
            var dict = id.ToDictionary(pair => pair.Key, pair => pair.Value);
            var dataobj = Repository.GetByDataObjectId(typ, dict);
            foreach (var field in typ.Field)
            {
                if (field.Type == "image" || field.Type == "file")
                {

                    FileService.RemoveFile(new List<Value> { dataobj.Value.FirstOrDefault(x => x.Name == field.Name) });
                }
            }
            Repository.DeleteValue(typ, dict);

            return this.Ok();
        }

        [HttpGet("GetAccess")]
        [Authorize(Policy = "Full")]
        public async virtual Task<IActionResult> GetAccess()
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }

            return new JsonResult(user.Id, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
