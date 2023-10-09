import java.util.Random;

public class Searches {
    private class HeapSort{
        public static void sort(int[] arr){
            int n = arr.length;

            for (int i=n/2-1; i>=0; i--){
                heapify(arr, n, i);
            }

            for (int i = n-1; i>0; i--){
                swap(arr, i, 0);
                heapify(arr, i, 0);
            }
        }

        private static void heapify(int[] arr, int heapSize, int node){
            int largest = node;
            int left = 2 * node + 1;
            int right = 2 * node + 2;

            if (left<heapSize && arr[left]>arr[largest]){
                largest=left;
            }
            if (right<heapSize && arr[right]>arr[largest]){
                largest=right;
            }

            if (largest!=node){
                swap(arr, largest, node);
                heapify(arr, heapSize, largest);
            }
        }

        private static void swap(int[] arr, int n1, int n2){
            int swap=arr[n1];
            arr[n1]=arr[n2];
            arr[n2]=swap;
        }
    }

    public static void fillRandomly(int[] arr, int z1, int z2){
        Random generator = new Random();
        int n = arr.length;
        for (int i=0; i<n; i++){
            arr[i]=generator.nextInt(z1, z2+1);
        }
    }

    public static int linearSearch(int[] arr, int n, int[] trafione, int[] chybione){
        int porownania=0;

        int i=0;
        while(i<arr.length){
            porownania++;
            if (arr[i]==n){
                trafione[0]++;
                trafione[1]+=porownania;
                return i;
            }
            i++;
        }
        chybione[0]++;
        chybione[1]+=porownania;
        return -1;
    }

    public static void heapsort(int[] arr){
        HeapSort.sort(arr);
    }

    public static int binarySearch(int[] arr, int n, int[] trafione, int[] chybione){
        int porownania=0;

        int left=0, right=arr.length-1, middle;

        while(left<=right){
            middle= left + (right-left)/2;
            porownania++;
            if (arr[middle]==n){
                trafione[0]++;
                trafione[1]+=porownania;
                return middle;
            }
            porownania++;
            if (arr[middle]<n)left=middle+1;
            else right=middle-1;
        }
        chybione[0]++;
        chybione[1]+=porownania;
        return -1;
    }

    public static void main(String[] args){
        int[] arr = new int[1000];

        fillRandomly(arr, 1, 1000);

        System.out.println("Wyszukiwanie liniowe 1000 losowych wartości w tablicy 1000 losowych liczb całkowitych:");
        int[] trafione = new int[2];
        int[] chybione = new int[2];


        Random generator = new Random();
        for (int i=0; i<1000; i++){
            linearSearch(arr, generator.nextInt(1, 1001), trafione, chybione);
        }
        System.out.println("Liczba wyszukań trafionych: "+trafione[0]);
        System.out.println("Średnia liczba porównań: "+trafione[1]/trafione[0]);

        System.out.println("Liczba wyszukań chybionych: "+chybione[0]);
        System.out.println("Średnia liczba porównań: "+chybione[1]/chybione[0]);

        System.out.println("\nŁączna średnia liczba porównań: "+(trafione[1]+chybione[1])/1000);

        System.out.println("-------------------------------------");
        System.out.println("sortowanie tablicy metodą heapsort");
        System.out.println("-------------------------------------");

        System.out.println("Wyszukiwanie binarne 1000 losowych wartości w tablicy 1000 posortowanych liczb całkowitych:");

        heapsort(arr);

        trafione = new int[2];
        chybione = new int[2];

        for (int i=0; i<1000; i++){
            binarySearch(arr, generator.nextInt(1, 1001), trafione, chybione);
        }
        System.out.println("Liczba wyszukań trafionych: "+trafione[0]);
        System.out.println("Średnia liczba porównań: "+trafione[1]/trafione[0]);

        System.out.println("Liczba wyszukań chybionych: "+chybione[0]);
        System.out.println("Średnia liczba porównań: "+chybione[1]/chybione[0]);

        System.out.println("\nŁączna średnia liczba porównań: "+(trafione[1]+chybione[1])/1000);
    }
}
