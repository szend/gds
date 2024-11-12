using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardTableController : ControllerBase
    {

        private readonly UserManager<AppUser> userManager;

        public DashboardTableController(ILogger<DashboardTableController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;

        }

        protected ILogger<DashboardTableController> Logger { get; set; }
        protected ApplicationDbContext DbContext { get; set; }



        [HttpPost("AddToDashboard")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> AddToDashboard(DashboardTable dashtable)
        {
            if(dashtable.ObjectTypeId == Guid.Empty || dashtable.ObjectTypeId == null)
            {
                return BadRequest("ObjectTypeId is required");
            }
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            dashtable.AppUserId = user?.Id;
            dashtable.Size = 12;
            var lastdash = DbContext.DashboardTable.Where(x => x.AppUserId == dashtable.AppUserId &&
            dashtable.ObjectTypeId == x.ObjectTypeId && dashtable.RootFilter == x.RootFilter);
            if (lastdash.Count() > 0)
            {
                return BadRequest("Table already exists in the dashboard");
            }

            DbContext.DashboardTable.Add(dashtable);

            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("SaveDashboard/{dashtableid}/{size}/{position}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> SaveDashboard(Guid dashtableid, int size, int position)
        {
            var dash = DbContext.DashboardTable.FirstOrDefault(x => x.DashboardTableId == dashtableid);
            dash.Size = size;
            dash.Position = position;
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("RemoveFromDashboard/{id}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> RemoveFromDashboard(Guid id)
        {
           
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var dash = DbContext.DashboardTable.FirstOrDefault(x => x.DashboardTableId == id);
            if(dash == null || user == null)
            {
                return BadRequest("Table not found");
            }

            DbContext.DashboardTable.Remove(dash);

            DbContext.SaveChanges();

            return Ok();
        }
    }
}
