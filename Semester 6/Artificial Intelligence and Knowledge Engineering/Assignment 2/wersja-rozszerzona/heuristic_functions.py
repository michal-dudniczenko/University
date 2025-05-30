from common import EMPTY, PLAYER1, PLAYER2
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


# czym mamy wiecej pionkow w centrum tym lepiej
def maximize_central_pieces(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    self_moves = get_num_of_possible_moves(board, player)
    opponent_moves = get_num_of_possible_moves(board, opponent)

    if opponent_moves == 0: return 1_000_000
    elif self_moves == 0: return -1_000_000
    
    rowN = len(board[0])
    colN = len(board)

    central_pieces = 0

    for col in range(1, colN - 1):
        for row in range(1, rowN - 1):
            if board[col][row] == player:
                central_pieces += 1
    
    return central_pieces

def adaptive_heuristic(board, player):
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    rowN = len(board[0])
    colN = len(board)
    start_pieces = rowN * colN
    current_pieces = sum(cell != EMPTY for column in board for cell in column)
    player_pieces = sum(column.count(player) for column in board)
    opponent_pieces = sum(column.count(opponent) for column in board)
    pieces_ratio = current_pieces / start_pieces

    # early game, wiekszosc pionkow dalej na planszy
    if (pieces_ratio > 0.8):
        # walczymy o centrum planszy
        return maximize_central_pieces(board, player)
    # end game, malo pionkow na planszy
    elif (pieces_ratio < 0.2):
        # staramy sie wykonczyc przeciwnika
        return minimize_opponent_moves(board, player)
    # gra środkowa - mamy przewagę
    elif (player_pieces > opponent_pieces):
        # staramy sie utrzymac jak najwieksza przewage nad przeciwnikiem
        return maximize_self_opponent_delta(board, player)
    # gra środkowa - przegrywamy
    else:
        # staramy sie uratowac sytuacje i przetrwac, 
        return maximize_self_moves(board, player)

heuristics = {
    1: maximize_self_moves,
    2: minimize_opponent_moves,
    3: maximize_self_opponent_delta,
    4: maximize_self_pieces,
    5: adaptive_heuristic
}