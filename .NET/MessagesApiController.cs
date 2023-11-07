using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePair.Models;
using WePair.Models.Domain.Messages;
using WePair.Models.Requests.Messages;
using WePair.Models.Domain.Users;
using WePair.Web.Hubs;
using WePair.Services;
using WePair.Services.Interfaces;
using WePair.Web.Controllers;
using WePair.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.SignalR;
using WePair.Web.Hubs.Clients;

namespace WePair.Web.Api.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageApiController : BaseApiController
    {
        private IMessageService _service = null;
        private IAuthenticationService<int> _authService = null;
        private readonly IHubContext<ChatHub, IChatHub> _chatHub = null;

        public MessageApiController(IMessageService service
            , ILogger<MessageApiController> logger
            , IHubContext<ChatHub, IChatHub> chatHub
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _chatHub = chatHub;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemsResponse<int>> Create(MessageAddRequest model)
        {

            ObjectResult result = null;
         
            try
            {
                int currentUserId = _authService.GetCurrentUserId();

                int id = _service.Add(model, currentUserId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };

                result = Created201(response);

                

               _chatHub.Clients.User(model.RecipientId.ToString()).ReceiveMessage(model.Body);                
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpGet]

        [HttpGet("users")]
        public ActionResult<ItemsResponse<BaseUser>> GetUsers()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int currentUser = _authService.GetCurrentUserId();

                List<BaseUser> list = _service.GetUsers(currentUser);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemsResponse<BaseUser> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("currentconversation/{recipientId:int}")]
        public ActionResult<ItemsResponse<Message>> GetCurrentUserConversations(int recipientId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                List<Message> list = _service.GetRecentConversationsByUserId(userId, recipientId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemsResponse<Message> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Error:  {ex.Message}");
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }      

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(MessageUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int user = _authService.GetCurrentUser().Id;
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
    }
}
