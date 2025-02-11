public class Insert {

    public static void sort(int[] arr, int[] wynik){
        int n = arr.length;

        for (int i=1; i<n; i++){
            int temp=arr[i];
            wynik[1]++;

            int j=i-1;
            while((wynik[0]++ > 0) && j>=0 && arr[j]>temp){
                arr[j+1]=arr[j];
                wynik[1]++;
                j--;
            }
            arr[j+1]=temp;
            wynik[1]++;
        }
    }
}
