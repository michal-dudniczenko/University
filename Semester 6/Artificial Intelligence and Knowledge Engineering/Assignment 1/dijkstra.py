from utils import get_edges, get_graph, minutes_between, backtrack_path, print_path, Node

import time
import sys
from datetime import datetime


def dijkstra(graph, start_vertex_name, end_vertex_name, start_time):
    nodes = dict()

    # 1. Inicjalizuj d(s) = 0 oraz d(v) = ∞ dla każdego v ∈ V \ {s}.
    for vertex in graph.keys():
        nodes[vertex] = Node (
            vertex=vertex,
            g = float("inf")
        )
    
    for vertex, node in nodes.items():
        if vertex.name == start_vertex_name:
            node.set_g(0)
    
    # 2. Utwórz zbiór Q zawierający wszystkie wierzchołki grafu G.
    Q = nodes.copy()

    # 3. Dopóki Q nie jest pusty, wykonuj:
    while Q:
        # (a) Wybierz wierzchołek u ∈ Q o najmniejszej wartości d(u) i usuń go ze zbioru Q
        _, u = min(Q.items(), key=lambda item: item[1].f)
        Q.pop(u.vertex)

        if u.parent is None:
            # Logika dla węzła startowego, uwzględnia czas pojawienia się na przystanku
            if u.vertex.name == start_vertex_name:
                for edge in graph[u.vertex]:
                    new_weight = minutes_between(start_time, edge.departure_time) + edge.travel_time
                    if new_weight < nodes[edge.end_vertex].g:
                        nodes[edge.end_vertex].set_g(new_weight)
                        nodes[edge.end_vertex].parent = u
                        nodes[edge.end_vertex].parent_edge = edge
                continue
            else: 
                # Węzły do których nie da się dojechać pomijamy
                continue

        # (b) Dla każdego v takiego, że ∃(u, v)∈E d(v) > d(u) + w(u, v) 
        # zaktualizuj d(v) = d(u) + w(u, v) oraz p(v) = u
        for edge in graph[u.vertex]:
            new_weight = u.g + minutes_between(u.parent_edge.arrival_time, edge.departure_time) + edge.travel_time
            if new_weight < nodes[edge.end_vertex].g or (new_weight == nodes[edge.end_vertex].g and edge.line == u.parent_edge.line):
                nodes[edge.end_vertex].set_g(new_weight)
                nodes[edge.end_vertex].parent_edge = edge
                nodes[edge.end_vertex].parent = u

    for vertex, node in nodes.items():
        if vertex.name == end_vertex_name:
            return node
    return None


def main():
    graph = get_graph(get_edges())

    if len(sys.argv) != 4:
        print("Usage: python dijkstra.py <nazwa przystanku początkowego> <nazwa przystanku docelowego> " +
        "<czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)
    
    start_stop = sys.argv[1].strip()
    end_stop = sys.argv[2].strip()
    start_time = sys.argv[3].strip()

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
        print("Usage: python dijkstra.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        " <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)
    
    try:
        datetime.strptime(start_time, "%H:%M")
        start_time = start_time + ":00"
    except ValueError:
        print("Nieprawidłowy czas początkowy")
        print("Usage: python dijkstra.py <nazwa przystanku początkowego> <nazwa przystanku docelowego>" +
        " <czas pojawienia się na przystanku początkowym hh:mm>")
        sys.exit(1)

    algorithm_start_time = time.perf_counter_ns()

    destination_node = dijkstra(graph, start_stop, end_stop, start_time)
    weight = destination_node.g
    path = backtrack_path(destination_node)

    algorithm_end_time = time.perf_counter_ns()

    print("\n#####################################################################\n")

    print(f"Czas działania algorytmu: {(algorithm_end_time - algorithm_start_time) / 1000000} ms", file=sys.stderr)
    
    if weight == float("inf"):
        print("Nie istnieje połączenie pomiędzy podanymi przystankami.")
    else:
        print(f"Najkrótszy czas przejazdu wynosi {weight} minut.\n", file=sys.stderr)
        print_path(path)


if __name__ == "__main__":
    main()
