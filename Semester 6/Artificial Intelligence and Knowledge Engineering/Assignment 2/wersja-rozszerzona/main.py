import sys
import time
import argparse

import requests

from board_utils import set_board, print_board, read_board
from common import API_ADDRESS, BOARD_COL_N, BOARD_ROW_N, PLAYER1, PLAYER2
from game_utils import apply_move, get_num_of_possible_moves, is_game_over
from heuristic_functions import heuristics
from algorithms import minimax, minimax_alpha_beta


# rozegranie gry clobber na podanej planszy z maksymalna glebokoscia przeszukiwania drzewa decyzyjnego
# z podanym graczem, ktory zaczyna oraz z wybrana funkcja heurystyki dla obu graczy
def play_game(player, max_depth, heuristic):
    round = 0
    node_count = [0]
    isWaiting = False
    while True:
        current_player = requests.get(f"{API_ADDRESS}/currentPlayerId").text.strip('"')
        current_board = read_board()

        if (current_player == player):
            round += 1

        if (is_game_over(current_board, current_player)):
            winner = PLAYER2 if current_player == PLAYER1 else PLAYER1
            return winner, round, current_board, node_count[0]

        if (current_player != player):
            if (not isWaiting):
                print('waiting for turn...')
                isWaiting = True
            time.sleep(1)
            continue

        isWaiting = False
        
        (_, best_move) = minimax_alpha_beta(board=current_board, player=player, depth=max_depth, heuristic=heuristic, counter=node_count)
        (fromX, fromY), (toX, toY) = best_move
        requests.post(f"{API_ADDRESS}/play/{player}/{fromX}/{fromY}/{toX}/{toY}")
        print(f"MOVE {current_player}: ({fromX+1}, {fromY+1}) -> ({toX+1}, {toY+1})")
        time.sleep(1)




def main():
    parser = argparse.ArgumentParser(description="GRA: wybór głębokości i heurystyki")
    parser.add_argument('--depth', type=int, required=True, help='Maksymalna głębokość (>=1)')
    parser.add_argument('--heuristic', type=int, choices=range(1, 6), required=True, help='Wybór heurystyki (1-5)')
    parser.add_argument('--playerId', type=int, choices=range(1, 3), required=True, help='ID gracza 1/2')
    args = parser.parse_args()

    if args.depth < 1:
        print("Błąd: Głębokość musi być większa lub równa 1.", file=sys.stderr)
        sys.exit(1)

    player_id = args.playerId

    if (player_id == 1):
        set_board(rowN=BOARD_ROW_N, colN=BOARD_COL_N)
    
    player = PLAYER1 if player_id == 1 else PLAYER2
    max_depth = args.depth
    heuristic = heuristics[args.heuristic]

    print("----------------------------------------------------------\n")

    winner, round, end_board, node_count = play_game(
        player=player,
        max_depth=max_depth,
        heuristic=heuristic
    )

    print_board(end_board)
    print(f"\nKONIEC GRY! Wygrywa gracz: {winner} w {round} rundzie.")

    print(f"liczba odwiedzonych węzłów: {node_count}", file=sys.stderr)

        
if __name__ == '__main__':
    main()