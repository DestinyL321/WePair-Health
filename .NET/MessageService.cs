using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Configuration.UserSecrets;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Messages;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class MessageService : IMessageService
    {
        IDataProvider _data = null;
        IAuthenticationService<int> _authenticationService;

        private static Dictionary<int, string> _connectUsers = new Dictionary<int, string>();

        public MessageService(IDataProvider data, IAuthenticationService<int> authService)
        {
            _data = data;
            _authenticationService = authService;
        }

        public Task<string> UserConnected(int userId, string connectionId)
        {
            if (_connectUsers.ContainsKey(userId))
            {
                _connectUsers[userId] = connectionId;
            }
            _connectUsers.Add(userId, connectionId);

            return Task.FromResult(connectionId);
        }

        public int Add(MessageAddRequest model, int currentUserId)
        {
            int id = 0;

            string procName = "[dbo].[Messages_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    col.AddWithValue("@currentUser", currentUserId);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                }
                );
            return id;
        }

        public void Update(MessageUpdateRequest model)
        {
            string procName = "[dbo].[Messages_Update]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Body", model.Body);
                    col.AddWithValue("@Subject", model.Subject);
                    col.AddWithValue("@Id", model.Id);

                },
                returnParameters: null);
        }
               
        public List<BaseUser> GetUsers(int userId)
        {
            string procName = "[dbo].[Messages_Select_Users_InConversation]";

            List<BaseUser> list = null;

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@UserId", userId);
                },
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    BaseUser baseUser = reader.DeserializeObject<BaseUser>(startingIndex);


                    if (list == null)
                    {
                        list = new List<BaseUser>();
                    }
                    list.Add(baseUser);
                });
            return list;
        }

        public List<Message> GetRecentConversationsByUserId(int userId, int recipientId)
        {
            List<Message> list = null;
            string procName = "[dbo].[Messages_SelectAll_By_UserId]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@senderId", userId);
                    paramCol.AddWithValue("@recipientId", recipientId);


                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    Message message = SingleMessageMapper(reader);
                    if (list == null)
                    {
                        list = new List<Message>();
                    }
                    list.Add(message);
                });
            return list;
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Messages_Delete_ById]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", id);
                },
                returnParameters: null);
        }

        public static void AddCommonParams(MessageAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Body", model.Body);
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@RecipientId", model.RecipientId);
            col.AddWithValue("@DateSent", model.DateSent);
            col.AddWithValue("@DateRead", DBNull.Value);
    
        }

        public static Message SingleMessageMapper(IDataReader reader)
        {
            Message message = new Message();
            int startingIndex = 0;
            message.Id = reader.GetSafeInt32(startingIndex++);
            message.Body = reader.GetSafeString(startingIndex++);
            message.Subject = reader.GetSafeString(startingIndex++);
            message.RecipientId = reader.GetSafeInt32(startingIndex++);
            message.SenderId = reader.GetSafeInt32(startingIndex++);
            message.DateSent = reader.GetSafeDateTime(startingIndex++);
            message.DateRead = reader.GetSafeDateTime(startingIndex++);            
            return message;
        }

    }
}
