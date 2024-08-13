using GenericDataStore.Filtering;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace GenericDataStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;

        public MessageController(ILogger<MessageController> logger, ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.userManager = userManager;
        }
        protected ILogger<MessageController> Logger { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        [HttpPost("Create")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> Create([FromBody] UserMessage model)
        {
            var sender = await userManager.FindByNameAsync(User.Identity.Name);
            var type = DbContext.ObjectType.FirstOrDefault(x => x.ObjectTypeId == model.ObjectTypeId);

            if(model.LastMessageId != null)
            {
                var lastMessage = DbContext.UserMessage.FirstOrDefault(x => x.UserMessageId == model.LastMessageId);
                if (lastMessage != null)
                {
                    model.ReceivUserId = lastMessage.SendUserId;
                }
            }
            else if(type != null)
            {
                model.ReceivUserId = type.AppUserId;

            }

            model.SendUserId = sender?.Id;

            model.Date = DateTime.Now;
            this.DbContext.UserMessage.Add(model);
            this.DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("GetMessages")]
        [Authorize(Policy = "Full")]
        public virtual async Task<IActionResult> GetMessages()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var messages = this.DbContext.UserMessage.Where(x => x.SendUserId == user.Id && x.NoVisibleSender != true).ToList();
            var messages2 = this.DbContext.UserMessage.Where(x => x.ReceivUserId == user.Id && x.NoVisibleReceiver != true).ToList();
            messages.AddRange(messages2);
            //List<Guid> obj = this.DbContext.DataObject.Where(x => x.AppUserId == user.Id).Select(x => x.DataObjectId).ToList();

            //var objmass = this.DbContext.UserMessage.Where(x => x.DataObjectId != null ? obj.Contains((Guid)x.DataObjectId) : false).ToList();
            //messages.AddRange(objmass);

            messages = messages.Distinct().ToList();
            foreach (var item in messages)
            {
                var usr1 = DbContext.Users.FirstOrDefault(x => x.Id == item.SendUserId);
                var usr2 = DbContext.Users.FirstOrDefault(x => x.Id == item.ReceivUserId);
                item.SenderName = usr1?.UserName;
                item.ReceiverName = usr2?.UserName;
                item.SenderMail = usr1?.Email;
                item.ReceiverMail = usr2?.Email;

            }
            return new JsonResult(messages, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [HttpGet("Delete/{id}")]
        [Authorize(Policy = "Full")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dbmodel = this.DbContext.UserMessage.FirstOrDefault(x => x.UserMessageId == id);
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (dbmodel != null)
            {
                if (dbmodel.ReceivUserId == user.Id)
                {
                    dbmodel.NoVisibleReceiver = true;
                }
                else if (dbmodel.SendUserId == user.Id)
                {
                    dbmodel.NoVisibleSender = true;
                }

                if (dbmodel.NoVisibleReceiver == true && dbmodel.NoVisibleSender == true)
                {
                    this.DbContext.Remove(dbmodel);

                }

                await this.DbContext.SaveChangesAsync();

            }

            return this.Ok();
        }
    }
}
