from game_utils import apply_move, get_num_of_possible_moves, get_possible_moves
from common import PLAYER1, PLAYER2


def minimax(
        board, 
        player, 
        depth, 
        heuristic, 
        counter, 
        maximizing_player=True
):
    # licznik odwiedzonych wezlow
    counter[0] += 1

    # jezeli limit glebokosci, zwroc ewaluacje pozycji
    if depth == 0:
        return heuristic(board, player), None
    
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    current_player = player if maximizing_player else opponent
    possible_moves = get_possible_moves(board=board, player=current_player)
    
    # zabezpieczenie, jezeli nie ma dostepnych ruchow to zwroc ewaluacje
    if len(possible_moves) == 0:
        return heuristic(board, player), None
    
    best_move = None

    # jezeli badamy sytuacje z perspektywy gracza
    if maximizing_player:
        max_eval = float('-inf')
        # dla kazdego mozliwego ruchu
        for move in possible_moves:
            # zasymuluj ten ruch
            new_board = apply_move(board=board, move=move)
            # i rekurencyjnie ewaluuj nowo otrzymana pozycje
            eval, _ = minimax(
                board=new_board, 
                player=player, 
                depth=depth - 1, 
                heuristic=heuristic, 
                counter=counter, 
                maximizing_player=not maximizing_player
            )
            # jezeli znalezlismy lepsza opcje to zapiszmy ja
            if eval > max_eval:
                max_eval = eval
                best_move = move
        # zwracamy ewaluacje najlepszego ruchu i ten ruch
        return max_eval, best_move
    # z perspektywy przeciwnika analogicznie jak wyzej
    else:
        min_eval = float('inf')
        for move in possible_moves:
            new_board = apply_move(board=board, move=move)
            eval, _ = minimax(
                board=new_board, 
                player=player, 
                depth=depth - 1, 
                heuristic=heuristic, 
                counter=counter, 
                maximizing_player=not maximizing_player
            )
            if eval < min_eval:
                min_eval = eval
                best_move = move
        return min_eval, best_move


def minimax_alpha_beta(
    board, 
    player, 
    depth, 
    heuristic, 
    counter, 
    maximizing_player=True, 
    alpha=float('-inf'), 
    beta=float('inf')
):
    # licznik odwiedzonych wezlow
    counter[0] += 1

    # jezeli limit glebokosci, zwroc ewaluacje pozycji
    if depth == 0:
        return heuristic(board, player), None
    
    opponent = PLAYER1 if player == PLAYER2 else PLAYER2
    current_player = player if maximizing_player else opponent
    possible_moves = get_possible_moves(board=board, player=current_player)
    
    # zabezpieczenie, jezeli nie ma dostepnych ruchow to zwroc ewaluacje
    if len(possible_moves) == 0:
        return heuristic(board, player), None
    
    best_move = None

    # jezeli badamy sytuacje z perspektywy gracza
    if maximizing_player:
        max_eval = float('-inf')
        # dla kazdego mozliwego ruchu
        for move in possible_moves:
            # zasymuluj ten ruch
            new_board = apply_move(board=board, move=move)
            # i rekurencyjnie ewaluuj nowo otrzymana pozycje
            eval, _ = minimax_alpha_beta(
                board=new_board, 
                player=player, 
                depth=depth - 1, 
                heuristic=heuristic, 
                counter=counter, 
                maximizing_player=not maximizing_player,
                alpha=alpha,
                beta=beta
            )
            # jezeli znalezlismy lepsza opcje to zapiszmy ja
            if eval > max_eval:
                max_eval = eval
                best_move = move
            # jezeli znalezlismy nowa najlepsza "globalna" opcje, czyli w obrebie analizowanego poddrzewa
            # to aktualizujemy alpha
            alpha = max(alpha, eval)
            # jezeli w analizowanym poddrzewie przeciwnik ma mozliwosc wyboru czegokolwiek
            # co bedzie gorsze dla nas niz obecna najlepsza "globalna" opcja alpha
            # to nie ma sensu analizowanie tego poddrzewa bo i tak go nie wybierzemy
            # skoro znamy juz lepsza alternatywe (ta ktora zwiazana jest z wartoscia alpha)
            if beta <= alpha:
                break
        # zwracamy ewaluacje najlepszego ruchu i ten ruch
        return max_eval, best_move
    # z perspektywy przeciwnika analogicznie jak wyzej
    else:
        min_eval = float('inf')
        for move in possible_moves:
            new_board = apply_move(board=board, move=move)
            eval, _ = minimax_alpha_beta(
                board=new_board, 
                player=player, 
                depth=depth - 1, 
                heuristic=heuristic, 
                counter=counter, 
                maximizing_player=not maximizing_player,
                alpha=alpha,
                beta=beta
            )
            if eval < min_eval:
                min_eval = eval
                best_move = move
            beta = min(beta, eval)
            if beta <= alpha:
                break
        return min_eval, best_move
