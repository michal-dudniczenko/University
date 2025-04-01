import java.util.Arrays;
import java.util.Random;
import java.util.Collections;

public class Main {
    public static void main(String[] args){
        Random generator = new Random();
        Integer[] tab8 = new Integer[8];
        Integer[] tab32 = new Integer[32];
        Integer[] tab128 = new Integer[128];
        Integer[] tab512 = new Integer[512];
        Integer[] tab2048 = new Integer[2048];

        for (int i=0; i<8; i++){
            tab8[i]=generator.nextInt(100)+1;
        }
        for (int i=0; i<32; i++){
            tab32[i]=generator.nextInt(100)+1;
        }
        for (int i=0; i<128; i++){
            tab128[i]=generator.nextInt(100)+1;
        }
        for (int i=0; i<512; i++){
            tab512[i]=generator.nextInt(100)+1;
        }
        for (int i=0; i<2048; i++){
            tab2048[i]=generator.nextInt(100)+1;
        }

        //sortowania random

        System.out.println("----------------------------------------------------------------");
        System.out.println("Wyniki sortowania dla tablicy losowych liczb całkowitych:");

        int[]wynikiSelect8=Algorytmy.selectSort(tab8);
        System.out.println("\nselect8: "+"porownania: "+wynikiSelect8[0]+" przepisania: "+wynikiSelect8[1]);
        int[]wynikiSelect32=Algorytmy.selectSort(tab32);
        System.out.println("select32: "+"porownania: "+wynikiSelect32[0]+" przepisania: "+wynikiSelect32[1]);
        int[]wynikiSelect128=Algorytmy.selectSort(tab128);
        System.out.println("select128: "+"porownania: "+wynikiSelect128[0]+" przepisania: "+wynikiSelect128[1]);
        int[]wynikiSelect512=Algorytmy.selectSort(tab512);
        System.out.println("select512: "+"porownania: "+wynikiSelect512[0]+" przepisania: "+wynikiSelect512[1]);
        int[]wynikiSelect2048=Algorytmy.selectSort(tab2048);
        System.out.println("select2048: "+"porownania: "+wynikiSelect2048[0]+" przepisania: "+wynikiSelect2048[1]);

        int[]wynikiInsert8=Algorytmy.insertSort(tab8);
        System.out.println("\ninsert8: "+"porownania: "+wynikiInsert8[0]+" przepisania: "+wynikiInsert8[1]);
        int[]wynikiInsert32=Algorytmy.insertSort(tab32);
        System.out.println("insert32: "+"porownania: "+wynikiInsert32[0]+" przepisania: "+wynikiInsert32[1]);
        int[]wynikiInsert128=Algorytmy.insertSort(tab128);
        System.out.println("insert128: "+"porownania: "+wynikiInsert128[0]+" przepisania: "+wynikiInsert128[1]);
        int[]wynikiInsert512=Algorytmy.insertSort(tab512);
        System.out.println("insert512: "+"porownania: "+wynikiInsert512[0]+" przepisania: "+wynikiInsert512[1]);
        int[]wynikiInsert2048=Algorytmy.insertSort(tab2048);
        System.out.println("insert2048: "+"porownania: "+wynikiInsert2048[0]+" przepisania: "+wynikiInsert2048[1]);

        int[]wynikiMerge8=Algorytmy.mergeSort(tab8);
        System.out.println("\nmerge8: "+"porownania: "+wynikiMerge8[0]+" przepisania: "+wynikiMerge8[1]);
        int[]wynikiMerge32=Algorytmy.mergeSort(tab32);
        System.out.println("merge32: "+"porownania: "+wynikiMerge32[0]+" przepisania: "+wynikiMerge32[1]);
        int[]wynikiMerge128=Algorytmy.mergeSort(tab128);
        System.out.println("merge128: "+"porownania: "+wynikiMerge128[0]+" przepisania: "+wynikiMerge128[1]);
        int[]wynikiMerge512=Algorytmy.mergeSort(tab512);
        System.out.println("merge512: "+"porownania: "+wynikiMerge512[0]+" przepisania: "+wynikiMerge512[1]);
        int[]wynikiMerge2048=Algorytmy.mergeSort(tab2048);
        System.out.println("merge2048: "+"porownania: "+wynikiMerge2048[0]+" przepisania: "+wynikiMerge2048[1]);

        int[]wynikiQuick8=Algorytmy.quickSort(tab8);
        System.out.println("\nquick8: "+"porownania: "+wynikiQuick8[0]+" przepisania: "+wynikiQuick8[1]);
        int[]wynikiQuick32=Algorytmy.quickSort(tab32);
        System.out.println("quick32: "+"porownania: "+wynikiQuick32[0]+" przepisania: "+wynikiQuick32[1]);
        int[]wynikiQuick128=Algorytmy.quickSort(tab128);
        System.out.println("quick128: "+"porownania: "+wynikiQuick128[0]+" przepisania: "+wynikiQuick128[1]);
        int[]wynikiQuick512=Algorytmy.quickSort(tab512);
        System.out.println("quick512: "+"porownania: "+wynikiQuick512[0]+" przepisania: "+wynikiQuick512[1]);
        int[]wynikiQuick2048=Algorytmy.quickSort(tab2048);
        System.out.println("quick2048: "+"porownania: "+wynikiQuick2048[0]+" przepisania: "+wynikiQuick2048[1]);
        
        //sortowania sorted

        Arrays.sort(tab8);
        Arrays.sort(tab32);
        Arrays.sort(tab128);
        Arrays.sort(tab512);
        Arrays.sort(tab2048);

        System.out.println("----------------------------------------------------------------");
        System.out.println("Wyniki sortowania dla tablicy posortowanych liczb całkowitych:");

        wynikiSelect8=Algorytmy.selectSort(tab8);
        System.out.println("\nselect8: "+"porownania: "+wynikiSelect8[0]+" przepisania: "+wynikiSelect8[1]);
        wynikiSelect32=Algorytmy.selectSort(tab32);
        System.out.println("select32: "+"porownania: "+wynikiSelect32[0]+" przepisania: "+wynikiSelect32[1]);
        wynikiSelect128=Algorytmy.selectSort(tab128);
        System.out.println("select128: "+"porownania: "+wynikiSelect128[0]+" przepisania: "+wynikiSelect128[1]);
        wynikiSelect512=Algorytmy.selectSort(tab512);
        System.out.println("select512: "+"porownania: "+wynikiSelect512[0]+" przepisania: "+wynikiSelect512[1]);
        wynikiSelect2048=Algorytmy.selectSort(tab2048);
        System.out.println("select2048: "+"porownania: "+wynikiSelect2048[0]+" przepisania: "+wynikiSelect2048[1]);

        wynikiInsert8=Algorytmy.insertSort(tab8);
        System.out.println("\ninsert8: "+"porownania: "+wynikiInsert8[0]+" przepisania: "+wynikiInsert8[1]);
        wynikiInsert32=Algorytmy.insertSort(tab32);
        System.out.println("insert32: "+"porownania: "+wynikiInsert32[0]+" przepisania: "+wynikiInsert32[1]);
        wynikiInsert128=Algorytmy.insertSort(tab128);
        System.out.println("insert128: "+"porownania: "+wynikiInsert128[0]+" przepisania: "+wynikiInsert128[1]);
        wynikiInsert512=Algorytmy.insertSort(tab512);
        System.out.println("insert512: "+"porownania: "+wynikiInsert512[0]+" przepisania: "+wynikiInsert512[1]);
        wynikiInsert2048=Algorytmy.insertSort(tab2048);
        System.out.println("insert2048: "+"porownania: "+wynikiInsert2048[0]+" przepisania: "+wynikiInsert2048[1]);

        wynikiMerge8=Algorytmy.mergeSort(tab8);
        System.out.println("\nmerge8: "+"porownania: "+wynikiMerge8[0]+" przepisania: "+wynikiMerge8[1]);
        wynikiMerge32=Algorytmy.mergeSort(tab32);
        System.out.println("merge32: "+"porownania: "+wynikiMerge32[0]+" przepisania: "+wynikiMerge32[1]);
        wynikiMerge128=Algorytmy.mergeSort(tab128);
        System.out.println("merge128: "+"porownania: "+wynikiMerge128[0]+" przepisania: "+wynikiMerge128[1]);
        wynikiMerge512=Algorytmy.mergeSort(tab512);
        System.out.println("merge512: "+"porownania: "+wynikiMerge512[0]+" przepisania: "+wynikiMerge512[1]);
        wynikiMerge2048=Algorytmy.mergeSort(tab2048);
        System.out.println("merge2048: "+"porownania: "+wynikiMerge2048[0]+" przepisania: "+wynikiMerge2048[1]);

        wynikiQuick8=Algorytmy.quickSort(tab8);
        System.out.println("\nquick8: "+"porownania: "+wynikiQuick8[0]+" przepisania: "+wynikiQuick8[1]);
        wynikiQuick32=Algorytmy.quickSort(tab32);
        System.out.println("quick32: "+"porownania: "+wynikiQuick32[0]+" przepisania: "+wynikiQuick32[1]);
        wynikiQuick128=Algorytmy.quickSort(tab128);
        System.out.println("quick128: "+"porownania: "+wynikiQuick128[0]+" przepisania: "+wynikiQuick128[1]);
        wynikiQuick512=Algorytmy.quickSort(tab512);
        System.out.println("quick512: "+"porownania: "+wynikiQuick512[0]+" przepisania: "+wynikiQuick512[1]);
        wynikiQuick2048=Algorytmy.quickSort(tab2048);
        System.out.println("quick2048: "+"porownania: "+wynikiQuick2048[0]+" przepisania: "+wynikiQuick2048[1]);


        //sortowania reversed
        
        Arrays.sort(tab8, Collections.reverseOrder());
        Arrays.sort(tab32, Collections.reverseOrder());
        Arrays.sort(tab128, Collections.reverseOrder());
        Arrays.sort(tab512, Collections.reverseOrder());
        Arrays.sort(tab2048, Collections.reverseOrder());

        System.out.println("--------------------------------------------------------------");
        System.out.println("Wyniki sortowania dla tablicy posortowanych odwrotnie liczb całkowitych:");

        wynikiSelect8=Algorytmy.selectSort(tab8);
        System.out.println("\nselect8: "+"porownania: "+wynikiSelect8[0]+" przepisania: "+wynikiSelect8[1]);
        wynikiSelect32=Algorytmy.selectSort(tab32);
        System.out.println("select32: "+"porownania: "+wynikiSelect32[0]+" przepisania: "+wynikiSelect32[1]);
        wynikiSelect128=Algorytmy.selectSort(tab128);
        System.out.println("select128: "+"porownania: "+wynikiSelect128[0]+" przepisania: "+wynikiSelect128[1]);
        wynikiSelect512=Algorytmy.selectSort(tab512);
        System.out.println("select512: "+"porownania: "+wynikiSelect512[0]+" przepisania: "+wynikiSelect512[1]);
        wynikiSelect2048=Algorytmy.selectSort(tab2048);
        System.out.println("select2048: "+"porownania: "+wynikiSelect2048[0]+" przepisania: "+wynikiSelect2048[1]);

        wynikiInsert8=Algorytmy.insertSort(tab8);
        System.out.println("\ninsert8: "+"porownania: "+wynikiInsert8[0]+" przepisania: "+wynikiInsert8[1]);
        wynikiInsert32=Algorytmy.insertSort(tab32);
        System.out.println("insert32: "+"porownania: "+wynikiInsert32[0]+" przepisania: "+wynikiInsert32[1]);
        wynikiInsert128=Algorytmy.insertSort(tab128);
        System.out.println("insert128: "+"porownania: "+wynikiInsert128[0]+" przepisania: "+wynikiInsert128[1]);
        wynikiInsert512=Algorytmy.insertSort(tab512);
        System.out.println("insert512: "+"porownania: "+wynikiInsert512[0]+" przepisania: "+wynikiInsert512[1]);
        wynikiInsert2048=Algorytmy.insertSort(tab2048);
        System.out.println("insert2048: "+"porownania: "+wynikiInsert2048[0]+" przepisania: "+wynikiInsert2048[1]);

        wynikiMerge8=Algorytmy.mergeSort(tab8);
        System.out.println("\nmerge8: "+"porownania: "+wynikiMerge8[0]+" przepisania: "+wynikiMerge8[1]);
        wynikiMerge32=Algorytmy.mergeSort(tab32);
        System.out.println("merge32: "+"porownania: "+wynikiMerge32[0]+" przepisania: "+wynikiMerge32[1]);
        wynikiMerge128=Algorytmy.mergeSort(tab128);
        System.out.println("merge128: "+"porownania: "+wynikiMerge128[0]+" przepisania: "+wynikiMerge128[1]);
        wynikiMerge512=Algorytmy.mergeSort(tab512);
        System.out.println("merge512: "+"porownania: "+wynikiMerge512[0]+" przepisania: "+wynikiMerge512[1]);
        wynikiMerge2048=Algorytmy.mergeSort(tab2048);
        System.out.println("merge2048: "+"porownania: "+wynikiMerge2048[0]+" przepisania: "+wynikiMerge2048[1]);

        wynikiQuick8=Algorytmy.quickSort(tab8);
        System.out.println("\nquick8: "+"porownania: "+wynikiQuick8[0]+" przepisania: "+wynikiQuick8[1]);
        wynikiQuick32=Algorytmy.quickSort(tab32);
        System.out.println("quick32: "+"porownania: "+wynikiQuick32[0]+" przepisania: "+wynikiQuick32[1]);
        wynikiQuick128=Algorytmy.quickSort(tab128);
        System.out.println("quick128: "+"porownania: "+wynikiQuick128[0]+" przepisania: "+wynikiQuick128[1]);
        wynikiQuick512=Algorytmy.quickSort(tab512);
        System.out.println("quick512: "+"porownania: "+wynikiQuick512[0]+" przepisania: "+wynikiQuick512[1]);
        wynikiQuick2048=Algorytmy.quickSort(tab2048);
        System.out.println("quick2048: "+"porownania: "+wynikiQuick2048[0]+" przepisania: "+wynikiQuick2048[1]);

        System.out.println("------------------------------------------------------------");
    }
}
