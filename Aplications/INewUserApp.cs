using Dependencies;

namespace Aplications
{
    public interface INewUserApp
    {
        IEmailModel Model { get; set; }

        void Run();
    }
}