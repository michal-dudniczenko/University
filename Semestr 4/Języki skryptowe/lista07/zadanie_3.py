import random
import string



class PasswordGenerator:
    def __init__(self, length, count, charset=None):
        self.length = length
        self.charset = charset if charset else list(string.ascii_letters) + list(string.digits)
        self.count = count
        self.generated_count = 0

    def __iter__(self):
        return self

    def __next__(self):
        if self.generated_count >= self.count:
            raise StopIteration

        password = ''.join(random.choice(self.charset) for _ in range(self.length))
        self.generated_count += 1
        return password



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 3:\n")

    passwd_generator = PasswordGenerator(10, 10)
    passwd_generator2 = PasswordGenerator(10, 10)
    try:
        for _ in range(100):
            print(passwd_generator.__next__())
    except StopIteration:
        print("StopIteration exception")
    
    print()

    for passwd in passwd_generator2:
        print(passwd)