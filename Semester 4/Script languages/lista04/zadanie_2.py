import sys
import os



def display_path_folders():

    print("\nWszystkie foldery w path:\n")

    for folder in dict(os.environ).get("PATH").split(os.pathsep):
        if len(folder) != 0:
            print(folder)



def display_path_folders_with_exe():

    print("\nWszystkie foldery w path wraz z plikami .exe:\n")

    for folder in dict(os.environ).get("PATH").split(os.pathsep):
        if len(folder) != 0:
            print(folder + ":")
            try:
                for file in os.listdir(folder):
                    if file.endswith(".exe"):
                        print("\t" + file)
            except:
                print("\tNie udalo sie wylistowac folderu.")



def main():
    if len(sys.argv) != 2 or sys.argv[1] not in ["1", "2"]:
        print("W celu uruchomienia skryptu podaj argument:\n1 - wyswietli wszystkie foldery w path\n2 - wyswietl foldery wraz z plikami exe")
        sys.exit()
    
    if sys.argv[1] == "1":
        display_path_folders()
    else:
        display_path_folders_with_exe()



if __name__ == "__main__":
    main()