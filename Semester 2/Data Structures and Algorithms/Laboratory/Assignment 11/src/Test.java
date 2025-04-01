public class Test {
    public static void main(String[] args){
        System.out.println("\n--------------------------------------------------------------------------------------------\n");
        System.out.println("Undirected:");
        
        Graph graph = new Graph();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();
        graph.addVertice();

        graph.addEdge(0,1);
        graph.addEdge(0,2);
        graph.addEdge(0,3);
        graph.addEdge(1,4);
        graph.addEdge(1,5);
        graph.addEdge(2,6);
        graph.addEdge(5,7);

        graph.displayMatrix();
        graph.displayList();
        graph.display();
        graph.BFS(0);
        graph.DFS();

        System.out.println("\n--------------------------------------------------------------------------------------------\n");

        System.out.println("Directed:");
        
        Graph digraph = new Graph();
        digraph.addVertice();
        digraph.addVertice();
        digraph.addVertice();
        digraph.addVertice();
        digraph.addVertice();
        digraph.addVertice();
        digraph.addVertice();

        digraph.addEdgeDir(0,1);
        digraph.addEdgeDir(1,4);
        digraph.addEdgeDir(1,5);
        digraph.addEdgeDir(0,2);
        digraph.addEdgeDir(0,3);
        digraph.addEdgeDir(2,6);

        digraph.displayDir();
        digraph.displayMatrix();
        digraph.displayList();
        digraph.BFS(0);
        digraph.DFS();
    }
}
