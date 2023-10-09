import java.util.Scanner;

public class MenuBST {
    public static void main(String[] args){
        Scanner input = new Scanner(System.in);

        int wybor = -1;

        BST bst = new BST();

        bst.insert(5);
        bst.insert(3);
        bst.insert(8);
        bst.insert(2);
        bst.insert(7);

        while(wybor!=0){
            System.out.println("------------------\tMENU\t------------------");
            System.out.print("1 - wstaw węzeł\t\t\t\t\t\t");
            System.out.print("2 - wyświetl klucze\n");
            System.out.print("3 - znajdź węzeł\t\t\t\t\t");
            System.out.print("4 - znajdź węzeł minimalny\n");
            System.out.print("5 - znajdź węzeł maksymalny\t\t\t");
            System.out.print("6 - wyznacz następnik\n");
            System.out.print("7 - wyznacz poprzednik\t\t\t\t");
            System.out.print("8 - usuń węzeł\n");
            System.out.print("9 - informacje o poddrzewach\t\t");
            System.out.print("10 - wyświetl informacje o drzewie\n");
            System.out.print("0 - zakończ\n");
            System.out.println("------------------\tDrzewo:\t------------------");
            bst.displayTree();
            System.out.println("----------------------------------------------");
            wybor = Integer.parseInt(input.nextLine());

            switch (wybor) {
                case 1: {
                    System.out.println("Podaj klucz: ");
                    int key = Integer.parseInt(input.nextLine());
                    bst.insert(key);
                    break;
                }
                case 2: {
                    System.out.println("Wybierz sposób wyświetlenia kluczy:");
                    System.out.println("1 - in order");
                    System.out.println("2 - pre order");
                    System.out.println("3 - post order");
                    int wyborPrzejscia = Integer.parseInt(input.nextLine());
                    switch (wyborPrzejscia) {
                        case 1: {
                            System.out.print("\nKlucze w kolejności in order:");
                            bst.displayInOrder();
                            break;
                        }
                        case 2: {
                            System.out.print("\nKlucze w kolejności pre order:");
                            bst.displayPreOrder();
                            break;
                        }
                        case 3: {
                            System.out.print("\nKlucze w kolejności post order:");
                            bst.displayPostOrder();
                            break;
                        }
                        default: {
                            System.out.println("Nieprawidłowy wybór!");
                            break;
                        }
                    }
                    break;
                }
                case 3:{
                    System.out.println("Podaj klucz do znalezienia:");
                    int key = Integer.parseInt(input.nextLine());
                    if (bst.search(key)){
                        System.out.println("Klucz znajduje się w drzewie.");
                    }else{
                        System.out.println("Klucz nie znajduje się w drzewie.");
                    }
                    break;
                }
                case 4:{
                    System.out.println("Węzęł o najmniejszym kluczu: "+bst.minValue());
                    break;
                }
                case 5:{
                    System.out.println("Węzęł o największym kluczu: "+bst.maxValue());
                    break;
                }
                case 6:{
                    System.out.println("Podaj klucz: ");
                    int key = Integer.parseInt(input.nextLine());
                    int result = bst.nastepnik(key).getKey();
                    if (result==-2){
                        System.out.println("Brak wybranego klucza w drzewie!");
                    }else if (result==-1){
                        System.out.println("Wybrany klucz nie ma następnika.");
                    }else{
                        System.out.println("Następnik wybranego klucza: "+result);
                    }
                    break;
                }
                case 7:{
                    System.out.println("Podaj klucz: ");
                    int key = Integer.parseInt(input.nextLine());
                    int result = bst.poprzednik(key).getKey();
                    if (result==-2){
                        System.out.println("Brak wybranego klucza w drzewie!");
                    }else if (result==-1){
                        System.out.println("Wybrany klucz nie ma poprzednika.");
                    }else{
                        System.out.println("Poprzednik wybranego klucza: "+result);
                    }
                    break;
                }
                case 8:{
                    System.out.println("Podaj klucz do usunięcia: ");
                    int key = Integer.parseInt(input.nextLine());
                    if (bst.search(key))bst.remove(key);
                    else System.out.println("Brak wybranego klucza w drzewie!");
                    break;
                }
                case 9:{
                    bst.heightAndNodesForAll();
                    break;
                }
                case 10:{
                    bst.displayInfo();
                    break;
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
