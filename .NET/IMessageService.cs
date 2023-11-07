using Microsoft.AspNetCore.Mvc;
using Sabio.Models;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IMessageService
    {
        int Add(MessageAddRequest model, int currentUserId);
        void Delete(int id);
        List<BaseUser> GetUsers(int userId);
        List<Message> GetRecentConversationsByUserId(int userId,int recipientId);       
        void Update(MessageUpdateRequest model );
        Task <string> UserConnected(int userId, string connectionId);
    }
}