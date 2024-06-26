import logging
import functools
import time
import sys
from datetime import datetime
import inspect

logging.basicConfig(level=logging.DEBUG, handlers=[logging.StreamHandler(sys.stdout)], format="%(message)s")



def log(level=logging.INFO):
    def decorator(func):
        @functools.wraps(func)
        def wrapper(*args, **kwargs):
            start = time.time()
            result = func(*args, **kwargs)
            end = time.time()
            run_time = end-start

            start_date = datetime.fromtimestamp(start).strftime("%H:%M:%S")

            if inspect.isclass(func):
                logging.log(level, f"Utworzenie obiektu klasy: {func.__name__}, o: {start_date}")
            else:
                logging.log(level, f"Funkcja: {func.__name__}, wywołana o: {start_date}, czas trwania: {run_time}, args: {args}, wartość zwracana: {result}")

            return result
        return wrapper
    return decorator



@log(logging.DEBUG)
def print_function(text):
    print(text)



@log(logging.INFO)
class test:
    def __init__(self, data):
        self.data = data
    
    def some_method(self):
        print("\nsome method\n")



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 6:\n")

    temp1 = test(123)
    temp1 = test(321)
    temp1 = test(100)

    temp1.some_method()

    print_function("Hello world!")
    print_function("some text")
    print_function("123")