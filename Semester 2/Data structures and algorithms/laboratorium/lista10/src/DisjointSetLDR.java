public class DisjointSetLDR {
    private int[] parent;
    private int[] rank;

    public DisjointSetLDR(int n){
        this.parent = new int[n];
        this.rank = new int[n];
    }

    public void makeSet(int x){
        this.parent[x]=x;
        this.rank[x]=0;
    }

    public int findSet(int x){
        if (x!=this.parent[x]){
            this.parent[x]=findSet(this.parent[x]);
        }
        return parent[x];
    }

    public void union(int x, int y){
        int repr_x = findSet(x);
        int repr_y = findSet(y);

        if (repr_x==repr_y)return;

        if (rank[repr_x] < rank[repr_y]){
            parent[repr_x]=repr_y;
        }else{
            parent[repr_y]=repr_x;
            if (rank[repr_x]==rank[repr_y]){
                rank[repr_x] = rank[repr_x] + 1;
            }
        }

    }

    public void display(){
        System.out.print("\nelement:   ");
        for (int i=0;i< parent.length; i++) System.out.print(i+" ");
        System.out.print("\nparent:    ");
        for (int i=0; i<parent.length; i++) System.out.print(parent[i]+" ");
        System.out.print("\nrank:      ");
        for (int i=0; i<rank.length; i++) System.out.print(rank[i]+" ");
        System.out.println("\n");
    }
}
