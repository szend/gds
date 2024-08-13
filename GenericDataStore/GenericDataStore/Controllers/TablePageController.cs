using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablePageController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMemoryCache _memoryCache;

        public TablePageController(IMemoryCache memoryCache, ILogger<TablePageController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
            _memoryCache = memoryCache;
        }

        protected ILogger<TablePageController> Logger { get; set; }
        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("Create")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Create(TablePage page)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if(user == null || user.AllowedDataCount <= 50)
            {
                return BadRequest("Premium feature");
            }
            page.AppUserId = user.Id;
            DbContext.TablePage.Add(page);
            DbContext.SaveChanges();

            return Ok();
        }


        [HttpPost("Edit")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Edit(TablePage page)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var dbpage = DbContext.TablePage.FirstOrDefault(x => x.AppUserId == user.Id && x.TablePageId == page.TablePageId);
            dbpage.Html = page.Html;
            dbpage.Css = page.Css;
            dbpage.Name = page.Name;

            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("GetAllPage/{id}/")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> GetAllPage(Guid id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var dbpages = DbContext.TablePage.Where(x => x.AppUserId == user.Id && x.ObjectTypeId == id).ToList();

            return new JsonResult(dbpages, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("Delete/{id}/")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var dbpages = DbContext.TablePage.FirstOrDefault(x => x.AppUserId == user.Id && x.TablePageId == id);
            if(dbpages != null)
            DbContext.TablePage.Remove(dbpages);
            DbContext.SaveChanges();

            return Ok();
        }


    }
}
