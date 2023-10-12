using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServices.Models;
using DataAccess;

namespace ChatServices
{
    public class ChatServices : IChatServices
    {
        public async Task AddChat(string user1, string user2)
        {
            using (var context = new Project1DBEntities())
            {
                var userF = context.Users.Select(x => new { x.Email, x.Id }).FirstOrDefault(x => x.Email == user1);
                var userS = context.Users.Select(x => new { x.Email, x.Id }).FirstOrDefault(x => x.Email == user2);
                Chat newChat = new Chat() { Messages = new List<Message>(), UserIdOne = userF.Id, UserIdTwo = userS.Id };
                context.Chats.Add(newChat);
                context.SaveChanges();
            }
        }

        public async Task<int> AddChatAndReturn(string user1, string user2)
        {
            using (var context = new Project1DBEntities())
            {
                var userF = context.Users.Select(x => new { x.Email, x.Id }).FirstOrDefault(x => x.Email == user1);
                var userS = context.Users.Select(x => new { x.Email, x.Id }).FirstOrDefault(x => x.Email == user2);
                Chat newChat = new Chat() { Messages = new List<Message>(), UserIdOne = userF.Id, UserIdTwo = userS.Id };
                context.Chats.Add(newChat);
                context.SaveChanges();
                return newChat.ChatId;
            }
        }
        public async Task AddMessage(MessageModel message, int chat, AttachedFile file)
        {
            using (var context = new Project1DBEntities())
            {
                int senderId = context.Users.Select(x => new { x.Email, x.Id }).First(x => x.Email == message.Sender).Id;
                Message newMessage = new Message()
                {
                    SenderId = senderId,
                    Content = message.Message,
                    Date = DateTime.Now,
                    ChatId = chat,
                };
                if (file != null)
                {
                    newMessage.AttachedFile = file;
                }

                context.Messages.Add(newMessage);


                context.SaveChanges();
            }
        }

        public async Task<int> GetChatIdentifier(string user1, string user2)
        {
            using (var context = new Project1DBEntities())
            {

                var chat = context.Chats.FirstOrDefault(x => (x.AspNetUser.Email == user1 || x.AspNetUser.Email == user2) && (x.AspNetUser1.Email == user1 || x.AspNetUser1.Email == user2)).ChatId;
                if (chat == 0)
                {
                    return await AddChatAndReturn(user1, user2);
                }
                return chat;
            }
        }

        public async Task<List<MessageCustomModel>> GetMessages(int chat)
        {
            using (var context = new Project1DBEntities())
            {
                List<MessageCustomModel> messagesToReturn = new List<MessageCustomModel>();
                dynamic messages;
                messages = context.Messages.Where(x => x.ChatId == chat).Select(x => new { Sender = x.AspNetUser.UserName, Path = x.FileId == null ? null : x.AttachedFile.FilePath, x.Content, x.Date }).OrderByDescending(x => x.Date).ToList();

                foreach (var mes in messages)
                {

                    messagesToReturn.Add(new MessageCustomModel(mes.Content, mes.Date, mes.Sender, mes.Path));
                }
                return messagesToReturn;
            }
        }
        public async Task<List<ChatViewModel>> GetAllChats(string user)
        {
            using(var context = new Project1DBEntities()) {
                int userid = context.Users.FirstOrDefault(x => x.UserName == user).Id;
                List<ChatViewModel> chats= context.Chats.Where(x => x.UserIdOne == userid || x.UserIdTwo == userid)
                    .Select(x => new ChatViewModel( x.UserIdOne == userid ? x.AspNetUser1.UserName: x.AspNetUser.UserName, DateTime.Now, 0)).ToList();
                return chats;

            }
        }
    }
}
