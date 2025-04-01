public class DSLW_Test {
    public static void main(String[] args){
        DisjointSetLW dslw = new DisjointSetLW();

        dslw.makeSet(1);
        dslw.makeSet(2);
        dslw.makeSet(3);
        dslw.makeSet(4);
        dslw.makeSet(5);
        dslw.makeSet(6);
        dslw.makeSet(7);
        dslw.makeSet(8);
        dslw.makeSet(9);
        dslw.makeSet(10);

        dslw.display();

        dslw.union(1,2);
        dslw.union(2,3);
        dslw.union(4,5);
        dslw.display();

        System.out.println("\nfindSet(3): "+dslw.findSet(3).key);

        dslw.union(6,7);
        dslw.union(8,9);
        dslw.union(9,10);
        dslw.display();

        dslw.union(5,2);
        dslw.unionWywazanie(7, 10);

        dslw.display();

        System.out.println("\nfindSet(5): "+dslw.findSet(5).key);
        System.out.println("\nfindSet(6): "+dslw.findSet(6).key);

    }
}
