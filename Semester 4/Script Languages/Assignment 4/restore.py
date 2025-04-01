import sys
import os
import subprocess



def main():
    os.environ["BACKUPS_DIR"] = f"{os.getcwd()}\\.backups"

    folder_path = ""
    if len(sys.argv) == 1:
        folder_path = os.getcwd()
    elif len(sys.argv) == 2:
        folder_path = sys.argv[1]
        if not (os.path.exists(folder_path) and os.path.isdir(folder_path)):
            subprocess.run(["mkdir", folder_path], shell=True)

    else:
        print("Usage: python restore.py <path_to_directory : optional>")
        sys.exit(1)
    
    if not os.path.exists(os.environ["BACKUPS_DIR"]+"\\backups_history.csv"):
        raise(Exception("Brak zapisanych kopii zapasowych!"))

    with open(os.environ["BACKUPS_DIR"]+"\\backups_history.csv") as f:
        content = f.readlines()
        content.reverse()

        backup_number = 1
        for line in content:

            line_tokens = line.rstrip().split(";")

            print(f"{backup_number}.\t {"\t\t".join(line_tokens)}")

            backup_number += 1

    if len(content) == 0:
        raise(Exception("Brak zapisanych kopii zapasowych!"))

    choice = int(input()) - 1

    if choice >= len(content):
        raise(Exception("Niepoprawny wybor!"))

    chosen_backup_name = content[choice].split(";")[-1]

    os.chdir(folder_path)

    subprocess.run(["7z", "x", f"{os.environ["BACKUPS_DIR"]}\\{chosen_backup_name}"], shell=True)

    subprocess.run(["del", f"{os.environ["BACKUPS_DIR"]}\\*", "/Q"], shell=True )



if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(e)