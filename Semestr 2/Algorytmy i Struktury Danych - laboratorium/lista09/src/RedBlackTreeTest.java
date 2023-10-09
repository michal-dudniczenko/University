import java.util.Scanner;

public class RedBlackTreeTest {
    public static void main(String[] args){
        Scanner input = new Scanner(System.in);

        int wybor = -1;

        RedBlackTree rbt = new RedBlackTree();

        rbt.insert(10);
        rbt.insert(20);
        rbt.insert(30);
        rbt.insert(40);
        rbt.insert(50);

        while(wybor!=0){
            System.out.println("------------------\tMENU\t------------------");
            System.out.print("1 - wstaw węzeł\t\t\t\t\t\t\t\t\t");
            System.out.print("2 - znajdź węzeł\n");
            System.out.print("3 - wyświetl klucze in order\t\t\t\t\t");
            System.out.print("4 - wyświetl klucze level order\n");
            System.out.print("5 - wyświetl informacje o drzewie\t\t\t\t");
            System.out.print("6 - wpisz i wyświetl informacje o poddrzewach\n");
            System.out.print("0 - zakończ\n");
            System.out.println("------------------\tDrzewo:\t------------------");
            rbt.displayTree();
            System.out.println("----------------------------------------------");
            wybor = Integer.parseInt(input.nextLine());

            switch (wybor) {
                case 1: {
                    System.out.println("Podaj klucz: ");
                    int key = Integer.parseInt(input.nextLine());
                    rbt.insert(key);
                    break;
                }
                case 2:{
                    System.out.println("Podaj klucz do znalezienia:");
                    int key = Integer.parseInt(input.nextLine());
                    if (rbt.search(key)){
                        System.out.println("Klucz znajduje się w drzewie.");
                    }else{
                        System.out.println("Klucz nie znajduje się w drzewie.");
                    }
                    break;
                }
                case 3:{
                    rbt.inOrderTraversal();
                    System.out.println();
                    break;
                }
                case 4:{
                    rbt.levelOrderTraversal();
                    System.out.println();
                    break;
                }
                case 5:{
                    rbt.getInfo();
                    break;
                }
                case 6:{
                    rbt.enterInfoForAllNodes();
                    rbt.getInfoForAllNodes();
                }
                case 0:{
                    break;
                }
                default: {
                    System.out.print("\nNieprawidłowy wybór!");
                    break;
                }
            }
        }
    }
}
