using GenericDataStore.Filtering;
using GenericDataStore.InputModels;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Caching.Memory;
using GenericDataStore.DatabaseConnector;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ObjectTypeController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMemoryCache _memoryCache;
        public string cacheKey = "types";

        public ObjectTypeController(IMemoryCache memoryCache, ILogger<ObjectTypeController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
            _memoryCache = memoryCache;
        }

        protected ILogger<ObjectTypeController> Logger { get; set; }
        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("GetByFilter")]
        public virtual async Task<IActionResult> GetByFilter()
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = query.Where(x => x.Private != true);
            query = await this.FilterQuery(filters, query);



            return new JsonResult(query, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("GetAllCategory")]
        public virtual async Task<IActionResult> GetAllCategory()
        {
            int idx = 0;
            var query = this.DbContext.ObjectType.Where(x => x.Category != null).Select(x => new { cat = x.Category, id = x.ObjectTypeId });
            var list = query.ToList().DistinctBy(x => x.cat);
            return new JsonResult(list, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("GetChildByFilter")]
        public virtual async Task<IActionResult> GetChildByFilter()
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            //query = query.Where(x => x.ParentObjectTypeId != null);
            query = await this.FilterQuery(filters, query, null, false, false);


            return new JsonResult(query, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("Save")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Save([FromBody] ObjectType model)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (model.Name.StartsWith("0") || model.Name.StartsWith("1") || model.Name.StartsWith("2") || model.Name.StartsWith("3") || model.Name.StartsWith("4") || model.Name.StartsWith("5") || model.Name.StartsWith("6") || model.Name.StartsWith("7") || model.Name.StartsWith("8") || model.Name.StartsWith("9"))
            {
                return BadRequest("Name can't start with number /");
            }
            if (model.Name.Contains("/"))
            {
                return BadRequest("Name can't contain /");
            }
            if (model.DatabaseConnectionPropertyId == null || model.DatabaseConnectionPropertyId == Guid.Empty)
            {
                return BadRequest("Choose a database");
            }
            if (model.Name == "" || model.Name == null)
            {
                return BadRequest("Name is undefinied");
            }
            if (model.Field.Where(x => x.Type == "id").Count() > 1)
            {
               // return BadRequest("You can only have 1 id field");
            }
            if (model.Field.GroupBy(x => x.Name).ToList().Any(x => x.Count() > 1))
            {
                return BadRequest("Can't have fields with the same name");
            }
            if (model.Field.Any(x => x.Name == ""))
            {
                return BadRequest("Can't have field with empty name");
            }
            if (this.DbContext.ObjectType.Any(x => x.Name == model.Name && x.ObjectTypeId != model.ObjectTypeId && x.DatabaseConnectionPropertyId == model.DatabaseConnectionPropertyId))
            {
                return BadRequest("List name already exist in this database");
            }

            var dbmodel = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.ObjectTypeId);

            if (dbmodel == null)
            {
                var Repository = GetRepo(model.DatabaseConnectionPropertyId);
                var database = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == model.DatabaseConnectionPropertyId);
                var alllistcount = DbContext.ObjectType.Where(x => x.AppUserId == user.Id).Count();
                if (user.AllowedListCount <= alllistcount)
                {
                    return BadRequest("You reached the maximum number of list: " + user.AllowedListCount + ". Increase your limits.");
                }

                dbmodel = new ObjectType();
                dbmodel.CreationDate = DateTime.Now;
                dbmodel.Name = model.Name;
                dbmodel.TableName = model.Name;
                dbmodel.Category = model.Category;
                dbmodel.Description = model.Description;
                dbmodel.DenyChart = model.DenyChart;
                dbmodel.DenyExport = model.DenyExport;
                dbmodel.NoFilterMenu = model.NoFilterMenu;
                dbmodel.DenyAdd = model.DenyAdd;
                dbmodel.AllUserFullAccess = model.AllUserFullAccess;
                dbmodel.Private = model.Private;
                dbmodel.DatabaseConnectionPropertyId = model.DatabaseConnectionPropertyId;
                //dbmodel.ParentObjectTypes.Add(DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.p));
                foreach (var item in model.Field)
                {
                    item.PropertyName = item.Name;
                    dbmodel.Field.Add(item);
                }

                user.ObjectType.Add(dbmodel);
                this.DbContext.Add(dbmodel);
                var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == model.DatabaseConnectionPropertyId);
                Repository.CreateTable(dbmodel,db?.DefaultIdType ?? "");

                if(!dbmodel.Field.Any(x => x.Type == "id"))
                {
                    dbmodel.Field.Add(new Field
                    {
                        PropertyName = dbmodel.Name+"Id",
                        Name = dbmodel.Name + "Id",
                        Type = "id",
                        ObjectTypeId = dbmodel.ObjectTypeId
                    });
                }
            }
            else
            {
                var Repository = GetRepo(dbmodel.DatabaseConnectionPropertyId);

                if (dbmodel.AppUserId != user.Id)
                {
                    return BadRequest("no access");
                }

                // dbmodel.AppUser = this.DbContext.Users.FirstOrDefault(x => x.Id == dbmodel.AppUserId);
                if(dbmodel.Name != model.Name)
                {
                    Repository.RenameTable(dbmodel.TableName, model.Name);
                }
                if(dbmodel.Name != model.Name)
                {

                }
                dbmodel.Name = model.Name;
                dbmodel.TableName = model.Name;
                dbmodel.Category = model.Category;
                dbmodel.Description = model.Description;
                dbmodel.DenyChart = model.DenyChart;
                dbmodel.DenyExport = model.DenyExport;
                dbmodel.NoFilterMenu = model.NoFilterMenu;
                dbmodel.DenyAdd = model.DenyAdd;
                dbmodel.AllUserFullAccess = model.AllUserFullAccess;
                dbmodel.Private = model.Private;
                //dbmodel.ParentObjectTypeId = model.ParentObjectTypeId;

                List<Field> fields = DbContext.Set<Field>().Where(x => x.ObjectTypeId == dbmodel.ObjectTypeId).ToList();
                List<Field> newfields = model.Field.Where(x => !fields.Any(y => y.FieldId == x.FieldId)).ToList();
                List<Field> oldfields = fields.Where(x => !model.Field.Any(y => y.FieldId == x.FieldId)).Where(x => x.Name != "AppUserId" && x.Name != "DataObjectId").ToList();

                if(oldfields.Count > 0)
                {
                    if(oldfields.Where(x => x.Type == "image").Count() > 0 || oldfields.Where(x => x.Type == "file").Count() > 0)
                    {
                        var data = Repository.GetAllDataFromTable(dbmodel);
                        foreach (var field in oldfields)
                        {
                            foreach (var item in data)
                            {
                                if (field.Type == "image" || field.Type == "file")
                                {
                                    var value = item.Value.Where(x => x.Name == field.PropertyName).ToList();
                                    FileService.RemoveFile(value);
                                }

                            }
                            if (!field.Type.Contains("calculated"))
                            {
                                Repository.RemoveColumn(dbmodel.TableName, field.PropertyName);
                            }

                        }
                    }
                    else
                    {
                        foreach (var field in oldfields)
                        {
                            if (!field.Type.Contains("calculated"))
                            {
                                Repository.RemoveColumn(dbmodel.TableName, field.PropertyName);
                            }

                        }
                    }

                }
                DbContext.RemoveRange(oldfields);

                foreach (var field in newfields)
                {
                    field.PropertyName = field.Name;
                    dbmodel.Field.Add(field);

                    Repository.AddColumn(dbmodel.TableName, field.PropertyName, field.Type);
                }


                foreach (var item in dbmodel.Field)
                {
                    foreach (var item2 in model.Field)
                    {
                        if (item.FieldId == item2.FieldId && item.FieldId != Guid.Empty)
                        {
                            if (item.Name != item2.Name)
                            {
                                if (item2.Type.ToLower().Contains("calculated"))
                                {
                                }
                                else
                                {
                                    Repository.UpdateColumn(dbmodel.TableName, item.Name, item2.Name, item2.Type);
                                }
                                item.Name = item2.Name;
                                item.PropertyName = item2.Name;
                            }
                            if (item.Type != item2.Type)
                            {
                                if(item2.Type.ToLower() == "option" || item2.Type.ToLower().Contains("calculated"))
                                {
                                }
                                else
                                {
                                    Repository.UpdateColumnType(dbmodel.TableName, item.PropertyName, item2.Type);
                                }
                                item.Type = item2.Type;
                            }
                            item.Option = item2.Option;
                            item.CalculationMethod = item2.CalculationMethod;
                            item.ColorMethod = item2.ColorMethod;
                            List<Option> opts = DbContext.Set<Option>().Where(x => x.FieldId == item.FieldId && !item.Option.Contains(x)).ToList();

                            this.DbContext.RemoveRange(opts);
                        }
                    }
                }

            }

            this.DbContext.SaveChanges();
            return this.Ok();
        }

        [HttpPost("AddChild/{parentid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> AddChild([FromBody] ObjectType model, Guid parentid)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (model.Name.StartsWith("0") || model.Name.StartsWith("1") || model.Name.StartsWith("2") || model.Name.StartsWith("3") || model.Name.StartsWith("4") || model.Name.StartsWith("5") || model.Name.StartsWith("6") || model.Name.StartsWith("7") || model.Name.StartsWith("8") || model.Name.StartsWith("9"))
            {
                return BadRequest("Name can't start with number /");
            }
            if (model.Name.Contains("/"))
            {
                return BadRequest("Name can't contain /");
            }
            if (model.DatabaseConnectionPropertyId == null || model.DatabaseConnectionPropertyId == Guid.Empty)
            {
                return BadRequest("Choose a database");
            }
            if (model.Name == "" || model.Name == null)
            {
                return BadRequest("Name is undefinied");
            }
            if (model.Field.Where(x => x.Type == "id").Count() > 1)
            {
                return BadRequest("You can only have 1 id field");
            }
            if (model.Field.GroupBy(x => x.Name).ToList().Any(x => x.Count() > 1))
            {
                return BadRequest("Can't have fields with the same name");
            }
            if (model.Field.Any(x => x.Name == ""))
            {
                return BadRequest("Can't have field with empty name");
            }
            if (this.DbContext.ObjectType.Any(x => x.Name == model.Name && x.ObjectTypeId != model.ObjectTypeId && x.DatabaseConnectionPropertyId == model.DatabaseConnectionPropertyId))
            {
                return BadRequest("List name already exist in this database");

            }

            var dbmodel = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.ObjectTypeId);

                var Repository = GetRepo(model.DatabaseConnectionPropertyId);

                var alllistcount = DbContext.ObjectType.Where(x => x.AppUserId == user.Id).Count();
                if (user.AllowedListCount <= alllistcount)
                {
                    return BadRequest("You reached the maximum number of list: " + user.AllowedListCount + ". Increase your limits.");
                }
                dbmodel = new ObjectType();
                dbmodel.CreationDate = DateTime.Now;
                dbmodel.Name = model.Name;
                dbmodel.TableName = model.Name;
                dbmodel.Category = model.Category;
                dbmodel.Description = model.Description;
                dbmodel.DenyChart = model.DenyChart;
                dbmodel.DenyExport = model.DenyExport;
                dbmodel.NoFilterMenu = model.NoFilterMenu;
                dbmodel.DenyAdd = model.DenyAdd;
                dbmodel.AllUserFullAccess = model.AllUserFullAccess;
                dbmodel.Private = model.Private;
                dbmodel.DatabaseConnectionPropertyId = model.DatabaseConnectionPropertyId;
                //dbmodel.ParentObjectTypes.Add(DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.p));
                foreach (var item in model.Field)
                {
                    item.PropertyName = item.Name;
                    dbmodel.Field.Add(item);
                }

                user.ObjectType.Add(dbmodel);
                this.DbContext.Add(dbmodel);
                var parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == parentid);
            parent.Field = DbContext.Field.Where(x => x.ObjectTypeId == parent.ObjectTypeId).ToList();
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == dbmodel.DatabaseConnectionPropertyId);
            if(parent.DatabaseConnectionPropertyId != dbmodel.DatabaseConnectionPropertyId)
            {
                Repository.AddChild(dbmodel, parent, db.DefaultIdType,true);
            }
            else
            {
                Repository.AddChild(dbmodel, parent, db.DefaultIdType);
            }

            dbmodel.Field.Add(new Field()
            {
                Type = "foreignkey",
                Name = parent.Name + "Id",
                PropertyName = parent.Name + "Id",
                ObjectTypeId = dbmodel.ObjectTypeId
            });
                if (!dbmodel.Field.Any(x => x.Type == "id"))
                {
                    dbmodel.Field.Add(new Field
                    {
                        PropertyName = dbmodel.Name + "Id",
                        Name = dbmodel.Name + "Id",
                        Type = "id",
                        ObjectTypeId = dbmodel.ObjectTypeId
                    });
                }

            DatabaseTableRelations databaseTableRelations = new DatabaseTableRelations()
            {
                ChildObjecttypeId = dbmodel.ObjectTypeId,
                ParentObjecttypeId = parentid,
                ChildTable = dbmodel.Name,
                ParentTable = parent.Name,
                ChildPropertyName = parent.Name + "Id",
                ParentPropertyName = parent.Field.FirstOrDefault(x => x.Type == "id").Name,
                Virtual = dbmodel.DatabaseConnectionPropertyId != parent.DatabaseConnectionPropertyId,
                FKName = dbmodel.Name + parent.Name + "FK"
            };
            DbContext.DatabaseTableRelations.Add(databaseTableRelations);

            this.DbContext.SaveChanges();
            return this.Ok();
        }

        [HttpGet("RemoveChild/{childid}/{parentid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> RemoveChild(Guid childid, Guid parentid)
        {
            var child = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == childid);
            var parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == parentid);
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == child.DatabaseConnectionPropertyId);
            var Repository = GetRepo(child.DatabaseConnectionPropertyId);
            var connection = DbContext.DatabaseTableRelations.FirstOrDefault(x => x.ParentObjecttypeId == parentid && x.ChildObjecttypeId == childid);
            if(parent.DatabaseConnectionPropertyId == child.DatabaseConnectionPropertyId)
            {
                Repository.RemoveRelation(connection.ParentTable, connection.ChildTable, connection.ParentPropertyName, connection.ChildPropertyName);

            }
            Repository.RemoveColumn(child.TableName, connection.ChildPropertyName);
            child.Field = DbContext.Field.Where(x => x.ObjectTypeId == child.ObjectTypeId).ToList();
            child.Field.Remove(child.Field.FirstOrDefault(x => x.Name == connection.ChildPropertyName));
            DbContext.DatabaseTableRelations.Remove(connection);

            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("GetAllChild/{parentid}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> GetAllChild(Guid parentid)
        {

            var parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == parentid);
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == parent.DatabaseConnectionPropertyId);
            var Repository = GetRepo(parent.DatabaseConnectionPropertyId);
            var relations = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == parentid).ToList();
            List<ObjectType> children = new List<ObjectType>();
            foreach (var item in relations)
            {
                var child = DbContext.ObjectType.FirstOrDefault(x => x.Name == item.ChildTable);
                if (child != null)
                {
                    children.Add(child);
                }
            }
            return new JsonResult(children, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("Delete/{id}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dbmodel = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
                var Repository = GetRepo(dbmodel.DatabaseConnectionPropertyId);

                if (dbmodel != null)
                {
                    var fields = DbContext.Field.Where(x => x.ObjectTypeId == dbmodel.ObjectTypeId).ToList();
                    var imgtypefields = fields.Where(x => x.Type == "image" || x.Type == "file");
                    if(imgtypefields.Count() > 0)
                    {
                        var obj = Repository.GetAllDataFromTable(dbmodel);
                        foreach (var item in imgtypefields)
                        {
                                foreach (var item2 in obj)
                                {
                                    var value = item2.Value.Where(x => x.Name == item.PropertyName).ToList();
                                    FileService.RemoveFile(value);
                                }
                        }
                    }
                    var connections = DbContext.DatabaseTableRelations.Where(x => x.ChildObjecttypeId == id || x.ParentObjecttypeId == id);
                    DbContext.DatabaseTableRelations.RemoveRange(connections);
                    this.DbContext.ObjectType.Remove(dbmodel);
                    await this.DbContext.SaveChangesAsync();
                    Repository.DropTable(dbmodel.TableName);

                }

                return this.Ok();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        [Authorize(Policy = "Full")]
        [HttpPost("GetByFilterPrivate")]
        public virtual async Task<IActionResult> GetByFilterPrivate()
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            var user = await this.userManager.FindByNameAsync(User.Identity.Name);
            query = query.Where(x => x.AppUserId == user.Id);
            query = await FilterQuery(filters, query);

            return new JsonResult(query, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }    

        [HttpGet("GetChartDataBase/{id}/")]
        public virtual async Task<IActionResult> GetChartDataBase(Guid id)
        {
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == id);
            db.ObjectType = DbContext.ObjectType.Where(x => x.DatabaseConnectionPropertyId == id).ToList();
            ChartModelOrganisation chartModelOrganisation = new ChartModelOrganisation();
            DatabaseChartModelSetter(db,chartModelOrganisation);
            return new JsonResult(chartModelOrganisation, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("GetChartData")]
        public virtual async Task<IActionResult> GetChartData()
        {

            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            RootFilter? filterResult;
            query = await FilterQuery(filters, query, null, true,true, null, true);

            List<ChartModelType> optch = new List<ChartModelType>();
            List<ChartModelType> numch = new List<ChartModelType>();
            List<ChartModelType> boolch = new List<ChartModelType>();
            foreach (var item in query)
            {
                foreach (var field in item.Field)
                {
                    if(field.Type == "text" || field.Type == "calculatedtext")
                    {
                        List<string> values = new List<string>();
                        foreach (var obj in item.DataObject)
                        {
                            var val = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                            values.Add(val?.ValueString);
                        }
                        if (values.GroupBy(values => values).Count() < 25)
                        {
                            var grval = values.GroupBy(values => values).Select(x => new { Name = x.Key, Count = x.Count() }).ToList();
                            ChartModelType chtyp = new ChartModelType();
                            chtyp.Datasets.Add(new ChartModelDataset());
                            chtyp.Name = field.Name;
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                            optch.Add(chtyp);
                        }

                    }
                    if (field.Type == "option")
                    {
                        ChartModelType chtyp = new ChartModelType();
                        chtyp.Datasets.Add(new ChartModelDataset());
                        chtyp.Name = field.Name;
                        List<string> values = new List<string>();
                        foreach (var obj in item.DataObject)
                        {
                            var val = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                            values.Add(val?.ValueString);
                        }
                        var grval = values.GroupBy(values => values).Select(x => new { Name = x.Key, Count = x.Count() }).ToList();
                        foreach (var g in grval)
                        {
                            chtyp.Labels.Add(g.Name);
                            chtyp.Datasets[0].Data.Add(g.Count);
                            var random = new Random();
                            var color = String.Format("#{0:X6}", random.Next(0x1000000));
                            chtyp.Datasets[0].BackgroundColor.Add(color);
                            chtyp.Datasets[0].BorderColor.Add(color);

                        }
                        optch.Add(chtyp);
                    }
                    if (field.Type == "numeric" || field.Type == "calculatednumeric")
                    {
                        ChartModelType chtyp = new ChartModelType();
                        chtyp.Datasets.Add(new ChartModelDataset());
                        chtyp.Name = field.Name;
                        List<double> values = new List<double>();
                        foreach (var obj in item.DataObject)
                        {
                            var val = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                            if (val.ValueString != "∞" && val?.ValueString != null && val?.ValueString != "")
                            {
                                if (val.ValueString.Contains("."))
                                {
                                    values.Add(Math.Round(double.Parse((val?.ValueString != "" && val?.ValueString != null ? val?.ValueString : "0"), CultureInfo.InvariantCulture),4));
                                }
                                //else if (val.ValueString.Contains("E"))
                                //{

                                //}
                                else
                                {
                                    values.Add(Math.Round(double.Parse((val?.ValueString != "" && val?.ValueString != null ? val?.ValueString : "0")),4));

                                }
                            }
                            else
                            {
                                values.Add(0);

                            }
                        }
                        //var grval = values.OrderBy(x => x);
                        chtyp.Datasets[0].Label = field.Name;
                        int idx = 1;
                        var random = new Random();
                        var color = String.Format("#{0:X6}", random.Next(0x1000000));
                        chtyp.Datasets[0].BorderColor.Add(color);
                        chtyp.Datasets[0].BackgroundColor.Add(color);
                        foreach (var g in values)
                        {
                            chtyp.Labels.Add(idx.ToString());
                            chtyp.Datasets[0].Data.Add(g);

                            idx++;
                        }
                        numch.Add(chtyp);
                    }
                    if (field.Type == "boolean" || field.Type == "calculatedboolean")
                    {
                        ChartModelType chtyp = new ChartModelType();
                        chtyp.Datasets.Add(new ChartModelDataset());
                        chtyp.Name = field.Name;
                        List<bool?> values = new List<bool?>();
                        foreach (var obj in item.DataObject)
                        {
                            var val = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                            values.Add(val?.ValueString != "" && val?.ValueString != null ? Boolean.Parse(val?.ValueString) : null);
                        }
                        //var grval = values.OrderBy(x => x);
                        var groupvalues = values.GroupBy(x => x).Select(x => new { Name = x.Key, Count = x.Count() }).ToList();
                        chtyp.Datasets[0].Label = field.Name;
                        int idx = 1;
                        foreach (var g in groupvalues)
                        {
                            chtyp.Labels.Add(g?.Name?.ToString() ?? "");
                            chtyp.Datasets[0].Data.Add(g.Count);
                            var random = new Random();
                            var color = String.Format("#{0:X6}", random.Next(0x1000000));
                            chtyp.Datasets[0].BackgroundColor.Add(color);
                            idx++;
                        }
                        boolch.Add(chtyp);
                    }
                }

            }
            var first = query.FirstOrDefault();
            ChartModelOrganisation chorg = new ChartModelOrganisation();
            chorg.Name = first.Name;
            
            ChartModelSetter(first,chorg);

            ChartModel ch = new ChartModel();
            ch.Options = optch;
            ch.Numbers = numch;
            ch.Booleans = boolch;
            ch.Organisation = chorg;

            return new JsonResult(ch, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [Authorize(Policy = "Full")]
        [HttpPost("CreateCalculatedChart")]
        public virtual async Task<IActionResult> CreateCalculatedChart([FromBody] ChartInput chartInput)
        {

            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            RootFilter? filterResult;
            query = await FilterQuery("", query, null, true, true, chartInput.Filter, true);
            var type = query.FirstOrDefault();

            if (type != null)
            {
                
                    Field fieldx = new Field
                    {
                        CalculationMethod = chartInput.Xcalculation,
                        Name = "CalculatedX",
                        PropertyName = "CalculatedX",
                        Type = "calculatednumeric",
                        ObjectTypeId = type.ObjectTypeId
                    };
                    Field fieldy = new Field
                    {
                        CalculationMethod = chartInput.Ycalculation,
                        Name = "CalculatedY",
                        PropertyName = "CalculatedY",
                        Type = "calculatednumeric",
                        ObjectTypeId = type.ObjectTypeId
                    };
                    type.Field.Add(fieldx);
                    type.Field.Add(fieldy);
                    this.CalculateValue(fieldx, type);
                    this.CalculateValue(fieldy, type);

                    double k = 0;
                    if(chartInput.GroupOption.ToLower() != "none")
                    {
                    ChartModelType chtyp = new ChartModelType();
                    chtyp.Datasets.Add(new ChartModelDataset());
                    chtyp.Name = "Grouped: " + chartInput.Xcalculation + "-" + chartInput.Ycalculation;
                    List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                    foreach (var obj in type.DataObject)
                    {
                        var valx = obj.Value.FirstOrDefault(x => x.Name == fieldx.Name);
                        var valy = obj.Value.FirstOrDefault(x => x.Name == fieldy.Name);
                        KeyValuePair<string, string> p = new KeyValuePair<string, string>(valx?.ValueString, valy?.ValueString);
                        values.Add(p);
                    }
                    if (chartInput.GroupOption.ToLower() == "count")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.Count() }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "sum")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.Sum(y => double.TryParse(y.Value?.Replace(',', '.'), CultureInfo.InvariantCulture, out k) == true ? k : 0) }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "average")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.Average(y => double.TryParse(y.Value?.Replace(',', '.'), CultureInfo.InvariantCulture, out k) == true ? k : 0) }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count != double.NaN ? g.Count : 0);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "min")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.Min(y => double.TryParse(y.Value?.Replace(',', '.'), CultureInfo.InvariantCulture, out k) == true ? k : 0) }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "max")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.Max(y => double.TryParse(y.Value?.Replace(',', '.'), CultureInfo.InvariantCulture, out k) == true ? k : 0) }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(g.Count);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "first")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.FirstOrDefault().Value }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                            chtyp.Datasets[0].Data.Add(double.TryParse(g.Count?.Replace(",", ".") ?? "0", CultureInfo.InvariantCulture, out k) == true ? k : 0);
                            var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                        else if (chartInput.GroupOption.ToLower() == "last")
                        {
                            var grval = values.GroupBy(values => values.Key).Select(x => new { Name = x.Key, Count = x.LastOrDefault().Value }).ToList();
                            foreach (var g in grval)
                            {
                                chtyp.Labels.Add(g.Name);
                                chtyp.Datasets[0].Data.Add(double.TryParse(g.Count?.Replace(",", ".") ?? "0", CultureInfo.InvariantCulture, out k) == true ? k : 0);
                                var random = new Random();
                                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                                chtyp.Datasets[0].BackgroundColor.Add(color);
                                chtyp.Datasets[0].BorderColor.Add(color);

                            }
                        }
                    return new JsonResult(chtyp, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
                else
                    {
                        ChartModelType chtyp = new ChartModelType();
                        chtyp.Datasets.Add(new ChartModelDataset());
                        chtyp.Name = chartInput.Xcalculation + "-" + chartInput.Ycalculation;
                        List<double> values = new List<double>();
                        List<object> labels = new List<object>();

                        foreach (var obj in type.DataObject)
                        {
                            var val = obj.Value.FirstOrDefault(x => x.Name == fieldy.Name);
                            if (val != null && val.ValueString != "∞" && val?.ValueString != null && val?.ValueString != "")
                            {
                                if (val.ValueString.Contains("."))
                                {
                                    try
                                    {
                                        values.Add(Math.Round(double.Parse((val?.ValueString != "" && val?.ValueString != null ? val?.ValueString : "0"), CultureInfo.InvariantCulture), 4));
                                    }
                                    catch (Exception)
                                    {
                                        values.Add(0);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        values.Add(Math.Round(double.Parse((val?.ValueString != "" && val?.ValueString != null ? val?.ValueString : "0")), 4));
                                    }
                                    catch (Exception)
                                    {
                                        values.Add(0);
                                    }

                                }
                            }
                            else
                            {
                                values.Add(0);

                            }
                            var lab = obj.Value.FirstOrDefault(x => x.Name == fieldx.Name);
                            if (lab != null && lab.ValueString != "∞" && lab?.ValueString != null && lab?.ValueString != "")
                            {
                                labels.Add((lab?.ValueString != "" && lab?.ValueString != null ? lab?.ValueString : "0"));

                            }
                            else
                            {
                                labels.Add(0);

                            }
                        }

                        //var grval = values.OrderBy(x => x);
                        chtyp.Datasets[0].Label = "custom";
                        var random = new Random();
                        var color = String.Format("#{0:X6}", random.Next(0x1000000));
                        chtyp.Datasets[0].BorderColor.Add(color);
                        chtyp.Datasets[0].BackgroundColor.Add(color);
                        foreach (var g in values)
                        {
                            chtyp.Datasets[0].Data.Add(g);
                        }

                        foreach (var g in labels)
                        {
                            chtyp.Labels.Add(g.ToString());
                        }
                        return new JsonResult(chtyp, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    }           
            }

            return BadRequest("Type not found");
        }

        [Authorize(Policy = "Full")]
        [HttpGet("Search")]
        public virtual async Task<IActionResult> Search(string searchstring)
        {
            var lists = DbContext.ObjectType.Where(x => x.Name.Contains(searchstring) || (x.Category != null ? x.Category.Contains(searchstring) : false) || (x.Description != null ? x.Description.Contains(searchstring) : false)).Where(x => x.Private != true).ToList();
            lists = lists.OrderByDescending(x => x.Promoted).ToList();


            List<Value> values = new List<Value>();
            foreach (var item in lists)
            {
                Repository Repository = GetRepo(item.DatabaseConnectionPropertyId);
                item.Field = DbContext.Field.Where(x => x.ObjectTypeId == item.ObjectTypeId).ToList();
                if (item != null)
                {
                    var alldata = Repository.GetAllDataFromTable(item);
                    foreach (var item2 in alldata)
                    {
                        foreach (var item3 in item2.Value)
                        {
                            if (values.Count > 500)
                            {
                                return new JsonResult(new { lists = lists, values = values }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                            }
                            
                            if (item3 != null && item3.ValueString != null && item3.ValueString.Contains(searchstring))
                            {
                                values.Add(item3);
                            }
                        }
                    }
                }
            }
            return new JsonResult(new { lists = lists, values = values }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [Authorize(Policy = "Full")]
        [HttpGet("SelectChild/{childid}/{parentid}")]
        public virtual async Task<IActionResult> SelectChild(string childid, string parentid)
        {

            var child = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == Guid.Parse(childid));
            var parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == Guid.Parse(parentid));
            var parentfield = DbContext.Field.FirstOrDefault(x => x.ObjectTypeId == parent.ObjectTypeId && x.Type == "id");
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == child.DatabaseConnectionPropertyId);
            var Repository = GetRepo(child.DatabaseConnectionPropertyId);
            if (db.DatabaseConnectionPropertyId != parent.DatabaseConnectionPropertyId)
            {
                Repository.CreateRelation(parent.Name, child.Name, parentfield.Name, parent.Name + "Id", db.DefaultIdType,true);
            }
            else
            {
                Repository.CreateRelation(parent.Name, child.Name, parentfield.Name, parent.Name + "Id", db.DefaultIdType);
            }
            child.Field.Add(new Field()
            {

                Name = parent.Name + "Id",
                PropertyName = parent.Name + "Id",
                Type = "foreignkey",
                ObjectTypeId = child.ObjectTypeId
            });
            DatabaseTableRelations databaseTableRelations = new DatabaseTableRelations()
            {
                ChildObjecttypeId = child.ObjectTypeId,
                ParentObjecttypeId = parent.ObjectTypeId,
                ChildTable = child.Name,
                ParentTable = parent.Name,
                ChildPropertyName = parent.Name + "Id",
                ParentPropertyName = parentfield.Name,
                Virtual = child.DatabaseConnectionPropertyId != parent.DatabaseConnectionPropertyId,
                FKName = child.Name + parent.Name + "FK"
            };
            DbContext.DatabaseTableRelations.Add(databaseTableRelations);
            DbContext.SaveChanges();
            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        [Authorize(Policy = "Full")]
        [HttpPost("SaveCalculatedField")]
        public virtual async Task<IActionResult> SaveCalculatedField([FromBody] CalculatedField calculatedfield)
        {
            var type = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == calculatedfield.TypeId);

            if (type != null)
            {
                if (DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).Any(x => x.Name == calculatedfield.Name))
                {
                    return BadRequest("Name already exist");
                }
                Field field = new Field() { Name = calculatedfield.Name, PropertyName = calculatedfield.Name, Type = "calculated" + calculatedfield.OriginalType, ObjectTypeId = type.ObjectTypeId };

                field.CalculationMethod = calculatedfield.CalculationString;
                type.Field.Add(field);
            }
            else
            {
                return BadRequest("Type not found");
            }
            DbContext.SaveChanges();

            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [Authorize(Policy = "Full")]
        [HttpPost("SaveCalculatedColor")]
        public virtual async Task<IActionResult> SaveCalculatedColor([FromBody] CalculatedColor calculatedcolor)
        {
            var type = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == calculatedcolor.TypeId);

            if (type != null)
            {
                var field = DbContext.Field.FirstOrDefault(x => x.ObjectTypeId == type.ObjectTypeId && x.FieldId == calculatedcolor.FieldId);
                if (field == null)
                {
                    return BadRequest("Field does not exist");
                }

                field.ColorMethod = calculatedcolor.CalculationColor;
            }
            else
            {
                return BadRequest("Type not found");
            }
            DbContext.SaveChanges();

            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("ML")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> ML()
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = await this.FilterQuery(filters, query, null, true);

            var first = query.FirstOrDefault();

            var mlquery = first?.DataObject.Select(x => MLService.TransformDataObject(x, first.Field.ToList()));
            if (mlquery == null)
            {
                return BadRequest("No data to train");
            }
            List<bool> fieldtypestr = first.Field.ToList().Select(x => x.Type != "numeric" && x.Type != "calculatednumeric").ToList();
            List<ClusteringMetrics> metricslist = new List<ClusteringMetrics>();
            int clustercount = 2;
            List<MLService> mlService = new List<MLService>();
            bool cancontinue = true;
            while (cancontinue && (metricslist.Count < 3 || Math.Abs((metricslist[clustercount - 5].AverageDistance - metricslist[clustercount - 4].AverageDistance) - (metricslist[clustercount - 4].AverageDistance - metricslist[clustercount - 3].AverageDistance)) > (metricslist.Last().AverageDistance / 4)))
            {
                var tempservice = new MLService(clustercount);
                mlService.Add(tempservice);
                try
                {
                    metricslist.Add(tempservice.TrainModel(mlquery, fieldtypestr));
                }
                catch (Exception)
                {
                    cancontinue = false;

                }
                clustercount++;
            }
            // var metrics = mlService?.TrainModel(mlquery,fieldtypestr);
            List<Prediction> predictions = new List<Prediction>();

            mlService.Remove(mlService.Last());

            foreach (var item in mlquery)
            {
                var p = mlService.Last()?.Predict(item);

                predictions.Add(p);
            }
            int fieldcount = first.Field.Where(x => x.Name.Contains("PredictedClusterId")).Count();
            if (first.Field.Any(x => x.Name == "PredictedClusterId" + fieldcount))
            {
                fieldcount++;
            }
            Field clusterfield = new Field() { Name = "PredictedClusterId" + fieldcount, Type = "numeric", ObjectTypeId = first.ObjectTypeId };
            first.Field.Add(clusterfield);
            Repository repository = GetRepo(first.DatabaseConnectionPropertyId);
            repository.AddColumn(first.TableName, clusterfield.Name, clusterfield.Type);
            int idx = 0;
            foreach (var item in first.DataObject)
            {
                item.Value.Add(new Value() { ObjectTypeId = first.ObjectTypeId, Name = clusterfield.Name, ValueString = predictions[idx].PredictedClusterId.ToString() });
                repository.UpdateValue(first, item);
                idx++;
            }
            DbContext.SaveChanges();
            var gr = predictions.GroupBy(x => x.PredictedClusterId);
            ClusteringResult clusteringResult = new ClusteringResult();

            List<float> centroids = new List<float>();

            foreach (var item in gr)
            {
                centroids.Add(item.Sum(x => x.Distances[item.Key - 1]) / item.Count());
            }

            foreach (var item in gr)
            {
                var color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                clusteringResult.Datasets.Add(
                    new ClusteringDataset()
                    {
                        Label = item.Key.ToString(),
                        Data = item.ToList().Select(x => x.Distances.ToList()).ToList(),
                        BackgroundColor = color,
                        BorderColor = color
                    }
                    ); ;
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var admin = await userManager.FindByEmailAsync("admin@admin.admin");


            UserMessage userMessage = new UserMessage()
            {
                Comment = "Clustering finished. We have created a new field for the clusters. You can delete or edit it any time",
                ReceivUserId = user.Id,
                SendUserId = admin.Id,
                Date = DateTime.Now,
            };
            DbContext.UserMessage.Add(userMessage);
            DbContext.SaveChanges();


            return new JsonResult(clusteringResult, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("ImageClassification/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> ImageClassification(string name)
        {
            return BadRequest("Image Classification CURRENTLY NOT AVAILABLE IN ONLYNE ENVIROMENT");
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = await this.FilterQuery(filters, query, null, true);

            var first = query.FirstOrDefault();

            ImageClassificationService imageClassificationService = new ImageClassificationService();
            imageClassificationService.Init(first, name);

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var admin = await userManager.FindByEmailAsync("admin@admin.admin");
            UserMessage userMessage = new UserMessage()
            {
                Comment = "Image classification finished. Now you can use the model. If you double click an element in the list, you can see a new button under your selected property",
                ReceivUserId = user.Id,
                SendUserId = admin.Id,
                Date = DateTime.Now,
            };

            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("ClassifySingleImage/{id}/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> ClassifySingleImage(Guid id, string name, List<KeyValuePair<string, string>> keys)
        {
            var dict = keys.ToDictionary(pair => pair.Key, pair => pair.Value);
            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            type.Field = this.DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();
            var Repository = GetRepo(type.DatabaseConnectionPropertyId);
            var obj = Repository.GetByDataObjectId(type, dict);
            ImageClassificationService imageClassificationService = new ImageClassificationService();
            var pr = imageClassificationService.ClassifySingleImage(obj, type, name);
            var res = pr.Select(x => new { Field = x.FieldName, Value = x.PredictedLabelValue });
            return new JsonResult(res, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("CreateRegression/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> CreateRegression(string name)
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = await this.FilterQuery(filters, query, null, true);

            var first = query.FirstOrDefault();

            var mlquery = first?.DataObject.ToList().Select(x => RegressionService.TransformDataObject(x, first.Field.ToList(), name)).ToList();
            if (mlquery == null)
            {
                return BadRequest("No data to train");
            }
            List<bool> fieldtypestr = first.Field.ToList().Where(x => x.Name != name).Select(x => x.Type != "numeric" && x.Type != "calculatednumeric").ToList();
            RegressionService regressionService = new RegressionService();
            var learnmodels = mlquery.Take(mlquery.Count() / 10 * 9).ToList();
            var testmodels = mlquery.Skip(mlquery.Count() / 10 * 9).ToList();
            var res = regressionService.Init(learnmodels, testmodels, fieldtypestr);
            regressionService.Save(first, name);

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var admin = await userManager.FindByEmailAsync("admin@admin.admin");
            UserMessage userMessage = new UserMessage()
            {
                Comment = "Regression finished. Now you can use the model. If you double click an element in the list, you can see a new button under your selected property",
                ReceivUserId = user.Id,
                SendUserId = admin.Id,
                Date = DateTime.Now,
            };



            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("PredictRegression/{id}/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> PredictRegression(Guid id, string name,[FromBody] List<KeyValuePair<string, string>> keys)
        {
            var dict = keys.ToDictionary(pair => pair.Key, pair => pair.Value);
            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            type.Field = this.DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();
            var Repository = GetRepo(type.DatabaseConnectionPropertyId);
            var obj = Repository.GetByDataObjectId(type, dict);
            RegressionService imageClassificationService = new RegressionService();
            imageClassificationService.LoadFromFile(type, name);
            var valuemodel = RegressionService.TransformDataObject(obj, type.Field.ToList(), name);
            var pr = imageClassificationService.TestSinglePrediction(valuemodel);

            return new JsonResult(new { value = pr.FareAmount != float.NaN ? pr.FareAmount : 0, field = name }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("Classification/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> Classification(string name)
        {
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = await this.FilterQuery(filters, query, null, true);

            var first = query.FirstOrDefault();

            var mlquery = first?.DataObject.ToList().Select(x => RegressionService.TransformDataObject(x, first.Field.ToList(), name)).ToList();
            if (mlquery == null)
            {
                return BadRequest("No data to train");
            }
            List<bool> fieldtypestr = first.Field.ToList().Where(x => x.Name != name).Select(x => x.Type != "numeric" && x.Type != "calculatednumeric").ToList();
            MultiClassificationService classificationService = new MultiClassificationService();

            var metrics = classificationService?.TrainModel(mlquery, fieldtypestr);

            classificationService.SaveModel(first, name);

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var admin = await userManager.FindByEmailAsync("admin@admin.admin");
            UserMessage userMessage = new UserMessage()
            {
                Comment = "Classification finished. Now you can use the model. If you double click an element in the list, you can see a new button under your selected property",
                ReceivUserId = user.Id,
                SendUserId = admin.Id,
                Date = DateTime.Now,
            };

            return new JsonResult(null, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("PredictClass/{id}/{name}")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> PredictClass(Guid id, string name, List<KeyValuePair<string, string>> keys)
        {
            var dict = keys.ToDictionary(pair => pair.Key, pair => pair.Value);
            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            type.Field = this.DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();
            var Repository = GetRepo(type.DatabaseConnectionPropertyId);
            var obj = Repository.GetByDataObjectId(type,dict);
            MultiClassificationService classificationService = new MultiClassificationService();
            classificationService.LoadModel(type, name);
            var valuemodel = RegressionService.TransformDataObject(obj, type.Field.ToList(), name);
            var pr = classificationService.Predict(valuemodel);

            return new JsonResult(new { value = pr.PredictedLabel, field = name }, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("AIModels/{id}/")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> AIModels(Guid id)
        {

            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            type.Field = this.DbContext.Field.Where(x => x.ObjectTypeId == type.ObjectTypeId).ToList();

            string path = Path.Combine(Environment.CurrentDirectory, "AIModels");
            string calssifications = Path.Combine(path, "classificationmodels");
            string imagemodels = Path.Combine(path, "imagemodels");
            string regressionmodels = Path.Combine(path, "regressionmodels");

            calssifications = Path.Combine(calssifications, type.ObjectTypeId.ToString());
            imagemodels = Path.Combine(imagemodels, type.ObjectTypeId.ToString());
            regressionmodels = Path.Combine(regressionmodels, type.ObjectTypeId.ToString());
            List<string> modelnames = new List<string>();
            if (Directory.Exists(calssifications))
            {
                DirectoryInfo d = new DirectoryInfo(calssifications); //Assuming Test is your Folder

                FileInfo[] Files = d.GetFiles("*.mdl");
                foreach (var item in Files)
                {
                    modelnames.Add(item.Name.Split('.')[0]);
                }

            }
            if (Directory.Exists(imagemodels))
            {
                DirectoryInfo d = new DirectoryInfo(imagemodels); //Assuming Test is your Folder

                FileInfo[] Files = d.GetFiles("*.mdl");
                foreach (var item in Files)
                {
                    modelnames.Add(item.Name.Split('.')[0]);
                }

            }
            if (Directory.Exists(regressionmodels))
            {
                DirectoryInfo d = new DirectoryInfo(regressionmodels); //Assuming Test is your Folder

                FileInfo[] Files = d.GetFiles("*.mdl");
                foreach (var item in Files)
                {
                    modelnames.Add(item.Name.Split('.')[0]);
                }

            }

            return new JsonResult(modelnames, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }


        [HttpGet("GetChildsName/{id}/")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> GetChildsName(Guid id)
        {

            var type = this.DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            var repository = GetRepo(type.DatabaseConnectionPropertyId);
            var connections = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == type.ObjectTypeId);
            if(connections == null)
            {
                return new JsonResult(new List<string>(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            return new JsonResult(connections.Select(x => x.ChildTable), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpPost("EditAllFiltered")]
        public virtual async Task<IActionResult> EditAllFiltered([FromBody] AllEditModel model)
        {

            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();
            query = await this.FilterQuery("", query, null, true, true,model.Filter);
            var first = query.FirstOrDefault();
            if (first != null)
            {
                var Repository = GetRepo(first.DatabaseConnectionPropertyId);
                foreach (var item in first.DataObject)
                {
                    foreach (var item2 in model.Object.Value.Where(x => x.ValueString != null && x.ValueString != ""))
                    {
                        if(item.Value.FirstOrDefault(x => x.Name == item2.Name) != null)
                        {
                            item.Value.FirstOrDefault(x => x.Name == item2.Name).ValueString = item2.ValueString;
                        }
                    }
                    Repository.UpdateValue(first, item);
                }
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("GetPage/{id}/{name}/")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> GetPage(Guid id, string name)
        {
            string html = "";
            string filters = await new StreamReader(Request.Body).ReadToEndAsync();
            RootFilter? filterResult = null;
            var query = this.DbContext.Set<ObjectType>().ToArray().AsQueryable();

            query = await this.FilterQuery(filters, query);

            var first = query.FirstOrDefault();
            var page = DbContext.TablePage.FirstOrDefault(x => x.ObjectTypeId == id && x.Name == name);
            var data = first.DataObject.ToList().Select(x => x.Value);

            return new JsonResult(new {data = data, page = page}, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("SelectParent/{id}/{name}/")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> SelectParent(Guid id, string name)
        {
            var child = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == id);
            var connection = DbContext.DatabaseTableRelations.FirstOrDefault(x => x.ChildObjecttypeId == id && x.ChildPropertyName == name);
            if(child != null && connection != null)
            {
                var parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == connection.ParentObjecttypeId);
                return new JsonResult(parent, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            }
            return BadRequest();

        }



        private async Task<IQueryable<ObjectType>> FilterQuery(string filters, IQueryable<ObjectType> query, Guid? objid = null, bool chart = false, bool all = true, RootFilter rootfilter = null, bool onlyfirstx = false)
        {
            RootFilter? filterResult = null;
            IQueryable<ObjectType> cachquery = null;
            if (/*!_memoryCache.TryGetValue(filters + objid + chart + all, out cachquery)*/true)
            {
                if (!string.IsNullOrEmpty(filters) || rootfilter != null)
                {
                    if(rootfilter != null)
                    {
                        filterResult = rootfilter;
                    }
                    else
                    {
                        filterResult = JsonConvert.DeserializeObject<RootFilter>(filters);
                    }
                    if (filterResult != null)
                    {
                        var parentfilter = filterResult.Filters.FirstOrDefault(x => x.Field == "ParentObjectTypeId");
                        if (parentfilter != null)
                        {
                            var parent = query.FirstOrDefault(x => x.ObjectTypeId.ToString().ToUpper() == parentfilter.Value.ToString().ToUpper());
                            var parentrepo = GetRepo(parent.DatabaseConnectionPropertyId);
                            var connections = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == parent.ObjectTypeId);
                            if(connections == null)
                            {
                                query = query.Where(x => false);

                            }
                            else
                            {
                                query = query.Where(x => connections.Any(y => y.ChildTable == x.Name));
                            }
                            filterResult.Filters.Remove(filterResult.Filters.FirstOrDefault(x => x.Field == "ParentObjectTypeId"));
                        }

                        query = CreateQuery<ObjectType>.ApplyFilter(query, filterResult);
                        query = query.OrderByDescending(x => x.CreationDate);
                        query = CreateQuery<ObjectType>.SortData(query, filterResult.SortingParams);

                        if (filterResult?.Take != 0)
                        {
                            query = query.Skip(filterResult.Skip).Take(filterResult.Take);
                        }
                    }
                }
                if (query != null && ((objid != null) || (filterResult != null && all == true && filterResult?.Filters?.Where(x => x.Field == "category").Count() == 0 && query.Count() == 1)))
                {
                    foreach (var item in query)
                    {
                        if (true)
                        {
                            var Repository = GetRepo(item.DatabaseConnectionPropertyId);
                            item.Field = this.DbContext.Set<Field>().Where(x => x.ObjectTypeId == item.ObjectTypeId).ToList();
                            if (filterResult != null && filterResult.ValueFilters != null && filterResult.ValueFilters.Any(x => x.Field == "ParentDataObjectId"))
                            {
                                var pfilter = filterResult.ValueFilters.FirstOrDefault(x => x.Field == "ParentDataObjectId").Operator.ToString();
                                var parenttable = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId.ToString() == pfilter);
                                var connection = DbContext.DatabaseTableRelations.FirstOrDefault(x => x.ChildObjecttypeId == item.ObjectTypeId && x.ParentObjecttypeId == parenttable.ObjectTypeId);
                                filterResult.ValueFilters.Add(new Filter()
                                {
                                    Field = connection.ChildPropertyName,
                                    Operator = "equals",
                                    Value = filterResult.ValueFilters.FirstOrDefault(x => x.Field == "ParentDataObjectId").Value,
                                });
                                filterResult.ValueFilters.RemoveAll(x => x.Field == "ParentDataObjectId");
                            }

                            if (filterResult.ValueFilters.Any(x => x.Field == "AppUserId"))
                            {
                                filterResult.ValueFilters.FirstOrDefault(Field => Field.Field == "AppUserId").Value = item.AppUserId.ToString();
                            }
                            item.Count = Repository.GetCount(item, filterResult);
                            if(item.Count > 2000 && onlyfirstx)
                            {
                                onlyfirstx = true;
                            }
                            else
                            {
                                onlyfirstx = false;
                            }
                            item.DataObject = Repository.GetAllDataFromTable(item, filterResult,chart,onlyfirstx);


                            //item.DataObject = CreateQuery<DataObject>.ObjectFilter(item.DataObject.AsQueryable(), filterResult).ToList();

                            //item.DataObject = item.DataObject.Reverse().ToList();




                            foreach (var item2 in item.Field)
                            {
                                if (item2.Type == "option")
                                {
                                    item2.Option = this.DbContext.Set<Option>().Where(x => x.FieldId == item2.FieldId).ToList();
                                }
                                if (item2.Type.Contains("calculatednumeric"))
                                {
                                    CalculateValue(item2, item);
                                }
                                else if (item2.Type.Contains("calculatedtext"))
                                {
                                    CalculateValue(item2, item);
                                }
                                else if (item2.Type.Contains("calculatedboolean"))
                                {
                                    CalculateValue(item2, item);
                                }
                            }


                            foreach (var item2 in item.Field.Where(x =>  x.ColorMethod != null && x.ColorMethod != ""))
                            {
                                CalculateColor(item2, item);
                            }


                        }
                    }
                }
                //_memoryCache.Set(filters + objid + chart + all, query,
                //        new MemoryCacheEntryOptions()
                //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
            }
            else
            {
                query = cachquery;
            }
            return query;
        }

        private void CalculateColor(Field field, ObjectType type)
        {
            var names = Regex.Matches(field.ColorMethod, @"(\{.+?\})");
            var functions = Regex.Matches(field.ColorMethod, @"(\[.+?\])");
            StringToValue stf = new StringToValue();
            var repository = GetRepo(type.DatabaseConnectionPropertyId);
            var connections = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == type.ObjectTypeId || x.ChildObjecttypeId == type.ObjectTypeId);
            if (field.ColorMethod != null && field.ColorMethod != "")
            {
                foreach (var obj in type.DataObject)
                {
                    try
                    {
                        var childscache = new List<ObjectType>();
                        var parentscache = new List<ObjectType>();
                        string resultstring = field.ColorMethod;
                        foreach (var item in names)
                        {
                            var matchfield = type.Field.FirstOrDefault(x => x.Name.ToLower() == item.ToString().ToLower().Replace("{", "").Replace("}", ""));
                            if(matchfield != null)
                            {
                                var value = obj.Value.FirstOrDefault(x => x.Name == matchfield.Name);
                                if (value != null)
                                {
                                    if (value.ValueString != null)
                                    {
                                        object dvalue = null;
                                        if (matchfield.Type == "numeric" || matchfield.Type == "calculatednumeric")
                                        {
                                            if (value.ValueString.Contains("."))
                                            {
                                                dvalue = double.Parse(value.ValueString, CultureInfo.InvariantCulture);
                                            }
                                            else
                                            {
                                                dvalue = double.Parse(value.ValueString);
                                            }
                                        }
                                        else
                                        {
                                            dvalue = value.ValueString;
                                        }

                                        resultstring = resultstring.Replace(item.ToString(), dvalue.ToString());
                                    }
                                }
                            }
                        }

                        foreach (var item in functions)
                        {
                            string[] word = item.ToString().Replace("[", "").Replace("]", "").Split(".");
                            if (word[0].ToLower() == "child")
                            {

                                if (connections != null)
                                {
                                    var connection = connections.FirstOrDefault(x => x.ParentTable == type.Name && x.ChildTable.ToLower() == word[1].ToLower() && x.ParentObjecttypeId == type.ObjectTypeId);
                                    if (connection != null)
                                    {
                                        RootFilter childfilter = new RootFilter()
                                        {
                                            ValueFilters = new List<Filter>()
                                            {
                                                new Filter()
                                                {
                                                    Field = connection.ChildPropertyName,
                                                    Operator = "equals",
                                                    Value = obj.Value.FirstOrDefault(x => x.Name == connection.ParentPropertyName).ValueString
                                                }
                                            }
                                        };
                                        ObjectType child = null;
                                        if (childscache.Any(x => x.Name == connection.ChildTable))
                                        {
                                            child = childscache.FirstOrDefault(x => x.Name == connection.ChildTable);
                                        }
                                        else
                                        {
                                            child = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == connection.ChildObjecttypeId);
                                            if (child != null)
                                            {
                                                child.Field = DbContext.Field.Where(x => x.ObjectTypeId == child.ObjectTypeId).ToList();
                                                if (type.DatabaseConnectionPropertyId != child.DatabaseConnectionPropertyId)
                                                {
                                                    var childrepo = this.GetRepo(child.DatabaseConnectionPropertyId);
                                                    child.DataObject = childrepo.GetAllDataFromTable(child, childfilter, true);

                                                }
                                                else
                                                {
                                                    child.DataObject = repository.GetAllDataFromTable(child, childfilter, true);
                                                }
                                                childscache.Add(child);
                                            }
                                        }
                                        if (child != null)
                                        {
                                            var childfield = child.Field.FirstOrDefault(x => x.Name == word[2]);
                                            if (childfield != null)
                                            {
                                                if (childfield.Type.Contains("calculated"))
                                                {
                                                    CalculateValue(childfield, child);
                                                }
                                                var allvalue = child.DataObject.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                var objvalues = child.DataObject.ToList();
                                                for (int i = 4; i < word.Length; i++)
                                                {
                                                    if (word[i].Split('(')[0].ToLower() == "where")
                                                    {
                                                        var where = word[i].Split('(')[1].Split(')')[0].Trim().Replace(" ", "");
                                                        if (where.Contains("->"))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("->")[0]);
                                                            var wherevalue = where.Split("->")[1].Split('(')[1].Split(')')[0].Split(',');
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => wherevalue.Contains(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString)).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                        }
                                                        else if (where.Contains("<-"))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("<-")[0]);
                                                            var wherevalue = where.Split("<-")[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString.Contains(wherevalue)).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                        }
                                                        else if (where.Contains("=in"))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("=in")[0]);
                                                            var wherevalue = where.Split("=in")[1].Split('(')[1].Split(')')[0].Split(',');
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => wherevalue.Contains(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString)).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                        }
                                                        else if (where.Contains(">="))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split(">=")[0]);
                                                            var wherevalue = where.Split(">=")[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) >= double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                        }
                                                        else if (where.Contains("<="))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("<=")[0]);
                                                            var wherevalue = where.Split("<=")[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) <= double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                        }
                                                        else if (where.Contains("!="))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("!=")[0]);
                                                            var wherevalue = where.Split("!=")[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) != double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                        }
                                                        else if (where.Contains("="))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('=')[0]);
                                                            var wherevalue = where.Split('=')[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) == double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                        }
                                                        else if (where.Contains("<"))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('<')[0]);
                                                            var wherevalue = where.Split('<')[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) < double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                        }
                                                        else if (where.Contains(">"))
                                                        {
                                                            var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('>')[0]);
                                                            var wherevalue = where.Split('>')[1];
                                                            if (wherefield != null)
                                                            {
                                                                objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) > double.Parse(wherevalue) : false).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                        }


                                                    }
                                                    else if (word[i].Split('(')[0].ToLower() == "orderby")
                                                    {

                                                        var where = word[i].Split('(')[1].Split(')')[0];
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where);
                                                        if (wherefield != null)
                                                        {
                                                            if (wherefield.Type == "numeric" || wherefield.Type == "calculatednumeric")
                                                            {
                                                                objvalues = objvalues.OrderBy(x => double.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name)?.ValueString ?? double.MaxValue.ToString())).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                            else if (wherefield.Type == "boolean" || wherefield.Type == "calculatedboolean")
                                                            {
                                                                objvalues = objvalues.OrderBy(x => bool.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name)?.ValueString ?? false.ToString())).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                            else
                                                            {
                                                                objvalues = objvalues.OrderBy(x => x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }

                                                        }
                                                    }
                                                    else if (word[i].Split('(')[0].ToLower() == "orderbydesc")
                                                    {

                                                        var where = word[i].Split('(')[1].Split(')')[0];
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where);
                                                        if (wherefield != null)
                                                        {
                                                            if (wherefield.Type == "numeric" || wherefield.Type == "calculatednumeric")
                                                            {
                                                                objvalues = objvalues.OrderByDescending(x => double.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name)?.ValueString ?? double.MinValue.ToString())).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                            else if (wherefield.Type == "boolean" || wherefield.Type == "calculatedboolean")
                                                            {
                                                                objvalues = objvalues.OrderByDescending(x => bool.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name)?.ValueString ?? true.ToString())).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                            }
                                                            else
                                                            {
                                                                objvalues = objvalues.OrderByDescending(x => x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString).ToList();
                                                                allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                            }
                                                        }
                                                    }

                                                }
                                                resultstring = FieldValueCalculator.Calculate(word[3], allvalue, resultstring, item);
                                            }

                                        }
                                    }
                                }
                            }
                            else if (word[0].ToLower() == "parent")
                            {
                                if (connections != null)
                                {
                                    var connection = connections.FirstOrDefault(x => x.ChildTable == type.Name && x.ParentTable.ToLower() == word[1].ToLower() && x.ChildObjecttypeId == type.ObjectTypeId);
                                    if (connection != null)
                                    {
                                        RootFilter parentfilter = new RootFilter()
                                        {
                                            ValueFilters = new List<Filter>()
                                            {
                                                new Filter()
                                                {
                                                    Field = connection.ParentPropertyName,
                                                    Operator = "equals",
                                                    Value = obj.Value.FirstOrDefault(x => x.Name == connection.ChildPropertyName).ValueString
                                                }
                                            }
                                        };
                                        ObjectType parent = null;
                                        if (parentscache.Any(x => x.Name == connection.ChildTable))
                                        {
                                            parent = parentscache.FirstOrDefault(x => x.Name == connection.ChildTable);
                                        }
                                        else
                                        {
                                            parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == connection.ParentObjecttypeId);
                                            if (parent != null)
                                            {
                                                parent.Field = DbContext.Field.Where(x => x.ObjectTypeId == parent.ObjectTypeId).ToList();
                                                if (type.DatabaseConnectionPropertyId != parent.DatabaseConnectionPropertyId)
                                                {
                                                    var parentrepo = this.GetRepo(parent.DatabaseConnectionPropertyId);
                                                    parent.DataObject = parentrepo.GetAllDataFromTable(parent, parentfilter, true);

                                                }
                                                else
                                                {
                                                    parent.DataObject = repository.GetAllDataFromTable(parent, parentfilter, true);
                                                }
                                                parentscache.Add(parent);
                                            }
                                        }
                                        if (parent != null)
                                        {
                                            var parentfield = parent.Field.FirstOrDefault(x => x.Name == word[2]);
                                            if(parentfield != null)
                                            {
                                                if (parentfield.Type.Contains("calculated"))
                                                {
                                                    this.CalculateValue(parentfield, parent);
                                                }

                                                var allvalue = parent.DataObject.Select(x => x.Value.FirstOrDefault(x => x.Name == parentfield.Name).ValueString).FirstOrDefault();
                                                var objvalues = parent.DataObject.ToList();
                                                if (item != null && allvalue != null)
                                                {
                                                    resultstring = resultstring.Replace(item.ToString(), allvalue.ToString());

                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        var controls = Regex.Matches(resultstring, @"(\$.+?\$)");
                        foreach (var item in controls)
                        {
                            string control = item.ToString().Replace("$", "");
                            if (control.ToString().ToLower().Replace(" ", "").StartsWith("if'"))
                            {
                                string ifword = Regex.Matches(control.ToString(), @"(\'.+?\')")[0].ToString().Replace("'", "");
                                object ifresult = stf.Eval(ifword.Replace("(", " ( ").Replace(")", " ) "));
                                if (ifresult.ToString().ToLower() == "true")
                                {
                                    resultstring = control.ToString().Split("??")[1];
                                }
                            }


                        }


                        object result = stf.Eval(resultstring);
                        var resultvalue = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                        if (resultvalue != null)
                        {
                                resultvalue.Color = result.ToString();
                            
                        }
                    }
                    catch (Exception)
                    {


                    }
                }
            }


        }


        private void CalculateValue(Field field, ObjectType type)
        {
            var names = Regex.Matches(field.CalculationMethod, @"(\{.+?\})");
            var functions = Regex.Matches(field.CalculationMethod, @"(\[.+?\])");
            StringToValue stf = new StringToValue();
            var repository = GetRepo(type.DatabaseConnectionPropertyId);
            var connections = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == type.ObjectTypeId || x.ChildObjecttypeId == type.ObjectTypeId);
            if (field.CalculationMethod != null && field.CalculationMethod != "")
            {
                foreach (var obj in type.DataObject)
                {
                    try
                    {
                        var childscache = new List<ObjectType>();
                        var parentscache = new List<ObjectType>();
                        string resultstring = field.CalculationMethod;
                        foreach (var item in names)
                        {
                            var matchfield = type.Field.FirstOrDefault(x => x.Name.ToLower() == item.ToString().ToLower().Replace("{", "").Replace("}", ""));
                            var value = obj.Value.FirstOrDefault(x => x.Name == matchfield.Name);
                            if (value != null)
                            {
                                if (value.ValueString != null)
                                {
                                    object dvalue = null;
                                    if(matchfield.Type == "numeric" || matchfield.Type == "calculatednumeric")
                                    {
                                        if (value.ValueString.Contains("."))
                                        {
                                            dvalue = double.Parse(value.ValueString, CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            dvalue = double.Parse(value.ValueString);
                                        }
                                    }
                                    else
                                    {
                                        dvalue = value.ValueString;
                                    }

                                    resultstring = resultstring.Replace(item.ToString(), dvalue.ToString());
                                }
                            }
                        }

                        foreach (var item in functions)
                        {
                            string[] word = item.ToString().Replace("[", "").Replace("]", "").Split(".");
                            if (word[0].ToLower() == "child")
                            {

                                if (connections != null)
                                {
                                    var connection = connections.FirstOrDefault(x => x.ParentTable == type.Name && x.ChildTable.ToLower() == word[1].ToLower() && x.ParentObjecttypeId == type.ObjectTypeId);
                                    if (connection != null)
                                    {
                                        RootFilter childfilter = new RootFilter()
                                        {
                                            ValueFilters = new List<Filter>()
                                            {
                                                new Filter()
                                                {
                                                    Field = connection.ChildPropertyName,
                                                    Operator = "equals",
                                                    Value = obj.Value.FirstOrDefault(x => x.Name == connection.ParentPropertyName).ValueString
                                                }
                                            }
                                        };
                                        ObjectType child = null;
                                        if (childscache.Any(x => x.Name == connection.ChildTable))
                                        {
                                            child = childscache.FirstOrDefault(x => x.Name == connection.ChildTable);
                                        }
                                        else
                                        {
                                            child = DbContext.ObjectType.FirstOrDefault(x =>x.ObjectTypeId == connection.ChildObjecttypeId);
                                            if (child != null)
                                            {
                                                child.Field = DbContext.Field.Where(x => x.ObjectTypeId == child.ObjectTypeId).ToList();
                                                if(type.DatabaseConnectionPropertyId != child.DatabaseConnectionPropertyId)
                                                {
                                                    var childrepo = this.GetRepo(child.DatabaseConnectionPropertyId);
                                                    child.DataObject = childrepo.GetAllDataFromTable(child, childfilter, true);

                                                }
                                                else
                                                {
                                                    child.DataObject = repository.GetAllDataFromTable(child, childfilter, true);
                                                }
                                                childscache.Add(child);
                                            }
                                        }
                                        if (child != null)
                                        {
                                            var childfield = child.Field.FirstOrDefault(x => x.Name == word[2]);
                                            if (childfield.Type.Contains("calculated"))
                                            {
                                                CalculateValue(childfield, child);
                                            }
                                            var allvalue = child.DataObject.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                            var objvalues = child.DataObject.ToList();
                                            for (int i = 4; i < word.Length; i++)
                                            {
                                                if (word[i].Split('(')[0].ToLower() == "where")
                                                {
                                                    var where = word[i].Split('(')[1].Split(')')[0].Trim().Replace(" ", "");
                                                    if (where.Contains("->"))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("->")[0]);
                                                        var wherevalue = where.Split("->")[1].Split('(')[1].Split(')')[0].Split(',');
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => wherevalue.Contains(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                    }
                                                    else if (where.Contains("<-"))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("<-")[0]);
                                                        var wherevalue = where.Split("<-")[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString.Contains(wherevalue)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                    }
                                                    else if (where.Contains("=in"))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("=in")[0]);
                                                        var wherevalue = where.Split("=in")[1].Split('(')[1].Split(')')[0].Split(',');
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => wherevalue.Contains(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                    }
                                                    else if (where.Contains(">="))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split(">=")[0]);
                                                        var wherevalue = where.Split(">=")[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) >= double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                    }
                                                    else if (where.Contains("<="))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("<=")[0]);
                                                        var wherevalue = where.Split("<=")[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) <= double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                    }
                                                    else if (where.Contains("!="))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split("!=")[0]);
                                                        var wherevalue = where.Split("!=")[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) != double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                    }
                                                    else if (where.Contains("="))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('=')[0]);
                                                        var wherevalue = where.Split('=')[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) == double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                    }
                                                    else if (where.Contains("<"))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('<')[0]);
                                                        var wherevalue = where.Split('<')[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) < double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                    }
                                                    else if (where.Contains(">"))
                                                    {
                                                        var wherefield = child.Field.FirstOrDefault(x => x.Name == where.Split('>')[0]);
                                                        var wherevalue = where.Split('>')[1];
                                                        if (wherefield != null)
                                                        {
                                                            objvalues = objvalues.Where(x => x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != null && x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString != "" ? double.Parse(x.Value.FirstOrDefault(x => x.Name == wherefield.Name).ValueString) > double.Parse(wherevalue) : false).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                    }


                                                }
                                                else if (word[i].Split('(')[0].ToLower() == "orderby")
                                                {

                                                    var where = word[i].Split('(')[1].Split(')')[0];
                                                    var wherefield = child.Field.FirstOrDefault(x => x.Name == where);
                                                    if (wherefield != null)
                                                    {
                                                        if(wherefield.Type == "numeric" || wherefield.Type == "calculatednumeric")
                                                        {
                                                            objvalues = objvalues.OrderBy(x => double.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                        else if(wherefield.Type == "boolean" || wherefield.Type == "calculatedboolean")
                                                        {
                                                            objvalues = objvalues.OrderBy(x => bool.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                        else
                                                        {
                                                            objvalues = objvalues.OrderBy(x => x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }

                                                    }
                                                }
                                                else if (word[i].Split('(')[0].ToLower() == "orderbydesc")
                                                {

                                                    var where = word[i].Split('(')[1].Split(')')[0];
                                                    var wherefield = child.Field.FirstOrDefault(x => x.Name == where);
                                                    if (wherefield != null)
                                                    {
                                                        if (wherefield.Type == "numeric" || wherefield.Type == "calculatednumeric")
                                                        {
                                                            objvalues = objvalues.OrderByDescending(x => double.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                        else if (wherefield.Type == "boolean" || wherefield.Type == "calculatedboolean")
                                                        {
                                                            objvalues = objvalues.OrderByDescending(x => bool.Parse(x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString)).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();

                                                        }
                                                        else
                                                        {
                                                            objvalues = objvalues.OrderByDescending(x => x.Value.FirstOrDefault(y => y.Name == wherefield.Name).ValueString).ToList();
                                                            allvalue = objvalues.Select(x => x.Value.FirstOrDefault(x => x.Name == childfield.Name).ValueString).ToList();
                                                        }
                                                    }
                                                }

                                            }
                                            resultstring = FieldValueCalculator.Calculate(word[3], allvalue, resultstring, item);

                                        }
                                    }
                                }
                            }
                            else if (word[0].ToLower() == "parent")
                            {
                                if (connections != null)
                                {
                                    var connection = connections.FirstOrDefault(x => x.ChildTable == type.Name && x.ParentTable.ToLower() == word[1].ToLower() && x.ChildObjecttypeId == type.ObjectTypeId);
                                    if (connection != null)
                                    {
                                        RootFilter parentfilter = new RootFilter()
                                        {
                                            ValueFilters = new List<Filter>()
                                            {
                                                new Filter()
                                                {
                                                    Field = connection.ParentPropertyName,
                                                    Operator = "equals",
                                                    Value = obj.Value.FirstOrDefault(x => x.Name == connection.ChildPropertyName).ValueString
                                                }
                                            }
                                        };
                                        ObjectType parent = null;
                                        if (parentscache.Any(x => x.Name == connection.ChildTable))
                                        {
                                            parent = parentscache.FirstOrDefault(x => x.Name == connection.ChildTable);
                                        }
                                        else
                                        {
                                            parent = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == connection.ParentObjecttypeId);
                                            if (parent != null)
                                            {
                                                parent.Field = DbContext.Field.Where(x => x.ObjectTypeId == parent.ObjectTypeId).ToList();
                                                if (type.DatabaseConnectionPropertyId != parent.DatabaseConnectionPropertyId)
                                                {
                                                    var parentrepo = this.GetRepo(parent.DatabaseConnectionPropertyId);
                                                    parent.DataObject = parentrepo.GetAllDataFromTable(parent, parentfilter, true);

                                                }
                                                else
                                                {
                                                    parent.DataObject = repository.GetAllDataFromTable(parent, parentfilter, true);
                                                }
                                                parentscache.Add(parent);
                                            }
                                        }
                                        if (parent != null)
                                        {
                                            var parentfield = parent.Field.FirstOrDefault(x => x.Name == word[2]);
                                            if (parentfield.Type.Contains("calculated"))
                                            {
                                                CalculateValue(parentfield, parent);
                                            }
                                            var allvalue = parent.DataObject.Select(x => x.Value.FirstOrDefault(x => x.Name == parentfield.Name).ValueString).FirstOrDefault();
                                            var objvalues = parent.DataObject.ToList();
                                            resultstring = resultstring.Replace(item.ToString(), allvalue.ToString());
                                        }
                                    }
                                }
                            }

                        }

                        var controls = Regex.Matches(resultstring, @"(\$.+?\$)");
                        foreach (var item in controls)
                        {
                            string control = item.ToString().Replace("$", "");
                            if(control.ToString().ToLower().Replace(" ", "").StartsWith("if'"))
                            {
                                string ifword = Regex.Matches(control.ToString(), @"(\'.+?\')")[0].ToString().Replace("'","");
                                object ifresult = stf.Eval(ifword.Replace("("," ( ").Replace(")"," ) "));
                                if(ifresult.ToString().ToLower() == "true")
                                {
                                    resultstring = control.ToString().Split("??")[1];
                                }
                            }


                        }


                        object result = stf.Eval(resultstring);
                        var resultvalue = obj.Value.FirstOrDefault(x => x.Name == field.Name);
                        if (resultvalue != null)
                        {
                            if(field.Type == "numeric" || field.Type == "calculatednumeric")
                            {
                                resultvalue.ValueString = result.ToString().Replace(",", ".");
                            }
                            else
                            {
                                resultvalue.ValueString = result.ToString();
                            }
                        }
                        else
                        {
                            if (field.Type == "numeric" || field.Type == "calculatednumeric")
                            {
                                obj.Value.Add(new Value()
                                {
                                    Name = field.Name,
                                    ValueString = result.ToString().Replace(",", ".")
                                });

                            }
                            else
                            {
                                obj.Value.Add(new Value()
                                {
                                    Name = field.Name,
                                    ValueString = result.ToString()
                                });
                            }


                        }
                    }
                    catch (Exception)
                    {


                    }
                }
            }


        }

        private void ChartModelSetter(ObjectType type, ChartModelOrganisation chart)
        {
            ChartModelOrganisationData baseelement = new ChartModelOrganisationData()
            {
                Name = type.Name,
                Y = 150
            };
            chart.Data.Add(baseelement);
            var connection = DbContext.DatabaseTableRelations.Where(x => x.ParentObjecttypeId == type.ObjectTypeId || x.ChildObjecttypeId == type.ObjectTypeId).ToList();
            var nton = connection.Where(x => x.ParentTable == type.Name || x.ChildTable == type.Name).ToList();
            var names = nton.Select(x => x.ParentTable).ToList();
            names.AddRange(nton.Select(x => x.ChildTable));

            var sibnames = names.Where(x => names.Count(y => y == x) > 1 && x != type.Name).Distinct().ToList();
            nton = nton.Where(x => sibnames.Contains(x.ParentTable) || sibnames.Contains(x.ChildTable)).ToList();
            connection = connection.Where(x => !nton.Contains(x)).ToList();
            nton = nton.Where(x => x.ChildTable == type.Name).ToList();

            var childconnections = connection.Where(x => x.ParentTable == type.Name);
            int idx = 1;
            foreach (var item in childconnections)
            {
                var childtype = DbContext.ObjectType.FirstOrDefault(x => x.Name == item.ChildTable);
                if(childtype != null)
                {
                    ChartModelOrganisationData child = new ChartModelOrganisationData()
                    {
                        Name = item.ChildTable,
                        X = (1000 / childconnections.Count()) * idx,
                        Y = 500,
                        ObjId = childtype.ObjectTypeId
                    };
                    if (childtype.Private)
                    {
                        child.Private = true;
                    }
                    chart.Data.Add(child);
                    chart.Links.Add(new ChartModelOrganisationLink()
                    {
                        Source = baseelement.Name,
                        Target = child.Name,
                    });
                }


                
                idx++;
            }
            var parentconnections = connection.Where(x => x.ChildTable == type.Name);
            idx = 1;
            foreach (var item in parentconnections)
            {
                var parenttype = DbContext.ObjectType.FirstOrDefault(x => x.Name == item.ParentTable);
                if(parenttype != null)
                {
                    ChartModelOrganisationData child = new ChartModelOrganisationData()
                    {
                        Name = item.ParentTable,
                        X = (1000 / parentconnections.Count()) * idx,
                        Y = 0,
                        ObjId = parenttype.ObjectTypeId
                    };
                    if (parenttype.Private)
                    {
                        child.Private = true;
                    }
                    chart.Data.Add(child);
                    chart.Links.Add(new ChartModelOrganisationLink()
                    {
                        Target = baseelement.Name,
                        Source = child.Name,
                    });
                }

                idx++;
            }
            idx = 1;
            foreach (var item in nton)
            {
                var sibling = DbContext.ObjectType.FirstOrDefault(x => (x.Name == item.ParentTable) && x.ObjectTypeId != type.ObjectTypeId);
                if (sibling != null)
                {
                    ChartModelOrganisationData child = new ChartModelOrganisationData()
                    {
                        Name = sibling.Name,
                        X = (1000 / nton.Count()) * idx,
                        Y = 300,
                        ObjId = sibling.ObjectTypeId
                    };
                    if (sibling.Private)
                    {
                        child.Private = true;
                    }
                    chart.Data.Add(child);
                    chart.Links.Add(new ChartModelOrganisationLink()
                    {
                        Target = baseelement.Name,
                        Source = child.Name,
                    });
                    chart.Links.Add(new ChartModelOrganisationLink()
                    {
                        Source = baseelement.Name,
                        Target = child.Name,
                    });
                }

                idx++;
            }

            if (chart.Data.Where(x => x.X != 0).Count() > 0)
            {
                baseelement.X = ((int)Math.Round(chart.Data.Where(x => x.X != 0).Select(x => x.X).Average()));

            }
            else
            {
                baseelement.X = 0;

            }

        }

        private void DatabaseChartModelSetter(DatabaseConnectionProperty db, ChartModelOrganisation chart)
        {
            List<DatabaseTableRelations> allrelation = new List<DatabaseTableRelations>();
            List<ObjectType> parents = new List<ObjectType>();
            List<int> layer = new List<int>();
            int count = db.ObjectType.Count();
            int rad = count * 500;
            int angle = 360 / count;
            int idx = 0;
            foreach (var item in db.ObjectType)
            {
                var connections = DbContext.DatabaseTableRelations.Where(x => x.Virtual != true && (x.ParentObjecttypeId == item.ObjectTypeId || x.ChildObjecttypeId == item.ObjectTypeId)).ToList();
                allrelation.AddRange(connections);

                int levely = GetCircleCoordinatey(idx, count, rad);
                int levelx = GetCircleCoordinatex(idx, count, rad);
                ChartModelOrganisationData element = new ChartModelOrganisationData()
                {
                    Name = item.Name,
                    Y = levely,
                    X = levelx
                };
                chart.Data.Add(element);
                idx++;
            }
            allrelation = allrelation.Distinct().ToList();
            foreach (var item in allrelation)
            {
                chart.Links.Add(new ChartModelOrganisationLink()
                {
                    Target = item.ChildTable,
                    Source = item.ParentTable,
                });
            }

            chart.Data = chart.Data.Distinct().ToList();
            chart.Links = chart.Links.Distinct().ToList();
        }

        private int GetCircleCoordinatex(int idx, int count, int rad)
        {
            return (int)(rad * Math.Cos((idx * 360 / count) * Math.PI / 180));
        }

        private int GetCircleCoordinatey(int idx, int count, int rad)
        {
            return (int)(rad * Math.Sin((idx * 360 / count) * Math.PI / 180));
        }

        private Repository GetRepo(Guid? id)
        {
            var db = DbContext.DatabaseConnectionProperty.FirstOrDefault(x => x.DatabaseConnectionPropertyId == id);

            Repository Repository = new Repository(db.ConnectionString, db.DatabaseType);
            return Repository;
        }
    }
}
