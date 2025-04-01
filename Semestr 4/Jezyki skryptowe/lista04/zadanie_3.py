import sys



def tail(lines=10, bytes=None, file=None):
    if bytes != None and bytes <= 0:
        raise Exception("Number of bytes to read must be positive number!")
    elif lines <= 0:
        raise Exception("Number of lines to read must be positive number!")
    
    try:
        if bytes:
            if file:
                with open(file, "rb") as f:
                    f.seek(0, 2)
                    file_size = f.tell()
                    f.seek(-min(bytes, file_size), 2)
                    content = f.read()    
            else:
                input_bytes = sys.stdin.buffer.read()
                content = input_bytes[-bytes:]
            
            print(content)
            

        else:
            if file:
                with open(file) as f:
                    content = f.readlines()
            else:
                content = sys.stdin.readlines()
            
            for i in range(min(lines, len(content)), 0, -1):
                print(content[-i].rstrip())
    except FileNotFoundError:
        print("File not found!")
    except Exception as e:
        print(e)



def main():
    if len(sys.argv) == 1:
        tail()
    elif len(sys.argv) == 2:
        if sys.argv[1].startswith("--lines="):
            tail(lines=int(sys.argv[1].split("=")[1]))
        elif sys.argv[1].startswith("--bytes="):
            tail(bytes=int(sys.argv[1].split("=")[1]))
        else:
            tail(file=sys.argv[1])
    elif len(sys.argv) == 3:
        if sys.argv[1].startswith("--lines="):
            tail(lines=int(sys.argv[1].split("=")[1]), file=sys.argv[2])
        elif sys.argv[1].startswith("--bytes="):
            tail(bytes=int(sys.argv[1].split("=")[1]), file=sys.argv[2])
        else:
            print("Niepoprawna liczba/kolejnosc/format argumentow. python <--lines=n lub --bytes=n> <sciezka do pliku>")
    else:
        print("Niepoprawna liczba/kolejnosc/format argumentow. python <--lines=n lub --bytes=n> <sciezka do pliku>")



if __name__ == "__main__":
    main()