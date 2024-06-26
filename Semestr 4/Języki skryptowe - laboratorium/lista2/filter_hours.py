#filter_hours.py

import sys

def filter_hours(start, end):
    for line in sys.stdin:
        tokenized = line.split()
        try:
            hour = int(tokenized[3].split(":")[1])

            if start < end:
                if start <= hour < end:
                    sys.stdout.write(line)
            elif start > end:
                if start <= hour < 24 or 0 <= hour < end:
                    sys.stdout.write(line)

        except:
            pass

if __name__ == "__main__":
    filter_hours(int(sys.argv[1]), int(sys.argv[2]))
