import scala.annotation.tailrec

def flatten1[A](listOfLists: List[List[A]]): List[A] = {
  @tailrec
  def flatten1Helper(result: List[A], remaining: List[List[A]]): List[A] = {
    if (remaining.isEmpty) result
    else {
      val (headList, tailLists) = (remaining.head, remaining.tail)
      val flatList = result ++ headList
      flatten1Helper(flatList, tailLists)
    }
  }
  flatten1Helper(Nil, listOfLists)
}



def count[A](x: A, xs: List[A]): Int = {
  if (xs.isEmpty) 0
  else {
    val (head, tail) = (xs.head, xs.tail)
    if (head == x) 1 + count(x, tail)
    else count(x, tail)
  }
}



def replicate[A] (x: A, n: Int): List[A] = {
  if (n == 0) {
    Nil
  } else {
    x :: replicate(x, n-1)
  }
}



def sqrList(xs: List[Int]): List[Int] = {
  if (xs.isEmpty) {
    Nil
  } else {
    val (head, tail) = (xs.head, xs.tail)
    (head * head) :: sqrList(tail)
  }
}



@tailrec
def palindrome[A](xs: List[A]): Boolean = {
  if (xs.size <= 1) {
    true
  } else if (xs.head == xs.last) {
    palindrome(xs.tail.init)
  } else {
    false
  }
}



def palindrome2[A](xs: List[A]): Boolean = {
  xs == xs.reverse
}



def  listLength[A](xs: List[A]): Int = {
  if (xs.isEmpty) {
    0
  } else {
    1 + listLength(xs.tail)
  }
}



@main
def main(): Unit = {
  println("\nflatten1:")
  val listOfLists = List(List(5, 6), List(1, 2, 3), List(7, 6, 5, 4, 3, 2, 1))
  println(flatten1(listOfLists))

  println("\ncount:")
  val list1 = List(1, 2, 3, 4, 3, 3, 3, 2, 3)
  println(count(3, list1))

  println("\nreplicate:")
  println(replicate("test", 5))

  println("\nsqrList:")
  println(sqrList(List(2, 3, 4, 5)))

  println("\npalindrome:")
  println(palindrome(List("a", "l", "a")))
  println(palindrome(List("t", "e", "s", "t")))
  println(palindrome(List("k", "a", "m", "i", "l", "s", "l", "i", "m", "a", "k")))

  println("\nlistLength:")
  println(listLength(List(1, 2, 3, 4, 5)))
}