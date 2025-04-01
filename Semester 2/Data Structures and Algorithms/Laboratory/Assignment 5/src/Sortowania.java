import java.util.Comparator;

public class Sortowania{
    public static Samochod[] bubbleSort(Samochod[] array, Comparator<Samochod> comp){
        Samochod[] wynik = array.clone();
        int n = wynik.length;
        for (int i=0; i<n-1; i++){
            for (int j=0; j<n-1-i; j++){
                if (comp.compare(wynik[j], wynik[j+1])>0){
                    Samochod temp = wynik[j];
                    wynik[j]=wynik[j+1];
                    wynik[j+1]=temp;
                }
            }
        }
        return wynik;
    }

    public static Samochod[] insertionSort(Samochod[] array, Comparator<Samochod> comp) {
        Samochod[] wynik = array.clone();
        int n = wynik.length;
        for (int i = 1; i < n; i++) {
            for (int j=i; j>0 && comp.compare(wynik[j-1], wynik[j])>0; j--){
                Samochod temp = wynik[j];
                wynik[j]=wynik[j-1];
                wynik[j-1]=temp;
            }
        }
        return wynik;
    }

    public static Samochod[] selectionSort(Samochod[] array, Comparator<Samochod> comp){
        Samochod[] wynik = array.clone();
        int n = wynik.length;
        for (int i = 0; i < n-1; i++)
        {
            int indexMin = i;
            for (int j = i+1; j < n; j++)
                if (comp.compare(wynik[j], wynik[indexMin])<0)
                    indexMin = j;

            Samochod temp = wynik[indexMin];
            wynik[indexMin] = wynik[i];
            wynik[i] = temp;
        }
        return wynik;
    }
}
