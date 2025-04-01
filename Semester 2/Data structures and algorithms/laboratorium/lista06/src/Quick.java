import java.util.Random;

public class Quick {

    public static void sort(int[] arr, int[] wynik){
        sort(arr, 0, arr.length-1, wynik);
    }

    private static void sort(int[] arr, int left, int right, int[] wynik){
        if (left<right){
            int pivot=partition(arr, left, right, wynik);

            sort(arr, left, pivot-1, wynik);
            sort(arr, pivot+1, right, wynik);
        }
    }

    private static int partition(int[] arr, int left, int right, int[] wynik){
        randomizePivot(arr, left, right);
        wynik[1]+=3;
        int pivot=arr[right];
        wynik[1]++;

        int smaller=left-1;

        for (int j=left; j<right; j++){
            wynik[0]++;
            if (arr[j]<pivot){
                smaller++;
                swap(arr, smaller, j);
                wynik[1]+=3;
            }
        }
        swap(arr, right, smaller+1);
        wynik[1]+=3;
        return (smaller+1);
    }

    private static void randomizePivot(int[] arr, int left, int right){
        Random generator = new Random();
        swap(arr, right, generator.nextInt(left, right+1));
    }

    private static void swap(int[] arr, int i, int j){
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }
}
