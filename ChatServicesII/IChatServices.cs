using ChatServicesII.Models;
using DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServicesII
{
    public interface IChatServices
    {
        Task AddChat(string user1, string user2);
        Task<int> AddChatAndReturn(string user1, string user2);
        Task AddMessage(MessageModel message, int chat, AttachedFile file);
        Task<int> GetChatIdentifier(string user1, string user2);
        Task<List<MessageCustomModel>> GetMessages(int chat);
        Task<List<ChatViewModel>> GetAllChats(string user);
        Task<string> GetUsername(string token);
        Task BlockUser(string blocker, string blocked);
        Task UnblockUser(string blocker, string blocked);
        Task ReportUser(string reporter, string reported, int reason, string message);
        Task<List<Report_Reasons>> GetReportReason();
    }
}