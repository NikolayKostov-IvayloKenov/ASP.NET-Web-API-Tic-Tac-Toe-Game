namespace TicTacToe.Web.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using System.Web.Http;

    using Antlr.Runtime.Tree;

    using Microsoft.AspNet.Identity;

    using TicTacToe.Data;
    using TicTacToe.Models;
    using TicTacToe.Web.DataModels;

    [Authorize]
    public class GameController : ApiController
    {
        private readonly ITicTacToeData data;

        public GameController(ITicTacToeData data)
        {
            this.data = data;
        }

        [HttpPost]
        public IHttpActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var game = new Game { FirstPlayerId = userId };
            this.data.Games.Add(game);
            this.data.SaveChanges();

            var gameDataModel =
                this.data.Games.All()
                    .Where(x => x.Id == game.Id)
                    .Select(
                        x =>
                        new GameInfoDataModel
                            {
                                Id = x.Id,
                                Board = x.Board,
                                FirstPlayerName = x.FirstPlayer.UserName,
                                SecondPlayerName =
                                    x.SecondPlayer == null ? null : x.SecondPlayer.UserName,
                                State = x.State.ToString(),
                            })
                    .FirstOrDefault();

            return this.Ok(gameDataModel);
        }

        [HttpPost]
        public IHttpActionResult Join()
        {
            var userId = User.Identity.GetUserId();

            var firstAvailableGame =
                this.data.Games.All()
                    .FirstOrDefault(x => x.State == GameState.WaitingForSecondPlayer && x.FirstPlayerId != userId);

            if (firstAvailableGame == null)
            {
                return this.NotFound();
            }

            firstAvailableGame.SecondPlayerId = userId;
            firstAvailableGame.State = GameState.TurnX;
            this.data.SaveChanges();

            var gameDataModel =
                this.data.Games.All()
                    .Where(x => x.Id == firstAvailableGame.Id)
                    .Select(
                        x =>
                        new GameInfoDataModel
                            {
                                Id = x.Id,
                                Board = x.Board,
                                FirstPlayerName = x.FirstPlayer.UserName,
                                SecondPlayerName =
                                    x.SecondPlayer == null ? null : x.SecondPlayer.UserName,
                                State = x.State.ToString(),
                            })
                    .FirstOrDefault();

            return this.Ok(gameDataModel);
        }

        [HttpGet]
        public IHttpActionResult Status([Required]string gameId)
        {
            var userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var gameIdAsGuid = new Guid(gameId);
            var gameDataModel =
                this.data.Games.All()
                    .Where(x => x.Id == gameIdAsGuid && (x.FirstPlayerId == userId || x.SecondPlayerId == userId))
                    .Select(
                        x =>
                        new GameInfoDataModel
                            {
                                Id = x.Id,
                                Board = x.Board,
                                FirstPlayerName = x.FirstPlayer.UserName,
                                SecondPlayerName =
                                    x.SecondPlayer == null ? null : x.SecondPlayer.UserName,
                                State = x.State.ToString(),
                            })
                    .FirstOrDefault();

            if (gameDataModel == null)
            {
                return this.NotFound();
            }

            return this.Ok(gameDataModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="row">1, 2 or 3</param>
        /// <param name="col">1, 2 or 3</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Play([Required]string gameId, [Range(1, 3)]int row, [Range(1, 3)]int col)
        {
            var userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var gameIdAsGuid = new Guid(gameId);
            var game =
                this.data.Games.All()
                    .FirstOrDefault(
                        x => x.Id == gameIdAsGuid && (x.FirstPlayerId == userId || x.SecondPlayerId == userId));

            if (game == null)
            {
                return this.NotFound();
            }

            if (game.State != GameState.TurnX && game.State != GameState.TurnO)
            {
                return this.BadRequest(string.Format("Invalid game state: {0}", game.State));
            }

            if ((game.State == GameState.TurnX && game.FirstPlayerId != userId)
                || (game.State == GameState.TurnO && game.SecondPlayerId != userId))
            {
                return this.BadRequest(string.Format("It's not your turn!"));
            }

            int positionIndex = ((row - 1) * 3) + col - 1;
            if (game.Board[positionIndex] != '-')
            {
                return this.BadRequest("This position is already taken. Please chose different one.");
            }

            var board = new StringBuilder(game.Board);
            board[positionIndex] = game.State == GameState.TurnX ? 'X' : 'O';
            var boardAsString = board.ToString();
            game.Board = boardAsString;
            game.State = game.State == GameState.TurnX ? GameState.TurnO : GameState.TurnX;

            var gameResult = this.CheckGameResult(game.Board);
            switch (gameResult)
            {
                case GameResult.XWins:
                    game.State = GameState.GameWonByX;
                    break;
                case GameResult.OWins:
                    game.State = GameState.GameWonByO;
                    break;
                case GameResult.Draw:
                    game.State = GameState.GameDraw;
                    break;
            }

            this.data.SaveChanges();

            var gameDataModel =
                this.data.Games.All()
                    .Where(x => x.Id == gameIdAsGuid && (x.FirstPlayerId == userId || x.SecondPlayerId == userId))
                    .Select(
                        x =>
                        new GameInfoDataModel
                        {
                            Id = x.Id,
                            Board = x.Board,
                            FirstPlayerName = x.FirstPlayer.UserName,
                            SecondPlayerName =
                                x.SecondPlayer == null ? null : x.SecondPlayer.UserName,
                            State = x.State.ToString(),
                        })
                    .FirstOrDefault();

            return this.Ok(gameDataModel);
        }

        private GameResult CheckGameResult(string boardAsString)
        {
            var board = new char[3, 3];
            for (int i = 0; i < boardAsString.Length; i++)
            {
                int row = i / 3;
                int col = i % 3;
                board[i / 3, i % 3] = boardAsString[i];
            }

            //// TODO: Implement Check if the game is won by X or O player

            if (!boardAsString.Contains('-'))
            {
                return GameResult.Draw;
            }
            else
            {
                return GameResult.NotFinished;
            }
        }
    }
}
