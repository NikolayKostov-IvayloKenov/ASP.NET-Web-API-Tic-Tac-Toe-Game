namespace TicTacToe.Data
{
    using TicTacToe.Models;

    public interface ITicTacToeData
    {
        IRepository<ApplicationUser> Users { get; }

        IRepository<Game> Games { get; }

        int SaveChanges();
    }
}
