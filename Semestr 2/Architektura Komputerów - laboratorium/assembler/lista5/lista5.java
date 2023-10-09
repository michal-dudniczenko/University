import java.util.Scanner;

public class lista5 {
    public static void main(String[] args){
        Scanner input = new Scanner(System.in);
        int wybor=-1;

        while (wybor!=0) {
            System.out.println("1 - policz wartosc wielomianu\t0 - zakoncz");
            wybor=Integer.parseInt(input.nextLine());
            if (wybor==0) continue;

            float[] wspolczynniki = {2.3f, 4.5f, 7.8f, 1.3f};
            int stopien = wspolczynniki.length - 1;

            System.out.print("\nPodaj wartość x: ");
            double x = Double.parseDouble(input.nextLine());

            double wynik = wspolczynniki[0];

            for (int i = 1; i <= stopien; i++) {
                wynik *= x;
                wynik += wspolczynniki[i];
            }

            System.out.println("\nWartość wielomianu = " + wynik+"\n");
        }
    }
}
