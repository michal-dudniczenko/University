public class DSLDR_Test {
    public static void main(String[] args){
        DisjointSetLDR dsldr = new DisjointSetLDR(10);

        dsldr.makeSet(0);
        dsldr.makeSet(1);
        dsldr.makeSet(2);
        dsldr.makeSet(3);
        dsldr.makeSet(4);
        dsldr.makeSet(5);
        dsldr.makeSet(6);
        dsldr.makeSet(7);
        dsldr.makeSet(8);
        dsldr.makeSet(9);

        dsldr.display();

        System.out.println("union(1,0)");dsldr.union(1,0);
        System.out.println("union(1,2)");dsldr.union(1,2);
        System.out.println("union(3,4)");dsldr.union(3,4);

        dsldr.display();

        System.out.println("\nfindSet(2): "+dsldr.findSet(2));


        System.out.println("union(5,6)");dsldr.union(5,6);
        System.out.println("union(7,8)");dsldr.union(7,8);
        System.out.println("union(8,9)");dsldr.union(8,9);
        dsldr.display();

        System.out.println("union(1,4)");dsldr.union(1,4);

        dsldr.display();

        System.out.println("findSet(5): "+dsldr.findSet(5));
        System.out.println("findSet(6): "+dsldr.findSet(6));
        System.out.println("findSet(0): "+dsldr.findSet(0));
        System.out.println("findSet(4): "+dsldr.findSet(4));
        System.out.println("findSet(9): "+dsldr.findSet(9));

        dsldr.display();
    }
}
