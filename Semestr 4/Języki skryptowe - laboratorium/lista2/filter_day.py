#filter_day.py

import sys

def filter_day(day):
    for line in sys.stdin:
        tokenized = line.split()
        try:
            date_day = int(tokenized[3][1:3])
            if date_day == day:
                sys.stdout.write(line)
        except:
            pass

if __name__ == "__main__":
    filter_day(int(sys.argv[1]))
