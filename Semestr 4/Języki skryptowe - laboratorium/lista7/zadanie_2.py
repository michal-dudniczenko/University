def forall(pred, iterable):
    return sum(1 for item in iterable if pred(item)) == len(iterable)



def exists(pred, iterable):
    return sum(1 for item in iterable if pred(item)) >= 1



def atleast(n, pred, iterable):
    return sum(1 for item in iterable if pred(item)) >= n



def atmost(n, pred, iterable):
    return sum(1 for item in iterable if pred(item)) <= n



if __name__ == "__main__":
    print("\n-----------------------------------------------------------------------\n")
    print("zadanie 2:\n")
    
    
    pred_even = lambda n : n % 2 == 0

    iterable1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    iterable2 = [2, 4, 6, 8, 10]
    iterable3 = [1, 3, 5, 7, 9]

    print(f"forall for {iterable1}: {forall(pred_even, iterable1)}")
    print(f"forall for {iterable2}: {forall(pred_even, iterable2)}")
    print(f"forall for {iterable3}: {forall(pred_even, iterable3)}")

    print(f"exists for {iterable1}: {exists(pred_even, iterable1)}")
    print(f"exists for {iterable2}: {exists(pred_even, iterable2)}")
    print(f"exists for {iterable3}: {exists(pred_even, iterable3)}")

    print(f"atleast 5 for {iterable1}: {atleast(5, pred_even, iterable1)}")
    print(f"atleast 10 for {iterable2}: {atleast(10, pred_even, iterable2)}")
    print(f"atleast 1 for {iterable3}: {atleast(1, pred_even, iterable3)}")

    print(f"atmost 5 for {iterable1}: {atmost(5, pred_even, iterable1)}")
    print(f"atmost 3 for {iterable2}: {atmost(3, pred_even, iterable2)}")
    print(f"atmost 1 for {iterable3}: {atmost(1, pred_even, iterable3)}")