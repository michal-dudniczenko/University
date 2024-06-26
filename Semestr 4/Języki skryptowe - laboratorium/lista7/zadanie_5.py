import functools



def fibonacci(n):
    print(f"wywołanie fibonacci({n})")
    if n <= 1:
        return n
    else:
        return fibonacci(n-1) + fibonacci(n-2)
    


def make_generator_mem(func):
    cache = {}
    
    def memoize(func):
        @functools.wraps(func)
        def wrapper(*args, **kwargs):
            key = str(args) + str(kwargs)
            if key not in cache:
                cache[key] = func(*args, **kwargs)
            return cache[key]
        return wrapper
    
    memoized_func = memoize(func)
    
    def generator():
        for key,value in globals().items():
            if value == func:
                globals()[key] = memoized_func

        n = 1
        while True:
            yield memoized_func(n)
            n += 1

    return generator



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 5:\n")

    generator = make_generator_mem(fibonacci)()

    for _ in range(10):
        print(next(generator))
