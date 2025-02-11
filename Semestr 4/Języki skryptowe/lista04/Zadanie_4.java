import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.util.HashMap;

public class Zadanie_4 {

    public static void main(String[] args) {
        
        String file_path = args[0];

        try {
            BufferedReader br = new BufferedReader(new FileReader(file_path));
            File file = new File(file_path);

            String absolute_path = file.getAbsolutePath();

            int char_count = 0;
            int word_count = 0;
            int line_count = 0;
            HashMap<Character, Integer> char_frequency = new HashMap<>();
            HashMap<String, Integer> word_frequency = new HashMap<>();

            String line;

            while ((line = br.readLine()) != null){

                String[] words_tokens = line.split("\\s+");

                char_count += line.length();
                word_count += words_tokens.length;
                line_count++;

                for (char c : line.toCharArray()){
                    char_frequency.put(c, char_frequency.getOrDefault(c, 0) + 1);
                }

                for (String word : words_tokens) {
                    word = word.replaceAll("[^a-zA-Z0-9]", "");

                    word_frequency.put(word, word_frequency.getOrDefault(word, 0) + 1);
                }
            }

            Object[] result_freq_char = find_most_frequent(char_frequency);
            Object[] result_freq_word = find_most_frequent(word_frequency);

            char most_frequent_char = (Character) result_freq_char[0];
            int most_frequent_char_count = (Integer) result_freq_char[1];
            String most_frequent_word = (String) result_freq_word[0];
            int most_frequent_word_count = (Integer) result_freq_word[1];

            System.out.println(absolute_path + "\t" + char_count + "\t" + word_count + "\t" + line_count + "\t" + most_frequent_char + "\t"  + most_frequent_char_count + "\t" + most_frequent_word + "\t"  + most_frequent_word_count);

            br.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }



    public static <T> Object[] find_most_frequent(HashMap<T, Integer> freqMap){
        
        T mostFreqElement = null;
        int maxFreq = 0;
        
        for (HashMap.Entry<T, Integer> entry : freqMap.entrySet()){
            T key = entry.getKey();
            int value = entry.getValue();
        
            if (value > maxFreq){
                mostFreqElement = key;
                maxFreq = value;
            }
        }

        return new Object[] {mostFreqElement, maxFreq};
    }
    
}