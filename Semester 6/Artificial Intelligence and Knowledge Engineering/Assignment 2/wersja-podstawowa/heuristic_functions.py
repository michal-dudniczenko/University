from common import PLAYER1, PLAYER2
from game_utils import get_num_of_possible_moves


# czym mamy wiecej dostepnych ruchow tym lepiej
def maximize_self_moves(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    self_moves = get_num_of_possible_moves(board, player)
    opponent_moves = get_num_of_possible_moves(board, opponent)

    if opponent_moves == 0: return 1_000_000
    elif self_moves == 0: return -1_000_000
    else: return self_moves

# czym przeciwnik ma mniej dostepnych ruchow tym lepiej
def minimize_opponent_moves(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    self_moves = get_num_of_possible_moves(board, player)
    opponent_moves = get_num_of_possible_moves(board, opponent)

    if opponent_moves == 0: return 1_000_000
    elif self_moves == 0: return -1_000_000
    else: return opponent_moves * (-1)

# czym roznica miedzy nasza iloscia ruchow a iloscia ruchow dostepnych dla przeciwnika wieksza tym lepiej
def maximize_self_opponent_delta(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    self_moves = get_num_of_possible_moves(board, player)
    opponent_moves = get_num_of_possible_moves(board, opponent)

    if opponent_moves == 0: return 1_000_000
    elif self_moves == 0: return -1_000_000
    else: return self_moves - opponent_moves

# czym mamy wiecej pionkow na planszy tym lepiej
def maximize_self_pieces(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    self_moves = get_num_of_possible_moves(board, player)
    opponent_moves = get_num_of_possible_moves(board, opponent)

    if opponent_moves == 0: return 1_000_000
    elif self_moves == 0: return -1_000_000
    else: return sum(column.count(player) for column in board)

heuristics = {
    1: maximize_self_moves,
    2: minimize_opponent_moves,
    3: maximize_self_opponent_delta,
    4: maximize_self_pieces,
}