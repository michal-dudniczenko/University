import java.util.ArrayList;
import java.util.LinkedList;

public class Graph {
    private class Vertice{
        private int key;
        private boolean visited;

        public Vertice(int key){
            this.key=key;
            this.visited=false;
        }

        public String toString(){
            return "("+this.key+")";
        }
    }
    private class Edge{
        private Vertice start;
        private Vertice end;

        public Edge(Vertice start, Vertice end){
            this.start=start;
            this.end=end;
        }

    }

    private int verticeNumber;
    private ArrayList<Vertice> V;
    private ArrayList<Edge> E;
    private int[][] macierzSasiedztwa;
    private ArrayList<LinkedList<Vertice>> listaSasiedztwa;

    public Graph(){
        this.verticeNumber=0;
        this.V = new ArrayList<>();
        this.E = new ArrayList<>();
        this.listaSasiedztwa = new ArrayList<>();
        createMatrix();
    }

    private void createMatrix(){
        this.macierzSasiedztwa = new int[verticeNumber][verticeNumber];
        for (Edge e: E){
            macierzSasiedztwa[e.start.key][e.end.key]=1;
        }
    }

    public void addVertice(){
        this.V.add(new Vertice(this.verticeNumber));
        this.verticeNumber++;
        createMatrix();
        this.listaSasiedztwa.add(new LinkedList<Vertice>());
    }

    public void addEdge(int start, int end){
        Edge e = new Edge(V.get(start), V.get(end));
        this.E.add(e);
        createMatrix();
        if (!listaSasiedztwa.get(e.start.key).contains(e.end)){
            listaSasiedztwa.get(e.start.key).add(e.end);
        }
        if (!listaSasiedztwa.get(e.end.key).contains(e.start)){
            listaSasiedztwa.get(e.end.key).add(e.start);
        }
    }

    public void addEdgeDir(int start, int end){
        Edge e = new Edge(V.get(start), V.get(end));
        this.E.add(e);
        createMatrix();
        if (!listaSasiedztwa.get(e.start.key).contains(e.end)){
            listaSasiedztwa.get(e.start.key).add(e.end);
        }
    }

    public void BFS(int n){
        Vertice s = this.V.get(n);
        System.out.println("\nPrzejście BFS po grafie (start: "+n+"):\n");
        System.out.println("Kolejka:");
        Vertice[] pre = new Vertice[verticeNumber];
        ArrayList<Vertice> queue = new ArrayList<>();

        for (Vertice v : this.V){
            v.visited=false;
            pre[v.key]=null;
        }
        s.visited=true;
        System.out.println("odwiedzony: "+s.key);

        queue.add(s);

        while (queue.size()>0){
            for (Vertice v : queue){
                System.out.print(v.key+" ");
            }
            System.out.print("\n");

            Vertice u = queue.get(0);
            queue.remove(0);
            for (Vertice v : this.listaSasiedztwa.get(u.key)){
                if (!v.visited){
                    v.visited=true;
                    System.out.println("odwiedzony: "+v.key);
                    pre[v.key]=u;
                    queue.add(v);
                }
            }
        }
        System.out.print("\nTablica poprzedników: [");
        for (Vertice v : pre){
            if (v==null) System.out.print("null");
            else System.out.print(v.key);
            System.out.print(", ");
        }
        System.out.print("]\n");
    }

    public void DFS(){
        System.out.println("\nPrzejście DFS po grafie:\n");
        System.out.println("Stos:");
        Vertice[] pre = new Vertice[verticeNumber];
        for (int i=0; i<verticeNumber; i++){
            V.get(i).visited=false;
            pre[i]=null;
        }
        for (Vertice u : V){
            if (!u.visited){
                DFS_Visit(u, pre);
            }
        }
        System.out.print("\nTablica poprzedników: [");
        for (Vertice v : pre){
            if (v==null) System.out.print("null");
            else System.out.print(v.key);
            System.out.print(", ");
        }
        System.out.print("]\n");
    }
    private void DFS_Visit(Vertice u, Vertice[] pre){
        System.out.println("Na stos trafia: "+u.key);
        u.visited=true;
        for (Vertice sasiad : listaSasiedztwa.get(u.key)){
            if (!sasiad.visited){
                pre[sasiad.key]=u;
                DFS_Visit(sasiad, pre);
            }
        }
        System.out.println("Ze stosu jest ściągnany: "+u.key);
    }

    public void display(){
        System.out.print("\nWierzchołki: [");
        for (Vertice v : V) System.out.print(v.key+", ");
        System.out.print("]\n");
        System.out.print("\nKrawędzie: [");
        for (Edge e : E) System.out.print("("+e.start.key+"-"+e.end.key+"), ");
        System.out.print("]\n");
    }

    public void displayDir(){
        System.out.print("\nWierzchołki: [");
        for (Vertice v : V) System.out.print(v.key+", ");
        System.out.print("]\n");
        System.out.print("\nKrawędzie: [");
        for (Edge e : E) System.out.print("("+e.start.key+"->"+e.end.key+"), ");
        System.out.print("]\n");
    }

    public void displayMatrix(){
        System.out.println("\nMacierz sąsiedztwa:\n");
        System.out.print("    ");
        for (int i = 0 ; i<this.verticeNumber; i++){
            System.out.format("%4d", i);
        }
        System.out.print("\n");
        for (int i = 0 ; i<this.verticeNumber; i++){
            System.out.format("%4d", i);
            for (int j=0; j<this.verticeNumber; j++){
                System.out.format("%4d", this.macierzSasiedztwa[i][j]);
            }
            System.out.print("\n");
        }
    }

    public void displayList(){
        System.out.println("\nLista sąsiedztwa:");
        int temp=0;
        for (LinkedList<Vertice> ll : this.listaSasiedztwa){
            System.out.print("\n("+temp+") -> ");
            for (Vertice v : ll){
                System.out.print(v.key+" -> ");
            }
            temp++;
        }
        System.out.print("\n");
    }
}
