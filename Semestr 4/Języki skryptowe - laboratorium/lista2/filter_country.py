#filter_country.py

import sys

def filter_country(country_code):
    for line in sys.stdin:
        tokenized = line.split()
        try:
            if tokenized[0].endswith("." + country_code):
                sys.stdout.write(line)
        except:
            pass

if __name__ == "__main__":
    filter_country(sys.argv[1])
