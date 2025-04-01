import sys
import subprocess
import os



def main():
    if len(sys.argv) != 2:
        print("Usage: python zadanie_4.py <path to directory>")
        sys.exit()

    path_to_dir = sys.argv[1]

    if not path_to_dir.endswith("\\"):
        path_to_dir += "\\"

    try:
        list_of_dict = []


        for file in os.listdir(path_to_dir):
            if file.endswith(".txt"):
                output = subprocess.run(["java", "Zadanie_4", path_to_dir + file], capture_output=True).stdout.decode('utf-8').rstrip()
                output_split = output.split("\t")

                output_as_dict = {}

                output_as_dict["file_path"] = output_split[0]
                output_as_dict["char_count"] = int(output_split[1])
                output_as_dict["words_count"] = int(output_split[2])
                output_as_dict["lines_count"] = int(output_split[3])
                output_as_dict["most_frequent_char"] = output_split[4]
                output_as_dict["most_frequent_char_count"] = int(output_split[5])
                output_as_dict["most_frequent_word"] = output_split[6]
                output_as_dict["most_frequent_word_count"] = int(output_split[7])

                list_of_dict.append(output_as_dict)


        files_read_count = len(list_of_dict)
        char_sum = 0
        words_sum = 0
        lines_sum = 0
        most_freq_char_globally = ""
        most_freq_word_globally = ""
        most_freq_char_count_globally = 0
        most_freq_word_count_globally = 0

        most_frequent_characters = {}
        most_frequent_words = {}

        for data_dict in list_of_dict:
            char_sum += data_dict["char_count"]
            words_sum += data_dict["words_count"]
            lines_sum += data_dict["lines_count"]

            char = data_dict["most_frequent_char"]
            char_count = data_dict["most_frequent_char_count"]

            word = data_dict["most_frequent_word"]
            word_count = data_dict["most_frequent_word_count"]

            most_frequent_characters[char] = most_frequent_characters.setdefault(char, 0) + char_count
            most_frequent_words[word] = most_frequent_words.setdefault(word, 0) + word_count
        
        for key,value in most_frequent_characters.items():
            if value > most_freq_char_count_globally:
                most_freq_char_count_globally = value
                most_freq_char_globally = key
        
        for key,value in most_frequent_words.items():
            if value > most_freq_word_count_globally:
                most_freq_word_count_globally = value
                most_freq_word_globally = key


        print("\n--------------------------------------\n")
        print(f"Liczba przeczytanych plików: {files_read_count}")
        print(f"Sumaryczna liczba znaków: {char_sum}")
        print(f"Sumaryczna liczba słów: {words_sum}")
        print(f"Sumaryczna liczba wierszy: {lines_sum}")
        print(f"Znak występujący najczęściej globalnie: \"{most_freq_char_globally}\", wystepuje {most_freq_char_count_globally} razy")
        print(f"Słowo występujące najczęściej globalnie: \"{most_freq_word_globally}\", wystepuje {most_freq_word_count_globally} razy")

        print("\n--------------------------------------\n")

    except Exception as e:
        print(e)



if __name__ == "__main__":
    main()