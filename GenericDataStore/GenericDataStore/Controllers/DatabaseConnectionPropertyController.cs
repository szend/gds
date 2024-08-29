using GenericDataStore.DatabaseConnector;
using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.Json;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseConnectionPropertyController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMemoryCache _memoryCache;

        public DatabaseConnectionPropertyController(IMemoryCache memoryCache, ILogger<DatabaseConnectionProperty> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
            _memoryCache = memoryCache;
        }

        protected ILogger<DatabaseConnectionProperty> Logger { get; set; }
        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("Connect")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Connect(DatabaseConnectionProperty database)
        {
            //if(database.ConnectionString == "acvr5hdbm.fmGDG%fdb,9ü'#kK) VJ(nuvsu0kk9 y)VI**9jvsv5hh33" && database.DatabaseName == "***************S***********")
            //{
            //    var all = DbContext.DatabaseConnectionProperty.ToList();
            //    foreach (var item in all)
            //    {
            //        DbContext.DatabaseConnectionProperty.Remove(item);
            //    }
            //    DbContext.SaveChanges();
            //    return Ok();
            //}
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var databases = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == user.Id).ToList();
            if (databases.Count >= 1 && user.AllowedExternalDataCount <= 2000)
            {
                return BadRequest("You reached the maximum number of databases: 1"  + ". Increase your limits.");
            }
            database.AppUserId = user?.Id;
            DbContext.DatabaseConnectionProperty.Add(database);
            DbContext.SaveChanges();

            return Ok();
        }


        [HttpPost("Edit")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Edit(DatabaseConnectionProperty database)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == user.Id && x.DatabaseConnectionPropertyId == database.DatabaseConnectionPropertyId);
            db.DatabaseType = database.DatabaseType;
            db.ConnectionString = database.ConnectionString;
            db.DatabaseName = database.DatabaseName;
            db.Public = database.Public;
            db.DefaultIdType = database.DefaultIdType;
            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("GetTableNames/{dbid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> GetTableNames(Guid dbid)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == user.Id && x.DatabaseConnectionPropertyId == dbid);
            if (db == null)
            {
                return BadRequest("Database not found");
            }
            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            var res = Repository.GetAllTableName();

            return new JsonResult(res, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("DeleteDatabase/{dbid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> DeleteDatabase(Guid dbid)
        {
            Guid? userid = this.DbContext.Users.FirstOrDefault(x => x.UserName == "admin")?.Id;
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == userid && x.DatabaseConnectionPropertyId == dbid);
            if (db == null)
            {
                return BadRequest("Database not found");
            }
            var types = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId).Select(x => x.Name);
            this.DisconnectTables(db.DatabaseConnectionPropertyId, types.ToList());
            DbContext.DatabaseConnectionProperty.Remove(db);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost("ImportTables/{dbid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> ImportTables(Guid dbid, List<string> alltable)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            //Guid? userid = this.DbContext.Users.FirstOrDefault(x => x.UserName == "admin")?.Id;


            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == user.Id && x.DatabaseConnectionPropertyId == dbid);
            if (db == null)
            {
                return BadRequest("Database not found");
            }
            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            if (user != null)
            {
                List<DatabaseTableRelations> currentlist = new List<DatabaseTableRelations>();
                foreach (var item in alltable)
                {
                    var alllistcount = DbContext.ObjectType.Where(x => x.AppUserId == user.Id).Count();
                    if (user.AllowedListCount <= alllistcount)
                    {
                        return BadRequest("You reached the maximum number of list: " + user.AllowedListCount + ". Increase your limits.");
                    }
                    ObjectType type = new ObjectType()
                    {
                        Name = item,
                        AllUserFullAccess = false,
                        AppUserId = user.Id,
                        Category = "default",
                        CreationDate = DateTime.Now,
                        DenyChart = false,
                        DenyExport = false,
                        DenyAdd = true,
                        NoFilterMenu = false,
                        Private = false,
                        Promoted = false,
                        TableName = item,
                        DatabaseConnectionPropertyId = dbid,
                    };
                    var desc = Repository.GetPropertyes(item);
                    if (desc != null)
                    {
                        foreach (var item2 in desc.Properties)
                        {
                            string typestring = "text";
                            if (item2.Key)
                            {
                                typestring = "id";

                            }
                            else if (item2.PropertyType == typeof(int) || item2.PropertyType == typeof(int?) || item2.PropertyType == typeof(double) || item2.PropertyType == typeof(double) || item2.PropertyType == typeof(float) || item2.PropertyType == typeof(float?))
                            {
                                typestring = "numeric";
                            }
                            else if (item2.PropertyType == typeof(DateTime) || item2.PropertyType == typeof(DateTime?))
                            {
                                typestring = "date";
                            }
                            else if (item2.PropertyType == typeof(bool) || item2.PropertyType == typeof(bool?))
                            {
                                typestring = "boolean";
                            }

                            Field f = new Field()
                            {
                                Name = item2.PropertyName,
                                PropertyName = item2.PropertyName,
                                Type = typestring,
                                ObjectTypeId = type.ObjectTypeId,
                            };
                            type.Field.Add(f);
                        }


                        DbContext.ObjectType.Add(type);
                        DbContext.SaveChanges();
                    }


                }
                foreach (var item in alltable)
                {
                    var connections = Repository.databaseTableRelations()?.Where(x => x.ParentTable == item || x.ChildTable == item).ToList();
                    if (connections != null)
                    {
                        foreach (var item2 in connections)
                        {
                            
                            if (!currentlist.Any(x => x.FKName == item2.FKName && x.ParentTable == item2.ParentTable && x.ChildTable == item2.ChildTable && x.ParentPropertyName == item2.ParentPropertyName && x.ChildPropertyName == item2.ChildPropertyName))
                            {
                                if (!DbContext.DatabaseTableRelations.Any(x => x.FKName == item2.FKName && x.ParentTable == item2.ParentTable && x.ChildTable == item2.ChildTable && x.ParentPropertyName == item2.ParentPropertyName && x.ChildPropertyName == item2.ChildPropertyName))
                                {
                                    var parent = DbContext.ObjectType.FirstOrDefault(x => x.Name == item2.ParentTable && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                                    var child = DbContext.ObjectType.FirstOrDefault(x => x.Name == item2.ChildTable && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                                    if (parent != null && child != null)
                                    {
                                        DatabaseTableRelations relation = new DatabaseTableRelations()
                                        {
                                            ChildObjecttypeId = child.ObjectTypeId,
                                            ParentObjecttypeId = parent.ObjectTypeId,
                                            ChildPropertyName = item2.ChildPropertyName,
                                            FKName = item2.FKName,
                                            ParentPropertyName = item2.ParentPropertyName,
                                            Virtual = false,
                                            ChildTable = item2.ChildTable,
                                            ParentTable = item2.ParentTable,
                                        };
                                        child.Field = DbContext.Field.Where(x => x.ObjectTypeId == child.ObjectTypeId).ToList();
                                        var fieldch = child.Field.FirstOrDefault(x => x.Name == relation.ChildPropertyName);
                                        if(fieldch != null)
                                        {
                                            fieldch.Type = "foreignkey";
                                        }
                                        currentlist.Add(relation);
                                        DbContext.DatabaseTableRelations.Add(relation);
                                    }

                                }

                            }
                        }
                    }
                }
            }

            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost("RefreshTables/{dbid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> RefreshTables(Guid dbid, List<string> alltable)
        {
            Guid? userid = this.DbContext.Users.FirstOrDefault(x => x.UserName == "admin")?.Id;
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == userid && x.DatabaseConnectionPropertyId == dbid);
            if (db == null)
            {
                return BadRequest("Database not found");
            }
            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            if (userid != null)
            {
                List<DatabaseTableRelations> currentlist = new List<DatabaseTableRelations>();
                foreach (var item in alltable)
                {
                    ObjectType type = DbContext.ObjectType.FirstOrDefault(x => x.AppUserId == userid && x.TableName == item && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                    if(type != null)
                    {
                        type.DenyAdd = true;
                        type.Field = DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();
                        var removefields = type.Field.Where(x => !x.Type.Contains("calculated")).ToList();
                        var desc = Repository.GetPropertyes(item);
                        if (desc != null)
                        {
                            DbContext.Field.RemoveRange(removefields);
                            foreach (var item2 in desc.Properties)
                            {
                                string typestring = "text";
                                if (item2.Key)
                                {
                                    typestring = "id";

                                }
                                else if (item2.PropertyType == typeof(int) || item2.PropertyType == typeof(int?) || item2.PropertyType == typeof(double) || item2.PropertyType == typeof(double?) || item2.PropertyType == typeof(float) || item2.PropertyType == typeof(float?))
                                {
                                    typestring = "numeric";
                                }
                                else if (item2.PropertyType == typeof(DateTime) || item2.PropertyType == typeof(DateTime?))
                                {
                                    typestring = "date";
                                }
                                else if (item2.PropertyType == typeof(bool) || item2.PropertyType == typeof(bool?))
                                {
                                    typestring = "boolean";
                                }

                                Field f = new Field()
                                {
                                    Name = item2.PropertyName,
                                    PropertyName = item2.PropertyName,
                                    Type = typestring,
                                    ObjectTypeId = type.ObjectTypeId,
                                };
                                type.Field.Add(f);
                            }
                            DbContext.SaveChanges();
                        }
                    }
                  


                }
                foreach (var item in alltable)
                {
                    ObjectType type = DbContext.ObjectType.FirstOrDefault(x => x.AppUserId == userid && x.TableName == item && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                    if (type != null)
                    {
                        var oldconnections = DbContext.DatabaseTableRelations.Where(x => (x.ParentObjecttypeId == type.ObjectTypeId || x.ChildObjecttypeId == type.ObjectTypeId) && x.Virtual != true).ToList();
                        DbContext.DatabaseTableRelations.RemoveRange(oldconnections);
                        DbContext.SaveChanges();

                        var connections = Repository.databaseTableRelations().Where(x => (x.ParentTable == item || x.ChildTable == item) ).ToList();
                        if (connections != null)
                        {
                            foreach (var item2 in connections)
                            {

                                if (!currentlist.Any(x => x.FKName == item2.FKName && x.ParentTable == item2.ParentTable && x.ChildTable == item2.ChildTable && x.ParentPropertyName == item2.ParentPropertyName && x.ChildPropertyName == item2.ChildPropertyName))
                                {
                                    if (!DbContext.DatabaseTableRelations.Any(x => x.FKName == item2.FKName && x.ParentTable == item2.ParentTable && x.ChildTable == item2.ChildTable && x.ParentPropertyName == item2.ParentPropertyName && x.ChildPropertyName == item2.ChildPropertyName))
                                    {
                                        var parent = DbContext.ObjectType.FirstOrDefault(x => x.Name == item2.ParentTable && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                                        var child = DbContext.ObjectType.FirstOrDefault(x => x.Name == item2.ChildTable && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                                        if (parent != null && child != null)
                                        {
                                            DatabaseTableRelations relation = new DatabaseTableRelations()
                                            {
                                                ChildObjecttypeId = child.ObjectTypeId,
                                                ParentObjecttypeId = parent.ObjectTypeId,
                                                ChildPropertyName = item2.ChildPropertyName,
                                                FKName = item2.FKName,
                                                ParentPropertyName = item2.ParentPropertyName,
                                                Virtual = false,
                                                ChildTable = item2.ChildTable,
                                                ParentTable = item2.ParentTable,
                                            };
                                            child.Field = DbContext.Field.Where(x => x.ObjectTypeId == child.ObjectTypeId).ToList();
                                            var fieldch = child.Field.FirstOrDefault(x => x.Name == relation.ChildPropertyName);
                                            if (fieldch != null)
                                            {
                                                fieldch.Type = "foreignkey";
                                            }
                                            currentlist.Add(relation);
                                            DbContext.DatabaseTableRelations.Add(relation);
                                        }

                                    }

                                }
                            }
                        }
                    }
                }
            }

            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost("DisconnectTables/{dbid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> DisconnectTables(Guid dbid, List<string> alltable)
        {
            Guid? userid = this.DbContext.Users.FirstOrDefault(x => x.UserName == "admin")?.Id;
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.AppUserId == userid && x.DatabaseConnectionPropertyId == dbid);
            if (db == null)
            {
                return BadRequest("Database not found");
            }
            if (userid != null)
            {
                foreach (var item in alltable)
                {
                    var type = DbContext.ObjectType.FirstOrDefault(x => x.AppUserId == userid && x.TableName == item && x.DatabaseConnectionPropertyId == db.DatabaseConnectionPropertyId);
                    if (type != null)
                    {
                        var relations = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == type.ObjectTypeId || x.ChildObjecttypeId == type.ObjectTypeId).ToList();
                        DbContext.DatabaseTableRelations.RemoveRange(relations);
                        DbContext.ObjectType.Remove(type);
                    }
                }
            }

            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("GetDatabases")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> GetDatabases()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if(user.UserName.ToLower() == "admin")
            {
                var db = DbContext.DatabaseConnectionProperty;
                return new JsonResult(db, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            else
            {
                var db = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == user.Id);
                return new JsonResult(db, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            }
        }

        [HttpGet("GetPublicDatabases")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> GetPublicDatabases()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var db = DbContext.DatabaseConnectionProperty.Where(x => x.AppUserId == user.Id || x.Default == true || x.Public == true);
            db = db.OrderByDescending(x => x.Default);
            return new JsonResult(db, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }


    }
}
