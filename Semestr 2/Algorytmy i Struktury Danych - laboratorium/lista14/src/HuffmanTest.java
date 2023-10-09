public class HuffmanTest {
    public static void main(String[] args) {
        char[] characters = new char[]{'a', 'b', 'c', 'd', 'e', 'f'};
        int[] freq = new int[]{45, 13, 12, 16, 9, 5};

        Huffman.displayCodes(characters, freq);

        String text = "abc";
        System.out.println("text: " + text);
        String coded = Huffman.codeText(text, characters, freq);
        System.out.println("coded: " + coded);
        String decoded = Huffman.decodeText(coded, characters, freq);
        System.out.println("decoded: " + decoded);
    }
}
