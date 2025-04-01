from utils import get_edges, get_graph, minutes_between, calculate_distance, backtrack_path, print_path, Node

import time
import sys
from datetime import datetime


def a_star_time(graph, start_vertex_name, end_vertex_name, start_time):
    open = dict()
    closed = dict()

    for vertex in graph.keys():
        if vertex.name == start_vertex_name:
            open[vertex] = Node (
                vertex=vertex
            )    

    while open:
        _, current_node = min(open.items(), key=lambda item: item[1].f)

        if current_node.vertex.name == end_vertex_name:
            return current_node
        
        closed[current_node.vertex] = current_node
        open.pop(current_node.vertex)

        if current_node.parent is None:
            # Logika dla węzła startowego, uwzględnia czas pojawienia się na przystanku
            if current_node.vertex.name == start_vertex_name:
                for edge in graph[current_node.vertex]:
                    neighbor = edge.end_vertex

                    if neighbor in closed.keys():
                        continue
                    
                    neighbor_g = minutes_between(start_time, edge.departure_time) + edge.travel_time

                    if neighbor not in open.keys():
                        new_node = Node(
                            vertex=neighbor,
                            parent=current_node,
                            parent_edge=edge,
                            g=neighbor_g,
                            h=calculate_distance(current_node.vertex, neighbor)
                        )
                        open[neighbor] = new_node
                    elif neighbor_g < open[neighbor].g:
                        open[neighbor].set_g(neighbor_g)
                        open[neighbor].parent = current_node
                        open[neighbor].parent_edge = edge
                continue
            else:
                # Węzły do których nie da się dojechać pomijamy
                continue

        for edge in graph[current_node.vertex]:
            neighbor = edge.end_vertex

            if neighbor in closed.keys():
                continue
            
            neighbor_g = current_node.g + minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) + edge.travel_time

            if neighbor not in open.keys():
                new_node = Node(
                    vertex=neighbor,
                    parent=current_node,
                    parent_edge=edge,
                    g=neighbor_g,
                    h=calculate_distance(current_node.vertex, neighbor)
                )
                open[neighbor] = new_node
            elif neighbor_g < open[neighbor].g or (neighbor_g == open[neighbor].g and edge.line == current_node.parent_edge.line):
                open[neighbor].set_g(neighbor_g)
                open[neighbor].parent = current_node
                open[neighbor].parent_edge = edge
    return None
                

def a_star_change(graph, start_vertex_name, end_vertex_name, start_time):
    open = dict()
    closed = dict()

    for vertex in graph.keys():
        if vertex.name == start_vertex_name:
            open[vertex] = Node (
                vertex=vertex
            )    

    while open:
        _, current_node = min(open.items(), key=lambda item: item[1].f)

        if current_node.vertex.name == end_vertex_name:
            return current_node
        
        closed[current_node.vertex] = current_node
        open.pop(current_node.vertex)

        if current_node.parent is None:
            # Logika dla węzła startowego
            if current_node.vertex.name == start_vertex_name:
                for edge in graph[current_node.vertex]:
                    neighbor = edge.end_vertex

                    if neighbor in closed.keys():
                        continue
                    
                    neighbor_g = 1

                    if neighbor not in open.keys():
                        new_node = Node(
                            vertex=neighbor,
                            parent=current_node,
                            parent_edge=edge,
                            g=neighbor_g,
                            h=calculate_distance(current_node.vertex, neighbor)
                        )
                        open[neighbor] = new_node
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

        for edge in graph[current_node.vertex]:
            neighbor = edge.end_vertex

            if neighbor in closed.keys():
                continue
            
            neighbor_g = current_node.g + (edge.line != current_node.parent_edge.line or minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) > 2)

            if neighbor not in open.keys():
                new_node = Node(
                    vertex=neighbor,
                    parent=current_node,
                    parent_edge=edge,
                    g=neighbor_g,
                    h=calculate_distance(current_node.vertex, neighbor)
                )
                open[neighbor] = new_node
            elif neighbor_g < open[neighbor].g or \
                (neighbor_g == open[neighbor].g and (minutes_between(current_node.parent_edge.arrival_time, edge.departure_time) + edge.travel_time) \
                < minutes_between(current_node.parent_edge.arrival_time, open[neighbor].parent_edge.departure_time) + open[neighbor].parent_edge.travel_time):
                open[neighbor].set_g(neighbor_g)
                open[neighbor].parent = current_node
                open[neighbor].parent_edge = edge
    return None


def main():
    graph = get_graph(get_edges())
    
    """
    start_stop = "Śliczna"
    end_stop = "PL. GRUNWALDZKI"
    start_time = "12:52:00"
    option = "p"
    """

    start_stop = None
    while not start_stop:
        user_input = input("Podaj przystanek początkowy: ")
        for vertex in graph.keys():
            if user_input.strip().lower() == vertex.name.lower():
                start_stop = vertex.name
                break
    
    end_stop = None
    while not end_stop:
        user_input = input("Podaj przystanek końcowy: ")
        for vertex in graph.keys():
            if user_input.strip().lower() == vertex.name.lower():
                end_stop = vertex.name
                break
    
    start_time = None
    while not start_time:
        user_input = input("Podaj czas odjazdu (HH:MM): ")
        try:
            datetime.strptime(user_input, "%H:%M")
            start_time = user_input + ":00"
        except ValueError:
            pass
    
    option = None
    while not option:
        user_input = input("Podaj opcję (t - czas, p - liczba przesiadek): ")
        if user_input.strip().lower() in ["t", "p"]:
            option = user_input.strip().lower()
    
    algorithm_start_time = time.perf_counter_ns()

    if option == "t":
        destination_node = a_star_time(graph, start_stop, end_stop, start_time)
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
