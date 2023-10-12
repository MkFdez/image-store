using ChatServicesII.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;

namespace ChatServicesII
{
    public class GoodChatServices : IChatServices
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
            string fileId = null;
            string filePath = null;
            if (file != null)
            {
                fileId = Guid.NewGuid().ToString() ;
                filePath = file.FilePath;
            }
           
                await sql.SaveData("dbo.spInsert_Messages", new { Email1 = message.Sender, Chat = chat, Content = message.Message, FileID = fileId, FilePath = filePath });
           
               
           
        }
       
        public async Task<int> GetChatIdentifier(string user1, string user2)
        {
            var result = await sql.LoadData<ChatID>("dbo.spGet_ChatID", new { User1 = user1, User2 = user2 });
            if(result.Count() == 0)
            {
                await AddChat(user1, user2);
                result = await sql.LoadData<ChatID>("dbo.spGet_ChatID", new { User1 = user1, User2 = user2 });
            }
            return result.First().ChatId;
        }
      
        public async Task<List<MessageCustomModel>> GetMessages(int chat)
        {
            List<MessageCustomModel> returnList = new();
           
                returnList = await sql.LoadData<MessageCustomModel>("dbo.spGet_ChatMessages", new { ChatID = chat });
           
           
            return returnList;
        }
        public async Task<List<ChatViewModel>> GetAllChats(string user)
        {
            List<ChatViewModel> returnList = new();
            returnList = await sql.LoadData<ChatViewModel>("dbo.spGet_AllChats", new { User = user });
            return returnList;
        }      

        public async Task<string> GetUsername(string token)
        {
            var model = await sql.LoadData<GetUsernameModel>("dbo.spGet_UsernameByToken", new {Token = token });  
            return model.First().Username;
        }
    }
}
