a = h = 0

while a <= 0:
    a = float(input("Podaj dlugosc podstawy trojkata: "))
    if a <= 0:
        print("Dlugosc podstawy musi byc dodatnia!")

while h <= 0:
    h = float(input("Podaj dlugosc wysokosci trojkata: "))
    if h <= 0:
        print("Dlugosc wysokosci musi byc dodatnia!")

print("Pole powierzchni trojkata o podanych parametrach wynosi {0} [j2].".format(a * h / 2))