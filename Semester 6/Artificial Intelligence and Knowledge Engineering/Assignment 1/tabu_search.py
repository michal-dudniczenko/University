from utils import Vertex, Edge, minutes_between, get_edges, get_graph
from a_star import a_star_time, a_star_time_faster

import random
import heapq
from collections import deque
import time
import sys


def calculate_path_time(graph, list_of_stops, start_time):
    total_time = 0
    prev_arr_time = start_time

    for i in range(0, len(list_of_stops) - 1):
        result = a_star_time(graph, list_of_stops[i], list_of_stops[i + 1], prev_arr_time)
        total_time += result.g
        prev_arr_time = result.parent_edge.arrival_time
    
    return total_time

def calculate_path_time_faster(graph, list_of_stops, start_time):
    total_time = 0
    prev_arr_time = start_time

    for i in range(0, len(list_of_stops) - 1):
        result = a_star_time_faster(graph, list_of_stops[i], list_of_stops[i + 1], prev_arr_time)
        total_time += result.g
        prev_arr_time = result.parent_edge.arrival_time
    
    return total_time


def tabu_search_time(graph, start_stop_name, stops_to_visit_names, start_time, max_run_time_seconds = 10):
    search_start_time = time.time()
    current_time = search_start_time
    time_limit = search_start_time + max_run_time_seconds

    random.shuffle(stops_to_visit_names)

    current_solution = [start_stop_name] + stops_to_visit_names + [start_stop_name]
    current_solution_time = calculate_path_time(graph, current_solution, start_time)
    best_solution = current_solution
    best_solution_time = current_solution_time

    tabu = set()

    print(f"{current_solution_time} minut")

    return

    while current_time < time_limit:
        current_time = time.time()

def tabu_search_time_faster(graph, start_stop_name, stops_to_visit_names, start_time, max_run_time_seconds = 10):
    search_start_time = time.time()
    current_time = search_start_time
    time_limit = search_start_time + max_run_time_seconds

    # random.shuffle(stops_to_visit_names)

    current_solution = [start_stop_name] + stops_to_visit_names + [start_stop_name]
    current_solution_time = calculate_path_time_faster(graph, current_solution, start_time)
    best_solution = current_solution
    best_solution_time = current_solution_time

    tabu = set()

    print(f"{current_solution_time} minut")

    return

    while current_time < time_limit:
        current_time = time.time()


def main():
    graph = get_graph(get_edges())
    
    start_stop_name = "Śliczna"
    start_time = "12:52:00"
    option = "t"

    stops_list = []

    vertices = list(graph.keys())
    vertices_len = len(vertices)

    while len(stops_list) < 5:
        stop = vertices[random.randint(0, vertices_len - 1)].name
        if stop not in stops_list:
            stops_list.append(stop)

    algorithm_start_time = time.perf_counter_ns()

    tabu_search_time(graph, start_stop_name, stops_list, start_time)

    algorithm_end_time = time.perf_counter_ns()

    print("\n#####################################################################\n")

    print(f"Czas działania algorytmu: {(algorithm_end_time - algorithm_start_time) / 1000000} ms", file=sys.stderr)

    algorithm_start_time = time.perf_counter_ns()

    tabu_search_time_faster(graph, start_stop_name, stops_list, start_time)

    algorithm_end_time = time.perf_counter_ns()

    print("\n#####################################################################\n")

    print(f"Czas działania algorytmu: {(algorithm_end_time - algorithm_start_time) / 1000000} ms", file=sys.stderr)
    """
    print("Best Route:", [v.name for v in best_route])
    print("Minimum Travel Time:", best_time)
    """

if __name__ == "__main__":
    main()

