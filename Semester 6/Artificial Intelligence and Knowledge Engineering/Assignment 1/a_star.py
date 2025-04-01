from utils import get_edges, get_graph, minutes_between, calculate_distance, backtrack_path, print_path, Node

import time
import sys
import heapq
from datetime import datetime


def a_star_time(graph, start_vertex_name, end_vertex_name, start_time):
    open = dict()
    closed = dict()

    # Dodaję do open węzeł startowy
    for vertex in graph.keys():
        if vertex.name == start_vertex_name:
            open[vertex] = Node (
                vertex=vertex
            )    

    while open:
        # Pobieram z open węzeł o najmniejszym f
        _, current_node = min(open.items(), key=lambda item: item[1].f)

        # Koniec jeżeli pobrany węzeł jest węzłem docelowym, nie ma innej lepszej potencjalnej alternatywy
        if current_node.vertex.name == end_vertex_name:
            return current_node
        
        # Przenoszę węzeł z open do closed
        closed[current_node.vertex] = current_node
        open.pop(current_node.vertex)

        if current_node.parent is None:
            # Logika dla węzła startowego, uwzględnia czas pojawienia się na przystanku
            if current_node.vertex.name == start_vertex_name:
                # Rozpatruję wszystkie krawędzie z węzła startowego
                for edge in graph[current_node.vertex]:
                    neighbor = edge.end_vertex

                    # Jeżeli sąsiad był już sprawdzony to pomijamy
                    if neighbor in closed.keys():
                        continue
                    
                    # Przy obliczaniu uwzględniam czas od pojawienia się na przystanku do odjazdu
                    neighbor_g = minutes_between(start_time, edge.departure_time) + edge.travel_time

                    # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
                    if neighbor not in open.keys():
                        new_node = Node(
                            vertex=neighbor,
                            parent=current_node,
                            parent_edge=edge,
                            g=neighbor_g,
                            h=calculate_distance(current_node.vertex, neighbor)
                        )
                        open[neighbor] = new_node
                    # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie jest szybsze od obecnego 
                    # tutaj nie sprawdzam pod kątem eliminowania przesiadki bo jest to dopiero pierwsze połączenie
                    elif neighbor_g < open[neighbor].g:
                        open[neighbor].set_g(neighbor_g)
                        open[neighbor].parent = current_node
                        open[neighbor].parent_edge = edge
                continue
            else:
                # Węzły do których nie da się dojechać pomijamy
                continue
        
        # Dla każdej krawędzi wychodzącej z obecnego węzła
        for edge in graph[current_node.vertex]:
            neighbor = edge.end_vertex

            # Jeżeli sąsiad był już sprawdzony to pomijamy
            if neighbor in closed.keys():
                continue
            
            # Obliczamy czas dotarcia z obecnego węzła do sąsiada
            neighbor_g = current_node.g + minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) + edge.travel_time

            # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
            if neighbor not in open.keys():
                new_node = Node(
                    vertex=neighbor,
                    parent=current_node,
                    parent_edge=edge,
                    g=neighbor_g,
                    h=calculate_distance(current_node.vertex, neighbor)
                )
                open[neighbor] = new_node
            # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie jest szybsze od obecnego 
            # lub jest tak samo szybkie, ale eliminuje konieczność przesiadki
            elif neighbor_g < open[neighbor].g or (neighbor_g == open[neighbor].g and edge.line == current_node.parent_edge.line):
                open[neighbor].set_g(neighbor_g)
                open[neighbor].parent = current_node
                open[neighbor].parent_edge = edge
    return None


def a_star_time_faster(graph, start_vertex_name, end_vertex_name, start_time):
    open = dict()
    closed = dict()

    # Zastosuję kolejkę priorytetową żeby zmniejszyć czas szukania minimum z O(n) do O(logn)
    open_heap = []

    # Dodaję do open węzeł startowy
    for vertex in graph.keys():
        if vertex.name == start_vertex_name:
            start_node = Node (
                vertex=vertex
            )    
            open[vertex] = start_node
            heapq.heappush(open_heap, start_node)   

    while open:
        # Pobieram z open węzeł o najmniejszym f
        current_node = heapq.heappop(open_heap)

        # Koniec jeżeli pobrany węzeł jest węzłem docelowym, nie ma innej lepszej potencjalnej alternatywy
        if current_node.vertex.name == end_vertex_name:
            return current_node
        
        # Przenoszę węzeł z open do closed
        closed[current_node.vertex] = current_node
        open.pop(current_node.vertex)

        if current_node.parent is None:
            # Logika dla węzła startowego, uwzględnia czas pojawienia się na przystanku
            if current_node.vertex.name == start_vertex_name:
                # Rozpatruję wszystkie krawędzie z węzła startowego
                for edge in graph[current_node.vertex]:
                    neighbor = edge.end_vertex

                    # Jeżeli sąsiad był już sprawdzony to pomijamy
                    if neighbor in closed.keys():
                        continue
                    
                    # Przy obliczaniu uwzględniam czas od pojawienia się na przystanku do odjazdu
                    neighbor_g = minutes_between(start_time, edge.departure_time) + edge.travel_time
                    
                    # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
                    if neighbor not in open.keys():
                        new_node = Node(
                            vertex=neighbor,
                            parent=current_node,
                            parent_edge=edge,
                            g=neighbor_g,
                            h=calculate_distance(current_node.vertex, neighbor)
                        )
                        open[neighbor] = new_node
                        heapq.heappush(open_heap, new_node)
                    # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie jest szybsze od obecnego 
                    # tutaj nie sprawdzam pod kątem eliminowania przesiadki bo jest to dopiero pierwsze połączenie
                    elif neighbor_g < open[neighbor].g:
                        open[neighbor].set_g(neighbor_g)
                        open[neighbor].parent = current_node
                        open[neighbor].parent_edge = edge
                continue
            else:
                # Węzły do których nie da się dojechać pomijamy
                continue
        
        # Dla każdej krawędzi wychodzącej z obecnego węzła
        for edge in graph[current_node.vertex]:
            neighbor = edge.end_vertex

            # Jeżeli sąsiad był już sprawdzony to pomijamy
            if neighbor in closed.keys():
                continue
            
            # Obliczamy czas dotarcia z obecnego węzła do sąsiada
            neighbor_g = current_node.g + minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) + edge.travel_time

            # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
            if neighbor not in open.keys():
                new_node = Node(
                    vertex=neighbor,
                    parent=current_node,
                    parent_edge=edge,
                    g=neighbor_g,
                    h=calculate_distance(current_node.vertex, neighbor)
                )
                open[neighbor] = new_node
                heapq.heappush(open_heap, new_node)
            # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie jest szybsze od obecnego 
            # lub jest tak samo szybkie, ale eliminuje konieczność przesiadki
            elif neighbor_g < open[neighbor].g or (neighbor_g == open[neighbor].g and edge.line == current_node.parent_edge.line):
                open[neighbor].set_g(neighbor_g)
                open[neighbor].parent = current_node
                open[neighbor].parent_edge = edge
    return None
                

def a_star_change(graph, start_vertex_name, end_vertex_name, start_time):
    open = dict()
    closed = dict()

    # Dodaję do open węzeł startowy
    for vertex in graph.keys():
        if vertex.name == start_vertex_name:
            open[vertex] = Node (
                vertex=vertex
            )    

    while open:
        # Pobieram z open węzeł o najmniejszym f
        _, current_node = min(open.items(), key=lambda item: item[1].f)

        # Koniec jeżeli pobrany węzeł jest węzłem docelowym, nie ma innej lepszej potencjalnej alternatywy
        if current_node.vertex.name == end_vertex_name:
            return current_node
        
        # Przenoszę węzeł z open do closed
        closed[current_node.vertex] = current_node
        open.pop(current_node.vertex)

        if current_node.parent is None:
            # Logika dla węzła startowego
            if current_node.vertex.name == start_vertex_name:
                # Rozpatruję wszystkie krawędzie z węzła startowego
                for edge in graph[current_node.vertex]:
                    neighbor = edge.end_vertex

                    # Jeżeli sąsiad był już sprawdzony to pomijamy
                    if neighbor in closed.keys():
                        continue
                    
                    # Dla węzła startowego i tak musimy się w coś "przesiąść"
                    neighbor_g = 1

                    # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
                    if neighbor not in open.keys():
                        new_node = Node(
                            vertex=neighbor,
                            parent=current_node,
                            parent_edge=edge,
                            g=neighbor_g,
                            h=calculate_distance(current_node.vertex, neighbor)
                        )
                        open[neighbor] = new_node
                    # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie ma mniej przesiadek
                    # od poprzedniego, jeżeli tyle samo to wybieramy to, które jest szybsze czasowo
                    elif neighbor_g < open[neighbor].g or \
                        (neighbor_g == open[neighbor].g and (minutes_between(start_time, edge.departure_time) + edge.travel_time) \
                        < minutes_between(start_time, open[neighbor].parent_edge.departure_time) + open[neighbor].parent_edge.travel_time):
                        open[neighbor].set_g(neighbor_g)
                        open[neighbor].parent = current_node
                        open[neighbor].parent_edge = edge
                continue
            else:
                # Węzły do których nie da się dojechać pomijamy
                continue
        
        # Dla każdej krawędzi wychodzącej z obecnego węzła
        for edge in graph[current_node.vertex]:
            neighbor = edge.end_vertex

            # Jeżeli sąsiad był już sprawdzony to pomijamy
            if neighbor in closed.keys():
                continue
            
            # Koszt rośnie tylko wtedy kiedy nastepuje zmiana linii lub gdy czas przyjazdu na przystanek różni się
            # od czasu odjazdu o więcej niż 2 minuty
            neighbor_g = current_node.g + (edge.line != current_node.parent_edge.line or minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) > 2)
            
            # Jeżeli pierwszy raz rozpatrujemy tego sąsiada to dodajemy go do kolejki
            if neighbor not in open.keys():
                new_node = Node(
                    vertex=neighbor,
                    parent=current_node,
                    parent_edge=edge,
                    g=neighbor_g,
                    h=calculate_distance(current_node.vertex, neighbor)
                )
                open[neighbor] = new_node
            # Jeżeli już analizowaliśmy tego sąsiada to sprawdzamy czy obecne połączenie ma mniej przesiadek
            # od poprzedniego, jeżeli tyle samo to wybieramy to, które jest szybsze czasowo
            elif neighbor_g < open[neighbor].g or \
                (neighbor_g == open[neighbor].g and (minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) + edge.travel_time) \
                < minutes_between(current_node.parent_edge.arrival_time, open[neighbor].parent_edge.departure_time) + open[neighbor].parent_edge.travel_time):
                open[neighbor].set_g(neighbor_g)
                open[neighbor].parent = current_node
                open[neighbor].parent_edge = edge
    return None


def main():
    graph = get_graph(get_edges())

    if len(sys.argv) != 5:
        print("Usage: python a_star.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        "<kryterium optymalizacji t/p> <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)
    
    start_stop = sys.argv[1].strip()
    end_stop = sys.argv[2].strip()
    option = sys.argv[3].strip()
    start_time = sys.argv[4].strip()

    isStartGood = False
    isEndGood = False

    for vertex in graph.keys():
        if start_stop.lower() == vertex.name.lower():
            start_stop = vertex.name
            isStartGood = True
        if end_stop.lower() == vertex.name.lower():
            end_stop = vertex.name
            isEndGood = True
    
    if not (isStartGood and isEndGood):
        print("Nieprawidłowa nazwa przystanku")
        print("Usage: python a_star.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        "<kryterium optymalizacji t/p> <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)
    
    if not option.lower() in ("t, p"):
        print("Nieprawidłowe kryterium optymalizacji")
        print("Usage: python a_star.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        "<kryterium optymalizacji t/p> <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)
    
    try:
        datetime.strptime(start_time, "%H:%M")
        start_time = start_time + ":00"
    except ValueError:
        print("Nieprawidłowy czas początkowy")
        print("Usage: python a_star.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        "<kryterium optymalizacji t/p> <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)

    algorithm_start_time = time.perf_counter_ns()

    if option == "t":
        destination_node = a_star_time_faster(graph, start_stop, end_stop, start_time)
    else:
        destination_node = a_star_change(graph, start_stop, end_stop, start_time)

    weight = destination_node.g
    path = backtrack_path(destination_node)

    algorithm_end_time = time.perf_counter_ns()

    print("\n#####################################################################\n")

    print(f"Czas działania algorytmu: {(algorithm_end_time - algorithm_start_time) / 1000000} ms", file=sys.stderr)
    
    if weight == float("inf"):
        print("Nie istnieje połączenie pomiędzy podanymi przystankami.")
    else:
        if option == "t":
            print(f"Najkrótszy czas przejazdu wynosi {weight} minut.\n", file=sys.stderr)
        else:
            print(f"Najmniejsza liczba przesiadek wynosi {weight}.\n", file=sys.stderr)
        print_path(path)


if __name__ == "__main__":
    main()
