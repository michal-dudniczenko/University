def liczba(numbers):
    return len([x for x in numbers if x % 2 == 0])



def median(numbers):
    mid_right = len(numbers) // 2
    mid_left = mid_right - (len(numbers) == 2) #jak bedzie parzysta dlugosc to true czyli 1, a jak nieparzysta to 0

    sorted_nums = sorted(numbers)

    return (sorted_nums[mid_right] + sorted_nums[mid_left]) / 2



def pierwiastek(x, epsilon):
    a = 1
    b = x

    while not abs(a**2 - x) < epsilon:
        a = (a + b) / 2
        b = x / a

    return a



def make_alpha_dict(text):
    return {char:[word for word in text.split() if char in word] for char in text if char.isalpha()}



def flatten(lista):
    return [item for sublist in lista for item in (flatten(sublist) if isinstance(sublist, (list, tuple)) else [sublist])]



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 1:\n")

    print(liczba([1, 2, 5]))
    print(median([1,1,19,2,3,4,4,5,1]))
    print(pierwiastek(3, epsilon = 0.1))
    print(make_alpha_dict("on i ona"))
    print(flatten(flatten([1, [2, 3], [[4, 5], 6]])))