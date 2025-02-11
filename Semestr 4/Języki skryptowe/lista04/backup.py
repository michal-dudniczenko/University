import sys
import os
import subprocess
from datetime import datetime



def main():
    if len(sys.argv) != 2:
        print("Usage: python backup.py <path_to_directory>")
        sys.exit(1)
    

    os.environ["BACKUPS_DIR"] = f"{os.getcwd()}\\.backups"


    folder_path = sys.argv[1]

    timestamp = datetime.now().strftime("%Y_%m_%d_%H_%M_%S")

    os.chdir(folder_path)

    full_folder_path = os.getcwd()
    dir_name = os.path.basename(full_folder_path)

    backup_name = f"{timestamp}-{dir_name}.zip"

    subprocess.run(["7z", "a", backup_name, "*"], shell=True)

    if not (os.path.exists(os.environ["BACKUPS_DIR"]) and os.path.isdir(os.environ["BACKUPS_DIR"])):
        subprocess.run(["mkdir", os.environ["BACKUPS_DIR"]], shell=True)

    subprocess.run(["move", f"{full_folder_path}\\{backup_name}", os.environ["BACKUPS_DIR"]], shell=True)

    history_path = f"{os.environ["BACKUPS_DIR"]}\\backups_history.csv"

    with open(history_path, "a") as f:
        f.write(timestamp + ";" + full_folder_path + ";" + backup_name + "\n")



if __name__ == "__main__":
    main()
       