using GenericDataStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;

        public OfferController(ILogger<OfferController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
        }
        protected ILogger<OfferController> Logger { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("Create")]
        public virtual async Task<IActionResult> Create([FromBody] Offer model)
        {
            this.DbContext.Offer.Add(model);
            this.DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("GetAllOffer")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> GetAllOffer()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if(user.Email != "davidszen1@gmail.com" && user.Email != "admin@admin.admin")
            {
                return base.BadRequest();
            }
            var offers = this.DbContext.Offer;
            return new JsonResult(offers, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }


    }
}
