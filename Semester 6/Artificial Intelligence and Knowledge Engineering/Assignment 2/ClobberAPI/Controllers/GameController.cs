using Microsoft.AspNetCore.Mvc;

namespace ClobberAPI.Controllers
{
    [ApiController]
    [Route("/")]
    public class GameController : ControllerBase
    {
        private static char[,] _board = new char[5, 5];
        private static char _currentPlayer = 'B';

        [HttpPost("setBoard/{rowN}/{colN}")]
        public IActionResult SetBoard(int rowN, int colN)
        {
            _board = new char[rowN, colN];

            char[] players = { 'W', 'B' };

            for (int y = 0; y < rowN; y++)
            {
                for (int x = 0; x < colN; x++)
                {
                    _board[y, x] = players[(x + (y % 2)) % 2];
                }
            }

            _currentPlayer = 'B';
            return Ok($"Board initialized to {rowN}x{colN}");
        }


        [HttpGet("boardState")]
        public ActionResult<string> GetBoardState()
        {
            int rows = _board.GetLength(0);
            int cols = _board.GetLength(1);
            var sb = new System.Text.StringBuilder();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    sb.Append(_board[y, x]);
                    if (x < cols - 1)
                        sb.Append(' ');
                }
                if (y < rows - 1)
                    sb.Append('\n');
            }

            return Ok(sb.ToString());
        }


        [HttpGet("currentPlayerId")]
        public ActionResult<int> GetCurrentPlayer()
        {
            return Ok(_currentPlayer);
        }

        [HttpPost("play/{player}/{fromX}/{fromY}/{toX}/{toY}")]
        public IActionResult MakeMove(char player, int fromX, int fromY, int toX, int toY)
        {
            if (player != _currentPlayer)
            {
                return BadRequest("Not your turn.");
            }
            
            _board[toY, toX] = _board[fromY, fromX];
            _board[fromY, fromX] = '_';

            _currentPlayer = _currentPlayer == 'B' ? 'W' : 'B';
            return Ok("Move applied.");
        }
    }
}
