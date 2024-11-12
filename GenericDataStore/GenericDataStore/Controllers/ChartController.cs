using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;

        public ChartController(ILogger<ChartController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;

        }

        protected ILogger<ChartController> Logger { get; set; }
        protected ApplicationDbContext DbContext { get; set; }



        [HttpPost("AddToDashboard")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> AddToDashboard(Chart chart)
        {
            if (chart.ObjectTypeId == Guid.Empty || chart.ObjectTypeId == null)
            {
                return BadRequest("ObjectTypeId is required");
            }
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            chart.AppUserId = user?.Id;
            chart.Size = 6;
           

            DbContext.Chart.Add(chart);

            DbContext.SaveChanges();

            return Ok();
        }


        [HttpPost("AddToDashboardArray")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> AddToDashboardArray( List<Chart> charts)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            Guid groupid = Guid.NewGuid();  
            foreach (var chart in charts)
            {
                chart.AppUserId = user?.Id;
                chart.Size = 6;
                chart.GroupId = groupid;
                DbContext.Chart.Add(chart);
            }
        

            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet("SaveChartDashboard/{chartid}/{size}/{position}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> SaveDashboard(Guid chartid, int size, int position)
        {
            var ch = DbContext.Chart.FirstOrDefault(x => x.ChartId == chartid);
            ch.Size = size;
            ch.Position = position;
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("RemoveFromDashboard/{id}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> RemoveFromDashboard(Guid id)
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var ch = DbContext.Chart.FirstOrDefault(x => x.ChartId == id);
            if (ch == null || user == null)
            {
                return BadRequest("Table not found");
            }
            var grouped = DbContext.Chart.Where(x => x.GroupId == ch.GroupId && x.GroupId != null && x.GroupId != Guid.Empty).ToList();
            if(grouped.Count > 1)
            {
                foreach (var chart in grouped)
                {
                    DbContext.Chart.Remove(chart);
                }
            }
            else
            {
                DbContext.Chart.Remove(ch);
            }


            DbContext.SaveChanges();

            return Ok();
        }
    }
}
