import java.util.Scanner;

public class SmithWaterman {
    public static String SmithWaterman(String s1, String s2, int penalty){
        int n = s1.length()+1;
        int m = s2.length()+1;

        int iMax=0;
        int jMax=0;
        int scoreMax=-1;

        int[][] matrix = new int[n][m];
        int[][] iFrom = new int[n][m];
        int[][] jFrom = new int[n][m];

        for (int i=0; i<n; i++){
            for (int j=0; j<m; j++){
                if (i==0 || j==0){
                    matrix[i][j]=0;
                }
                else {
                    int iFromTemp=-1;
                    int jFromTemp=-1;
                    int fill = 0;
                    int left = matrix[i][j - 1] - penalty;
                    if (left > fill){
                        fill = left;
                        iFromTemp=i;
                        jFromTemp=j-1;
                    }
                    int top = matrix[i - 1][j] - penalty;
                    if (top > fill){
                        fill = top;
                        iFromTemp=i-1;
                        jFromTemp=j;
                    }
                    int score = -1;
                    if (s1.charAt(i - 1) == s2.charAt(j - 1)) {
                        score = 1;
                    }
                    int diag = matrix[i - 1][j - 1] + score;
                    if (diag > fill){
                        fill = diag;
                        iFromTemp=i-1;
                        jFromTemp=j-1;
                    }
                    matrix[i][j] = fill;
                    iFrom[i][j]=iFromTemp;
                    jFrom[i][j]=jFromTemp;
                }
                if (matrix[i][j]>scoreMax){
                    scoreMax=matrix[i][j];
                    iMax=i;
                    jMax=j;
                }
            }
        }
        String result="";

        while (matrix[iMax][jMax]>0){
            int iTemp = iFrom[iMax][jMax];
            int jTemp = jFrom[iMax][jMax];
            if (iTemp==iMax-1 && jTemp==jMax-1){
                result=s1.charAt(iMax-1)+result;
            }
            iMax=iTemp;
            jMax=jTemp;
        }
        return result;
    }

    public static void main(String[] args){
        Scanner input = new Scanner(System.in);
        String s1,s2;
        int gapPenalty;

        System.out.println("Podaj sekwencje 1: ");
        s1=input.nextLine();
        System.out.println("Podaj sekwencje 2: ");
        s2=input.nextLine();
        System.out.println("Podaj gap penalty: ");
        gapPenalty=Integer.parseInt(input.nextLine());

        System.out.println(SmithWaterman(s1, s2, gapPenalty));
    }
}
