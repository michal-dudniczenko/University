#graphics_ratio.py

import sys

def graphics_ratio():
    extensions = (".gif", ".jpg", ".jpeg", ".xbm")
    
    graphics_count = 0
    others_count = 0
    for line in sys.stdin:
        tokenized = line.split()
        try:
            if tokenized[-4].endswith(extensions):
                graphics_count += 1
            else:
                others_count += 1
        except:
            pass
    
    print("Stosunek pobran grafiki do pozostalych zasobow wynosi: " +
    f"{graphics_count}:{others_count} = {round((graphics_count / others_count),2)}")

if __name__ == "__main__":
    graphics_ratio()
