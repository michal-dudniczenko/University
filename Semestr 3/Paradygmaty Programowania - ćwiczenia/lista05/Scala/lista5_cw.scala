def lrepeat[A](k: Int)(lxs: LazyList[A]): LazyList[A] =
  lxs.flatMap(x => LazyList.fill(k)(x))

def lfib(): LazyList[Int] = {
  def fib(a: Int, b: Int): LazyList[Int] = a #:: fib(b, a + b)

  fib(0, 1)
}

@main
def main(): Unit = {

  val result1 = lrepeat(3)(LazyList('a', 'b', 'c', 'd')).toList
  println(result1)

  val result2 = lrepeat(3)(LazyList.from(1)).take(15).toList
  println(result2)

  val result3 = lrepeat(3)(LazyList()).take(15).toList
  println(result3)

  val fibonacciSequence = lfib().take(10).toList
  println(fibonacciSequence)
}