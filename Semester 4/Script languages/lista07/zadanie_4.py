def make_generator(func):
    def generator():
        n = 1
        while True:
            yield func(n)
            n += 1

    return generator



def fibonacci(n):
    if n <= 1:
        return n
    else:
        return fibonacci(n-1) + fibonacci(n-2)



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 4:\n")

    fib_generator = make_generator(fibonacci)()

    for _ in range(10):
        print(next(fib_generator))
    print()
    
    generator1 = make_generator(lambda n: 2 * n + 1)()
    generator2 = make_generator(lambda n: 2 ** n)()

    for _ in range(10):
        print(next(generator1))
    print()

    for _ in range(10):
        print(next(generator2))
    print()