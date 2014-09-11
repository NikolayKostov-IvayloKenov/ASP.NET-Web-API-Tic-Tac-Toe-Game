namespace TicTacToe.Web.DataModels
{
    using System;

    using AutoMapper;

    using TicTacToe.Models;
    using TicTacToe.Web.Mapping;

    public class GameInfoDataModel : IMappableFrom<Game>, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string FirstPlayerName { get; set; }

        public string SecondPlayerName { get; set; }

        public string Board { get; set; }

        public string State { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Game, GameInfoDataModel>()
                   .ForMember(m => m.FirstPlayerName, opt => opt.MapFrom(game => game.FirstPlayer.UserName))
                   .ForMember(m => m.SecondPlayerName, opt => opt.MapFrom(game => game.SecondPlayer == null ? null : game.SecondPlayer.UserName))
                   .ForMember(m => m.State, opt => opt.MapFrom(game => game.State.ToString()));
        }
    }
}