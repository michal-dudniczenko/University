public class DisjointSet {
    private int[] parent;

    public DisjointSet(int size){
        this.parent=new int[size];
    }

    public void makeSet(int x){
        parent[x]=x;
    }

    public void union(int x, int y){
        link(findSet(x),findSet(y));
    }

    private void link(int x, int y){
        parent[y]=x;
    }

    public int findSet(int x){
        if (x!=parent[x]){
            parent[x] = findSet(parent[x]);
        }
        return parent[x];
    }
}
