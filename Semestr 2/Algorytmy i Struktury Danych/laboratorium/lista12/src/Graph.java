import java.util.ArrayList;
import java.util.Collections;

public class Graph {
    public class Vertice{
        private int key;

        public Vertice(int key){
            this.key=key;
        }

        public String toString(){
            return String.valueOf(this.key);
        }
    }

    public class Edge implements Comparable<Edge>{
        private Vertice v1;
        private Vertice v2;
        private int weight;

        public Edge(Vertice v1, Vertice v2, int weight){
            this.v1=v1;
            this.v2=v2;
            this.weight=weight;
        }

        public int compareTo(Edge e){
            return this.weight-e.weight;
        }

        public String toString(){
            return "("+v1+" --- "+v2+"), weight: "+weight;
        }
    }

    private ArrayList<Vertice> V;
    private ArrayList<Edge> E;
    private int verticeNumber;
    private ArrayList<ArrayList<Vertice>> adjList;

    public Graph(int verticeNumber){
        this.V = new ArrayList<>();
        this.E = new ArrayList<>();
        this.verticeNumber=verticeNumber;
        this.adjList =new ArrayList<>();

        for (int i=0; i<verticeNumber; i++){
            this.V.add(new Vertice(i));
        }
    }

    public void addEdge(int v1, int v2, int weight){
        Edge e = new Edge(V.get(v1), V.get(v2), weight);
        this.E.add(e);
    }

    private void createAdjacencyList(){
        ArrayList<ArrayList<Vertice>> result = new ArrayList<>();
        for (int i=0; i<verticeNumber; i++){
            result.add(new ArrayList<Vertice>());
        }
        for (Edge e : E){
            if (!result.get(e.v1.key).contains(e.v2)){
                result.get(e.v1.key).add(e.v2);
            }
            if (!result.get(e.v2.key).contains(e.v1)){
                result.get(e.v2.key).add(e.v1);
            }
        }
        this.adjList =result;
    }

    public void display(){
        System.out.print("\nWierzchołki: [");
        for (Vertice v : V) System.out.print(v.key+", ");
        System.out.print("]\n");
        System.out.print("Krawędzie: [");
        for (Edge e : E) System.out.print("("+e.v1.key+"-"+e.v2.key+"), ");
        System.out.print("]\n");
    }

    public int weight(Vertice v1, Vertice v2){
        for (Edge e : E){
            if (e.v1.equals(v1) && e.v2.equals(v2)){
                return e.weight;
            }
            else if (e.v2.equals(v1) && e.v1.equals(v2)){
                return e.weight;
            }
        }
        return -1;
    }

    private Edge findEdge(Vertice v1, Vertice v2){
        for (Edge e : E){
            if (e.v1.equals(v1) && e.v2.equals(v2)){
                return e;
            }
            else if (e.v2.equals(v1) && e.v1.equals(v2)){
                return e;
            }
        }
        return null;
    }

    public ArrayList<Edge> kruskalMST(){
        ArrayList<Edge> result = new ArrayList<>();
        DisjointSet disjointSet = new DisjointSet(verticeNumber);

        for (Vertice v : V){
            disjointSet.makeSet(v.key);
        }

        ArrayList<Edge> edges = new ArrayList<>(E);
        Collections.sort(edges);

        int v1;
        int v2;
        for (Edge edge: edges){
            v1 = edge.v1.key;
            v2 = edge.v2.key;

            if (disjointSet.findSet(v1)!=disjointSet.findSet(v2)){
                result.add(edge);
                disjointSet.union(v1, v2);
                System.out.println("Dodany: "+edge);
            }
            else{
                System.out.println("Odrzucony: "+edge);
            }
        }

        return result;
    }

    public ArrayList<Edge> primMST() {
        ArrayList<Edge> result = new ArrayList<>();

        createAdjacencyList();
        ArrayList<Vertice> visited = new ArrayList<>();
        visited.add(V.get(0));

        int min,weight;
        Vertice min1,min2;
        while (visited.size()<verticeNumber) {
            min = Integer.MAX_VALUE;
            min1 = visited.get(0);
            min2 = visited.get(0);

            for (Vertice v : visited) {
                for (Vertice w : adjList.get(v.key)) {
                    if (!visited.contains(w)){
                        weight = weight(v, w);
                        if (weight<min){
                            min=weight;
                            min1=v;
                            min2=w;
                        }
                    }
                }
            }
            visited.add(min2);
            Edge temp = new Edge(min1, min2, min);
            result.add(temp);
            System.out.println("Dodany: "+temp);
        }
        return result;
    }

    public ArrayList<Edge> dijkstraSP(int start, int end) {
        createAdjacencyList();

        int[] shortestDistance = new int[verticeNumber];
        Edge[] previous = new Edge[verticeNumber];

        for (int i=0; i<verticeNumber; i++){
            shortestDistance[i] = Integer.MAX_VALUE;
            previous[i]=null;
        }

        ArrayList<Vertice> visited = new ArrayList<>();
        ArrayList<Vertice> unvisited = new ArrayList<>(V);
        shortestDistance[start]=0;

        int min;
        Vertice minVert;
        while (visited.size()<verticeNumber){
            min=Integer.MAX_VALUE;
            minVert=unvisited.get(0);
            for (Vertice v : unvisited){
                if (shortestDistance[v.key]<min){
                    min=shortestDistance[v.key];
                    minVert=v;
                }
            }
            for (Vertice v : adjList.get(minVert.key)){
                if (!visited.contains(v)){
                    int weight = weight(minVert, v) + shortestDistance[minVert.key];
                    if (weight<shortestDistance[v.key]){
                        shortestDistance[v.key]=weight;
                        previous[v.key]=findEdge(v, minVert);
                    }
                }
            }
            visited.add(minVert);
            unvisited.remove(minVert);
        }

        ArrayList<Edge> result = new ArrayList<>();

        Edge temp = previous[end];
        int temp2 = end;

        while (temp!=null){
            result.add(temp);
            if (temp.v2.key==temp2){
                temp2=temp.v1.key;
            }
            else{
                temp2=temp.v2.key;
            }
            temp=previous[temp2];
        }
        return result;
    }


    public static void main(String[] args){
        System.out.println("--------------------------------------------------------------");
        Graph graph = new Graph(9);
        graph.addEdge(0,1,4);
        graph.addEdge(1,2,8);
        graph.addEdge(2,3,7);
        graph.addEdge(3,4,9);
        graph.addEdge(4,5,10);
        graph.addEdge(3,5,14);
        graph.addEdge(2,5,4);
        graph.addEdge(2,8,2);
        graph.addEdge(8,6,6);
        graph.addEdge(6,5,2);
        graph.addEdge(6,7,1);
        graph.addEdge(7,8,7);
        graph.addEdge(1,7,11);
        graph.addEdge(0,7,8);

        System.out.println("Krawędzie Kruskal MST:");
        ArrayList<Edge> mst1 = graph.kruskalMST();
        System.out.println();
        for (Edge e : mst1) System.out.println(e);
        System.out.println();

        System.out.println("--------------------------------------------------------------");

        System.out.println("Krawędzie Prim MST:");
        ArrayList<Edge> mst2 = graph.primMST();
        System.out.println();
        for (Edge e : mst2) System.out.println(e);
        System.out.println();

        System.out.println("--------------------------------------------------------------");

        int start = 3;
        int end = 7;
        System.out.println("Najkrótsza ścieżka Dijkstra od "+start+" do "+end+": ");
        ArrayList<Edge> dijkstra = graph.dijkstraSP(start, end);
        System.out.println();
        for (int i = dijkstra.size()-1; i>=0; i--) System.out.println(dijkstra.get(i));
        System.out.println();
    }
}
