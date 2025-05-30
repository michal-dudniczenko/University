import sys
import time
import argparse

from board_utils import generate_board, print_board, read_board
from common import PLAYER1, PLAYER2
from game_utils import apply_move, get_num_of_possible_moves, is_game_over
from heuristic_functions import heuristics
from algorithms import minimax, minimax_alpha_beta


# rozegranie gry clobber na podanej planszy z maksymalna glebokoscia przeszukiwania drzewa decyzyjnego
# z podanym graczem, ktory zaczyna oraz z wybrana funkcja heurystyki dla obu graczy
def play_game(start_player, start_board, max_depth, heuristic):
    current_board = start_board
    current_player = start_player
    round = 0
    node_count = [0]

    while (not is_game_over(current_board, current_player)):
        if current_player == start_player:
            round += 1

        # (_, best_move) = minimax(board=current_board, player=current_player, depth=max_depth, heuristic=heuristic, counter=node_count)
        (_, best_move) = minimax_alpha_beta(board=current_board, player=current_player, depth=max_depth, heuristic=heuristic, counter=node_count)

        # (fromX, fromY), (toX, toY) = best_move
        # if (current_player == PLAYER1):
        #     print(f"{current_player}: ({fromX+1}, {fromY+1}) -> ({toX+1}, {toY+1})")
        # else:
        #     print(f"\t{current_player}: ({fromX+1}, {fromY+1}) -> ({toX+1}, {toY+1})")
        
        current_board = apply_move(current_board, best_move)
        current_player = PLAYER2 if current_player == PLAYER1 else PLAYER1
    
    winner = PLAYER2 if current_player == PLAYER1 else PLAYER1

    return winner, round, current_board, node_count[0]


def main():
    parser = argparse.ArgumentParser(description="GRA: wybór głębokości i heurystyki")
    parser.add_argument('--depth', type=int, required=True, help='Maksymalna głębokość (>=1)')
    parser.add_argument('--heuristic', type=int, choices=range(1, 5), required=True, help='Wybór heurystyki (1-4)')
    args = parser.parse_args()

    if args.depth < 1:
        print("Błąd: Głębokość musi być większa lub równa 1.", file=sys.stderr)
        sys.exit(1)

    # generate_board(colN=5, rowN=6, isRandom=False)

    start_board = read_board()
    if (start_board == []):
        print("Błąd: Nie podano prawidłowej planszy na stdin.", file=sys.stderr)
        sys.exit(1)

    start_player = PLAYER1
    max_depth = args.depth
    heuristic = heuristics[args.heuristic]

    print("----------------------------------------------------------\n")

    start_time = time.perf_counter()

    winner, round, end_board, node_count = play_game(
        start_player = start_player,
        start_board=start_board,
        max_depth=max_depth,
        heuristic=heuristic
    )

    end_time = time.perf_counter()

    print_board(end_board)
    print(f"\nKONIEC GRY! Wygrywa gracz: {winner} w {round} rundzie.")

    print(f"liczba odwiedzonych węzłów: {node_count}", file=sys.stderr)
    print(f"Czas działania algorytmu: {end_time - start_time:.4f} sekund", file=sys.stderr)

        
if __name__ == '__main__':
    main()