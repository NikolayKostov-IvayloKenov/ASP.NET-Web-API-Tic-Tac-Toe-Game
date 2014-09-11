namespace TicTacToe.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Game
    {
        public Game()
        {
            this.Id = Guid.NewGuid();
            this.Board = "---------";
            this.State = GameState.WaitingForSecondPlayer;
        }

        [Key]
        public Guid Id { get; set; }

        [StringLength(9)]
        [Column(TypeName = "char")]
        public string Board { get; set; }

        public GameState State { get; set; }

        [Required]
        public string FirstPlayerId { get; set; }

        /// <summary>
        /// The 'X' player
        /// </summary>
        public ApplicationUser FirstPlayer { get; set; }

        public string SecondPlayerId { get; set; }

        /// <summary>
        /// The 'O' player
        /// </summary>
        public ApplicationUser SecondPlayer { get; set; }
    }
}
