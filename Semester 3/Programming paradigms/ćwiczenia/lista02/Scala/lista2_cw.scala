import scala.annotation.tailrec

//zadanie1
def evenR(n: Int): Boolean = {
  def oddR(n: Int): Boolean = {
    println(s"oddR($n)")
    if (n == 0) false
    else evenR(n - 1)
  }

  println(s"evenR($n)")
  if (n == 0) true
  else oddR(n - 1)
}


//zadanie2
def fib(n: Int): Int = {
  if (n == 0) 0
  else if (n == 1) 1
  else fib(n-2) + fib(n-1)
}



def fibTail(n: Int): Int = {
  @tailrec
  def fibTailHelper(n: Int, a: Int, b: Int): Int = {
    if (n == 0) a
    else fibTailHelper(n-1, b, a+b)
  }
  fibTailHelper(n, 0, 1)
}


//zadanie3
def root3function(a: Double): Double = {
  val epsilon = 1e-15
  @tailrec
  def root3Helper(x: Double): Double = {
    if (Math.abs(Math.pow(x, 3) - a) <= epsilon * Math.abs(a)) x
    else {
      val nextX = x + (a / (x*x) - x) / 3.0
      root3Helper(nextX)
    }
  }
  if (a > 1.0) {
    root3Helper(a/3.0)
  }
  else {
    root3Helper(a)
  }
}
class Zadanie3 {
  def root3method(a: Double): Double = {
    val epsilon = 1e-15

    @tailrec
    def root3Helper(x: Double): Double = {
      if (Math.abs(Math.pow(x, 3) - a) <= epsilon * Math.abs(a)) x
      else {
        val nextX = x + (a / (x * x) - x) / 3.0
        root3Helper(nextX)
      }
    }

    if (a > 1.0) {
      root3Helper(a / 3.0)
    }
    else {
      root3Helper(a)
    }
  }
}



//zadanie5
@tailrec
def initSegment[A](xs: List[A], ys: List[A]): Boolean = {
  (xs, ys) match {
    case (Nil, _) => true
    case (x :: xRest, y :: yRest) if x == y => initSegment(xRest, yRest)
    case _ => false
  }
}



//zadanie6
def replaceNth[A](xs: List[A], n: Int, x: A): List[A] = {
  def replaceNthHelper(xs: List[A], n:Int): List[A] = {
    xs match {
      case head :: tail if n == 0 => x :: tail
      case head :: tail => head :: replaceNthHelper(tail, n - 1)
      case Nil => Nil
    }
  }

  if (n < 0 || n >= xs.length) {
    throw new IllegalArgumentException("Index out of range")
  }
  else replaceNthHelper(xs, n)
}



@main
def main(): Unit = {
  println("\nzadanie 1:")
  evenR(3)

  println("\nzadanie 2:")
  //println(fib(42))
  println(fibTail(42))

  println("\nzadanie 3:")
  println(root3function(76))
  println(new Zadanie3().root3method(76))

  println("\nzadanie 4:")
  val (_, _, x, _, _) = (-2, -1, 0, 1, 2)
  //val (_, (x, _)) = ((1, 2), (0, 1))
  println(x)

  println("\nzadanie 5:")
  println(initSegment(List(1, 2, 3), List(1, 2, 3, 4, 5)))

  println("\nzadanie 6:")
  println(replaceNth(List(1, 2, 3, 4, 5, 6), 2, 777))
}