import java.util.HashMap;

public class Huffman {
    private static class Node{
        private char character;
        private final int freq;
        private final boolean isChar;
        private Node left;
        private Node right;

        public Node(char character, int freq){
            this.character=character;
            this.freq=freq;
            this.isChar=true;
        }

        public Node(int freq){
            this.freq=freq;
            this.isChar=false;
        }

        public String toString(){
            return this.character+" ("+this.freq+")";
        }
    }

    private static class PriorityQueue{
        private Node[] heap=new Node[100];
        private int size=0;

        public PriorityQueue(char[] characters, int[] freq){
            int n = characters.length;

            for (int i=0; i<n; i++){
                enqueue(new Node(characters[i], freq[i]));
            }
        }

        private void swap(int i, int j){
            Node temp = heap[i];
            heap[i]=heap[j];
            heap[j]=temp;
        }

        public void enqueue(Node node){
            heap[size]=node;
            swim(size);
            size++;
        }

        public Node dequeue(){
            Node result=heap[0];
            heap[0]=heap[size-1];
            size--;
            sink(0);
            return result;
        }

        private void swim(int i){
            int parent;
            while ((parent=(i-1)/2)>=0 && heap[parent].freq>heap[i].freq){
                swap(i, parent);
                i=parent;
            }
        }

        private void sink(int i){
                int smallest=i;
                int left=i*2+1;
                int right=i*2+2;

                if (left<size && heap[smallest].freq>heap[left].freq){
                    smallest=left;
                }
                if (right<size && heap[smallest].freq>heap[right].freq){
                    smallest=right;
                }

                if (smallest!=i){
                    swap(i, smallest);
                    sink(smallest);
                }
        }

        public void display(){
            System.out.print("[");
            for (int i=0; i<size; i++){
                System.out.print(heap[i].freq+" ");
            }
            System.out.print("]\n");
        }

    }

    private static Node getCodeTree(char[] characters, int[] freq){
        PriorityQueue pq = new PriorityQueue(characters, freq);
        int n = characters.length;
        for (int i=1; i<n; i++){
            Node x = pq.dequeue();
            Node y = pq.dequeue();
            Node z = new Node(x.freq+y.freq);
            z.left=x;
            z.right=y;
            pq.enqueue(z);
        }
        return pq.dequeue();
    }

    private static void displayCodes(Node node, String s){
        if (node.isChar){
            System.out.println(node.character+": "+s);
        }
        else {
            displayCodes(node.left, s+"0");
            displayCodes(node.right, s+"1");
        }
    }
    public static void displayCodes(char[] characters, int[] freq){
        Node tree = getCodeTree(characters, freq);
        displayCodes(tree, "");
    }

    public static String codeText(String text, char[] characters, int[] freq){
        Node tree = getCodeTree(characters, freq);
        HashMap<Character, String> codeMap = getCodeMap(tree);
        String result="";
        for (char c : text.toCharArray()){
            result+=codeMap.get(c);
        }
        return result;
    }

    private static HashMap<Character, String> getCodeMap(Node node){
        HashMap<Character, String> result = new HashMap<>();
        getCodeMap(node, result, "");
        return result;
    }
    private static void getCodeMap(Node node, HashMap<Character, String> hm, String s){
        if (node.isChar){
            hm.put(node.character, s);
        }
        else {
            getCodeMap(node.left, hm, s+"0");
            getCodeMap(node.right, hm, s+"1");
        }
    }

    public static String decodeText(String text, char[] characters, int[] freq){
        Node tree = getCodeTree(characters, freq);
        String result="";
        Node node=tree;
        for (char c : text.toCharArray()){
            if (c=='0'){
                node=node.left;
            }
            else{
                node=node.right;
            }

            if (node.isChar){
                result+=node.character;
                node=tree;
            }
        }
        return result;
    }
}
