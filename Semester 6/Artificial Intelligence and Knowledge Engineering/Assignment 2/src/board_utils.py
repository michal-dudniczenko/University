import random
import sys
from common import EMPTY, PLAYER1, PLAYER2


# generowanie planszy do pliku board.txt
def generate_board(colN, rowN, isRandom=False):
    players = ['W', 'B']
    with open('board.txt', 'w') as file:
        if (not isRandom):
            for i in range(rowN):
                row = ''
                for col in range(colN):
                    row += players[(col + (i % 2)) % 2]
                    if (col < colN-1):
                        row += ' '
                if (i < rowN-1): 
                    row += '\n'
                file.write(row)
        else:
            all_options = ['W', 'B', '_']
            for i in range(rowN):
                row = ''
                for col in range(colN):
                    row += random.choice(all_options)
                    if (col < colN-1):
                        row += ' '
                if (i < rowN-1): 
                    row += '\n'
                file.write(row)


# odczytanie planszy z stdin
def read_board():
    if sys.stdin.isatty():
        return []
    
    input_text = sys.stdin.read().strip()

    board_rows = []

    for line in input_text.splitlines():
        board_rows.append(line.strip().split(' '))
        
    if (not is_board_valid(boardRows=board_rows)):
        return []
    
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