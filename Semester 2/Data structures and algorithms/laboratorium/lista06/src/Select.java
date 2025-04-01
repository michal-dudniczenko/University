public class Select {

    public static void sort(int[] arr, int[] wynik) {
        int n = arr.length;

        for (int i=0; i<n-1; i++){
            int min=i;
            for (int j=i+1; j<n; j++){
                wynik[0]++;
                if (arr[j]<arr[min]){
                    min=j;
                }
            }
            swap(arr, i, min);
            wynik[1]+=3;
        }
    }

    private static void swap(int[] arr, int i, int j){
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }
}
