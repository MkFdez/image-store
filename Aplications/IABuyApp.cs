using Dependencies;
namespace Aplications
{
    public interface IBuyApp
    {
        IEmailSender Model { get; set; }
        void Run();
    }
}