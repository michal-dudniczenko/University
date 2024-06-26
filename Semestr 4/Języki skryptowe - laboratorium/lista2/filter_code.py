#filter.code.py

import sys

def filter_code(code):
    for line in sys.stdin:
        tokenized = line.split()
        try:
            if int(tokenized[-2]) == code:
                sys.stdout.write(line)
        except:
            pass

if __name__ == "__main__":
    filter_code(int(sys.argv[1]))
