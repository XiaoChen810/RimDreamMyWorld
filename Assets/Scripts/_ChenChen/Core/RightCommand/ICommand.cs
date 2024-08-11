using ChenChen_AI;

namespace ChenChen_Core
{
    public interface ICommand
    {
        string CommandName { get; }

        void CommandFunc(Pawn p);
    }
}
