using Microsoft.AspNetCore.Mvc;
using WePair.Models;
using WePair.Models.Domain.Messages;
using WePair.Models.Domain.Users;
using WePair.Models.Requests.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WePair.Services.Interfaces
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
