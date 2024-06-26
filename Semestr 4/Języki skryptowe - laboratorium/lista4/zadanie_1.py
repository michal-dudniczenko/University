import sys
import os



def display_all_env_var():
    print(sorted(os.environ.items()))



def display_filter_env_var(env_vars):

    if len(env_vars) == 0:
        print("Nie podano zadnych zmiennych srodowiskowych do wypisania.")
        return

    for i in range(len(env_vars)):
        env_vars[i] = env_vars[i].upper()

    result = {}

    for key,val in os.environ.items():
        for arg in env_vars:
            if arg in key:
                result[key] = val
                break
    
    print(sorted(result.items()))

    

def main():
    args = sys.argv[1:]

    print("\n-----------------------------------------------------------\n")

    print("\nWszystkie zmienne srodowiskowe:\n")

    display_all_env_var()

    print("\nPodane zmienne srodowiskowe:\n")

    display_filter_env_var(args)

    print("\n-----------------------------------------------------------\n")



if __name__ == "__main__":
    main()