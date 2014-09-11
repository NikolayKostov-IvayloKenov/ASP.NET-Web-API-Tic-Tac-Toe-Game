namespace TicTacToe.Models
{
    public enum GameState
    {
        WaitingForSecondPlayer = 0,
        TurnX = 1,
        TurnO = 2,
        GameWonByX = 3,
        GameWonByO = 4,
        GameDraw = 5,
    }
}
