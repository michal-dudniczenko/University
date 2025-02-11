#request_count.py

import sys

def request_count(req_code):
    count = 0
    for line in sys.stdin:
        tokenized = line.split()
        try:
            if int(tokenized[-2]) == req_code:
                count += 1
        except:
            pass

    print(f"Liczba zadan z kodem {req_code} = {count}")

if __name__ == "__main__":
    request_count(int(sys.argv[1]))
