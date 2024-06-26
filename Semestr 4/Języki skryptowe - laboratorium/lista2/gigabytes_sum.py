#gigabytes_sum.py

import sys

def gigabytes_sum():
    sum = 0
    for line in sys.stdin:
        tokenized = line.split()
        try:
            sum += int(tokenized[-1])
        except:
            pass

    print(f"Laczna liczba danych wyslanych do hostow: {round((sum / 1024 / 1024 / 1024), 2)} [GB]")

if __name__ == "__main__":
    gigabytes_sum()
