import copy
from board_utils import is_inside
from common import EMPTY, PLAYER1, PLAYER2


# pomocnicze reprezentacje kierunkow, przy sprawdzaniu mozliwych ruchow
directions = [
    # (col, row = x,y)

    # left
    (-1, 0),

    # top
    (0, -1), 
    
    # right
    (1, 0),

    # bottom
    (0, 1)
]


# generowanie listy mozliwych ruchow dla danego gracza
def get_possible_moves(board, player):
    enemy = PLAYER2 if (player == PLAYER1) else PLAYER1
    rowN = len(board[0])
    colN = len(board)
    moves = []
    for y in range(rowN):
        for x in range(colN):
            if board[x][y] == player:
                for direction in directions:
                    testX = x + direction[0]
                    testY = y + direction[1]
                    if (is_inside(rowN=rowN, colN=colN, x=testX, y=testY)):
                        if (board[testX][testY] == enemy):
                            # [(fromX, fromY), (toX, toY)]
                            moves.append([(x, y), (testX, testY)])
    return moves


# sprawdzenie liczby mozliwych ruchow dla danego gracza
def get_num_of_possible_moves(board, player):
    enemy = PLAYER2 if (player == PLAYER1) else PLAYER1
    rowN = len(board[0])
    colN = len(board)
    moves = 0
    for y in range(rowN):
        for x in range(colN):
            if board[x][y] == player:
                for direction in directions:
                    testX = x + direction[0]
                    testY = y + direction[1]
                    if (is_inside(rowN=rowN, colN=colN, x=testX, y=testY)):
                        if (board[testX][testY] == enemy):
                            # [(fromX, fromY), (toX, toY)]
                            moves += 1
    return moves


# wygenerowanie stanu planszy po wykonaniu ruchu
def apply_move(board, move):
    (fromX, fromY), (toX, toY) = move
    newBoard = copy.deepcopy(board)
    newBoard[toX][toY] = board[fromX][fromY]
    newBoard[fromX][fromY] = EMPTY

    return newBoard


# sprawdzenie czy gra dobiegla konca - czy gracz nie ma juz zadnych dostepnych ruchow
def is_game_over(board, player):
    return get_num_of_possible_moves(board=board, player=player) == 0
        