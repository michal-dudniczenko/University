from datetime import datetime, timedelta


class Vertex:
    def __init__(self, name, latitude, longitude):
        self.name = name
        self.latitude = latitude
        self.longitude = longitude
    
    def __eq__(self, other):
        if isinstance(other, str):
            return self.name == other

        return self.name == other.name
                
    def __hash__(self):
        return hash(self.name)


class Edge:
    def __init__(self, line, departure_time, arrival_time, start_vertex, end_vertex):
        self.line = line

        dep_time_hours, dep_time_minutes, dep_time_seconds = map(int, departure_time.split(":"))
        self.departure_time = f"{dep_time_hours % 24:02}:{dep_time_minutes % 60:02}:{dep_time_seconds % 60:02}"

        arr_time_hours, arr_time_minutes, arr_time_seconds = map(int, arrival_time.split(":"))
        self.arrival_time = f"{arr_time_hours % 24:02}:{arr_time_minutes % 60:02}:{arr_time_seconds % 60:02}"

        self.start_vertex = start_vertex
        self.end_vertex = end_vertex

        self.travel_time = minutes_between(self.departure_time, self.arrival_time)


class Node:
    def __init__(self, vertex, parent=None, parent_edge=None,  g=0, h=0):
        self.vertex = vertex
        self.parent = parent
        self.parent_edge = parent_edge
        self.g = g
        self.h = h
        self.f = g + h
    
    def set_g(self, g):
        self.g = g
        self.f = self.g + self.h
    
    def __lt__(self, other):
        return self.f < other.f
    
    def __eq__(self, other):
        return self.vertex == other.vertex
    
    def __hash__(self):
        return hash(self.vertex)


def minutes_between(time1, time2):
    fmt = "%H:%M:%S"
    t1 = datetime.strptime(time1, fmt)
    t2 = datetime.strptime(time2, fmt)

    if t2 < t1:
        t2 += timedelta(days=1)

    return (t2 - t1).seconds // 60


def calculate_distance(vertex1, vertex2):
    return (abs(vertex1.latitude - vertex2.latitude) + abs(vertex1.longitude - vertex2.longitude)) / 1


def get_edges():
    edges = []
    with open("connection_graph.csv", "r", encoding="utf-8") as file:
        next(file)
        for line in file:
            try:
                _, _, bus_line, departure_time, arrival_time, \
                start_stop, end_stop, start_stop_lat, start_stop_lon, \
                end_stop_lat, end_stop_lon = line.strip().split(sep=",")

                start_vertex = Vertex(
                    name=start_stop.strip(),
                    latitude=float(start_stop_lat),
                    longitude=float(start_stop_lon)
                )
                end_vertex = Vertex(
                    name=end_stop.strip(),
                    latitude=float(end_stop_lat),
                    longitude=float(end_stop_lon)
                )

                edges.append(
                    Edge(
                        line=bus_line.strip(), 
                        departure_time=departure_time,
                        arrival_time=arrival_time,
                        start_vertex=start_vertex,
                        end_vertex=end_vertex
                    )
                )
            except:
                pass

    return edges


def get_graph(edges):
    graph = dict()

    for edge in edges:
        graph.setdefault(edge.start_vertex, []).append(edge)
        graph.setdefault(edge.end_vertex, [])
    
    return graph


def backtrack_path(node):
    path = []
    current_node = node
    while current_node.parent is not None:
        path.append(current_node.parent_edge)
        current_node = current_node.parent
    path.reverse()

    return path


def print_path(path):
    print("Trasa:")
    
    line = path[0].line
    enter_stop = path[0].start_vertex.name
    exit_stop = None
    enter_time = path[0].departure_time
    exit_time = None

    path_index = 1
    while path_index < len(path):
        if path[path_index].line != line or path[path_index].departure_time != path[path_index - 1].arrival_time:
            exit_stop = path[path_index - 1].end_vertex.name
            exit_time = path[path_index - 1].arrival_time
            print(f"\tLinia {line}: {enter_stop} ({enter_time}) -> {exit_stop} ({exit_time})")
            line = path[path_index].line
            enter_stop = path[path_index].start_vertex.name
            enter_time = path[path_index].departure_time
        path_index += 1
    
    exit_stop = path[-1].end_vertex.name
    exit_time = path[-1].arrival_time
    print(f"\tLinia {line}: {enter_stop} ({enter_time}) -> {exit_stop} ({exit_time})")
