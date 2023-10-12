using ChatServices.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServices
{
    public class GoodChatServices : IChatServices;
    {
        public ISqlDataAccess sql { get; set; }

        private readonly IHttpContextAccessor _contextAccessor;
        public GoodChatServices(ISqlDataAccess sql, IHttpContextAccessor accesor)
        {
            this.sql = sql;
            _contextAccessor = accesor;
        }

        public async Task AddChat(string user1, string user2)
        {
            await sql.SaveData("dbo.spCreate_NewChat", new { ChatId = Guid.NewGuid().ToString(), Email1 = user1, Email2 = user2 });
        }

        public Task<int> AddChatAndReturn(string user1, string user2)
        {
            throw new NotImplementedException();
        }         


        public async Task AddMessage(MessageModel message, int chat, AttachedFile file)
        {
            string? fileId = null;
            string? filePath = null;
            if (file != null)
            {
                fileId = file.id;
                filePath = file.Path;
            }
            if (!isGroup)
            {
                await sql.SaveData("dbo.spInsert_Messages", new { Email1 = message.Sender, Email2 = reciver, Content = message.Message, MessageId = Guid.NewGuid().ToString(), FileID = fileId, FilePath = filePath });
            }
            else
            {
                await sql.SaveData("dbo.spInsert_Messages", new { Email1 = message.Sender, GroupId = reciver, Content = message.Message, MessageId = Guid.NewGuid().ToString(), FileID = fileId, FilePath = filePath });
            }
        }
       
        public Task<int> GetChatIdentifier(string user1, string user2)
        {
            throw new NotImplementedException();
        }
      
        public async Task<List<MessageCustomModel>> GetMessages(int chat)
        {
            List<MessageCustomModel> returnList = new();
            if (!isGroup)
            {
                returnList = await sql.LoadData<MessageCustomModel>("dbo.spGet_ChatMessages", new { Email1 = user, Email2 = identifierOrUser });
            }
            else
            {
                returnList = await sql.LoadData<MessageCustomModel>("dbo.spGet_ChatMessages", new { GroupId = identifierOrUser });

            }
            return returnList;
        }
        public async Task<List<ChatViewModel>> GetAllChats(string user)
        {
            throw new NotImplementedException();
        }      
    }
}
