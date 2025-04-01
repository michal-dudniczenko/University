import scala.collection.mutable
import scala.collection.mutable.HashMap

def zadanie1(): Unit = {
  def stirling(n: Int, m: Int): Int = {
    //println(n + " " + m)
    if (m == 1 || n == m) {
      return 1
    }
    else if (m == 0 || m > n) {
      return 0
    }
    else {
      return stirling(n - 1, m - 1) + m * stirling(n - 1, m)
    }
  }

  def memoizedStirling(n: Int, m: Int): Int = {
    val memoized: mutable.HashMap[(Int, Int), Int] = mutable.HashMap.empty[(Int, Int), Int]

    def memoizedStirlingHelper(n: Int, m: Int): Int = {
      //println(n + " " + m)
      if (m == 1 || n == m) {
        return 1
      }
      else if (m == 0 || m > n) {
        return 0
      }
      else if (memoized.contains((n, m))) {
        return memoized((n, m))
      }
      else {
        var partialResult1 = 0
        var partialResult2 = 0
        if (memoized.contains((n - 1, m - 1))) {
          partialResult1 = memoized((n - 1, m - 1))
        }
        else {
          partialResult1 = memoizedStirlingHelper(n - 1, m - 1)
        }
        if (memoized.contains((n - 1, m))) {
          partialResult2 = memoized((n - 1, m))
        }
        else {
          partialResult2 = memoizedStirlingHelper(n - 1, m)
        }
        val result: Int = partialResult1 + m * partialResult2
        memoized += ((n, m) -> result)
        return result
      }
    }

    memoizedStirlingHelper(n, m)
  }

  println("\nZadanie 1:")

  println(memoizedStirling(10, 5))
  println(stirling(10, 5))
}



def zadanie2(): Unit = {
  def makeMemoize[A, B](fun: A => B): A => B = {
    val memoized = mutable.HashMap.empty[A, B]

    (arg: A) => memoized.getOrElseUpdate(arg, fun(arg))
  }

  def complexFunction(x: Int): Int = {
    println(s"Calculating for $x")
    x * x
  }

  println("\nZadanie 2:")

  val memoizedComplex = makeMemoize(complexFunction)

  println(memoizedComplex(5))
  println(memoizedComplex(5))
  println(memoizedComplex(5))
}



def zadanie3(): Unit = {
  def complexFunction(x: Int): Int = {
    println(s"Calculating for $x")
    x * x
  }

  println("\nZadanie 3:")

  lazy val lazyComplex = complexFunction(5)

  println("before usage")

  println(lazyComplex)

  println("after usage")
}


@main
def main(): Unit = {
  zadanie1()
  zadanie2()
  zadanie3()
}



