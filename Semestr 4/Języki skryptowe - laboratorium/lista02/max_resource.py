#max_resource.py

import sys

def max_resource():
    max_size = -1
    max_path = ""
    
    for line in sys.stdin:
        tokenized = line.split()
        try:
            if int(tokenized[-1]) > max_size:
                max_size = int(tokenized[-1])
                max_path = tokenized[-4]
        except:
            pass
    
    print(f"Sciezka zasobu o najwiekszym rozmiarze = {max_size} [B] \"{max_path}\"")

if __name__ == "__main__":
    max_resource()
