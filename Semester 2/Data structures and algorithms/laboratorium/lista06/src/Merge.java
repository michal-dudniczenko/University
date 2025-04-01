public class Merge {

    public static void sort(int[] arr, int[] wynik){
        sort(arr, 0, arr.length-1, wynik);
    }

    private static void sort(int[] arr, int left, int right, int[] wynik){
        if (left<right){
            int middle = left + (right-left)/2;

            sort(arr, left, middle, wynik);
            sort(arr, middle+1, right, wynik);

            merge(arr, left, middle, right, wynik);
        }
    }

    public static void merge(int[] arr, int left, int middle, int right, int[] wynik){
        int n1 = middle-left+1;
        int n2 = right-middle;

        int[] L = new int[n1];
        int[] R = new int[n2];

        for (int i=0; i<n1; i++){
            L[i]=arr[left+i];
            wynik[1]++;
        }
        for (int i=0; i<n2; i++){
            R[i]=arr[middle+1+i];
            wynik[1]++;
        }

        int i=0, j=0, k=left;

        while(i<n1 && j<n2){
            if (L[i]<=R[j]){
                arr[k]=L[i];
                i++;
            }else{
                arr[k]=R[j];
                j++;
            }
            wynik[0]++;
            wynik[1]++;
            k++;
        }

        while (i<n1){
            arr[k]=L[i];
            i++;
            k++;
            wynik[1]++;
        }
        while (j<n2){
            arr[k]=R[j];
            j++;
            k++;
            wynik[1]++;
        }
    }
}
