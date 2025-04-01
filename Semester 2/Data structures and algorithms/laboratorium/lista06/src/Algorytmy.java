public class Algorytmy {
    public static int[] selectSort(Integer[] array) {
        int[] arrayTemp = new int[array.length];
        for (int i = 0; i < array.length; i++)
            arrayTemp[i] = array[i];

        int[] wynik = new int[2];

        Select.sort(arrayTemp, wynik);

        return wynik;
    }

    public static int[] insertSort(Integer[] array) {
        int[] arrayTemp = new int[array.length];
        for (int i = 0; i < array.length; i++)
            arrayTemp[i] = array[i];

        int[] wynik = new int[2];

        Insert.sort(arrayTemp, wynik);

        return wynik;
    }

    public static int[] mergeSort(Integer[] array) {
        int[] arrayTemp = new int[array.length];
        for (int i = 0; i < array.length; i++)
            arrayTemp[i] = array[i];

        int[] wynik = new int[2];

        Merge.sort(arrayTemp, wynik);

        return wynik;
    }

    public static int[] quickSort(Integer[] array) {
        int[] arrayTemp = new int[array.length];
        for (int i = 0; i < array.length; i++)
            arrayTemp[i] = array[i];

        int[] wynik = new int[2];

        Quick.sort(arrayTemp, wynik);

        return wynik;
    }
}
