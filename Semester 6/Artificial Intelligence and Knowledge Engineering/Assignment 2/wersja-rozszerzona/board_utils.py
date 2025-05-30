import requests
from common import API_ADDRESS, EMPTY, PLAYER1, PLAYER2


# generowanie planszy
def set_board(rowN, colN):
    requests.post(f"{API_ADDRESS}/setBoard/{rowN}/{colN}")


# odczytanie planszy
def read_board():
    board_string = requests.get(f"{API_ADDRESS}/boardState").text

    board_rows = []
    for line in board_string.strip().split('\n'):
        board_rows.append(line.strip().split(' '))
    
    board = []
    colN = len(board_rows[0])
    for _ in range(colN):
        board.append([])
    
    for i in range(colN):
        for row in board_rows:
            board[i].append(row[i])

    return board


# walidacja planszy
def is_board_valid(boardRows):
    if (boardRows == []): return False

    colN = len(boardRows[0])
    for line in boardRows:
        if (len(line)) != colN:
            return False
        for element in line:
            if (element not in [PLAYER1, PLAYER2, EMPTY]):
                return False
    return True


# wypisanie reprezentacji planszy na stdin
def print_board(board):
    rowN = len(board[0])
    colN = len(board)
    for y in range(rowN): 
        line = ""
        for x in range(colN): 
            line += board[x][y]
            if x < colN - 1:
                line += ' '
        print(line)


# sprawdzenie czy pole jest w obrebie planszy
def is_inside(rowN, colN, x, y):
    return 0 <= x < colN and 0 <= y < rowN