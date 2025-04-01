import scala.annotation.tailrec

@tailrec
def lastElement[A](list: List[A]): Option[A] = {
  list match {
    case Nil => None
    case x :: Nil => Some(x)
    case _ :: tail => lastElement(tail)
  }
}

@tailrec
def twoLastElements[A](list: List[A]): Option[(A, A)] = {
  list match {
    case Nil => None
    case _ :: Nil => None
    case a :: b :: Nil => Some((a, b))
    case _ :: tail => twoLastElements(tail)
  }
}

def listLength[A](list: List[A]): Int = {
  list match {
    case Nil => 0
    case _ :: tail => 1 + listLength(tail)
  }
}

def reverseList[A](list: List[A]): List[A] = {
  list match {
    case Nil => Nil
    case head :: tail => reverseList(tail) :+ head
  }
}

def isPalindrome(s : String): Boolean = {
  @tailrec
  def isPalindromeHelper(s: String): Boolean = {
    if (s.length <= 1) {
      true
    } else if (s.head != s.last) {
      false
    } else {
      isPalindromeHelper(s.substring(1, s.length - 1))
    }
  }
  
  val cleanString = s.replaceAll("\\s", "").toLowerCase()
  isPalindromeHelper(cleanString)
}

def removeDuplicates[A](list: List[A]): List[A] = {
  def removeAllOccurences(list: List[A], element: A): List[A] = {
    list match {
      case Nil => Nil
      case head :: tail if head == element =>
        removeAllOccurences(tail, element)
      case head :: tail =>
        head :: removeAllOccurences(tail, element)
    }
  }

  list match {
    case Nil => Nil
    case head :: tail =>
      val filteredTail = removeAllOccurences(tail, head)
      head :: removeDuplicates(filteredTail)
  }
}

def onlyEvenIndexes[A](list: List[A]): List[A] = {
  def onlyEvenIndexesHelper(list: List[A], index: Int): List[A] = {
    list match {
      case Nil => Nil
      case head :: tail if index % 2 == 0 =>
        head :: onlyEvenIndexesHelper(tail, index + 1)
      case head :: tail =>
        onlyEvenIndexesHelper(tail, index + 1)
    }
  }

  onlyEvenIndexesHelper(list, 0)
}

def isPrime(n: Int): Boolean = {
  @tailrec
  def isPrimeHelper(n: Int, divisor: Int): Boolean = {
    if (divisor * divisor > n) {
      true
    } else if (n % divisor == 0) {
      false
    } else {
      isPrimeHelper(n, divisor + 2)
    }
  }

  if (n <= 1){
    false
  } else if (n <= 3){
    true
  } else if (n % 2 == 0){
    false
  } else{
    isPrimeHelper(n, 3)
  }
}


@main
def main(): Unit = {
  val list1 = List(4, 1, 4, 2, 3, 4, 5, 4, 3, 2, 2, 4)
  val list2 = List()
  val list3 = List(5)
  val list4 = List(7, 7)

  println("\nLast element:")
  println(lastElement(list1))
  println(lastElement(list2))
  println(lastElement(list3))
  println(lastElement(list4))

  println("\nTwo last elements:")
  println(twoLastElements(list1))
  println(twoLastElements(list2))
  println(twoLastElements(list3))
  println(twoLastElements(list4))

  println("\nList length:")
  println(listLength(list1))
  println(listLength(list2))
  println(listLength(list3))
  println(listLength(list4))

  println("\nReverse list:")
  println(reverseList(list1))
  println(reverseList(list2))
  println(reverseList(list3))
  println(reverseList(list4))

  println("\nIs palindrome:")
  println(isPalindrome("ka mil slima   k"))
  println(isPalindrome("test"))

  println("\nRemove duplicates:")
  println(removeDuplicates(list1))
  println(removeDuplicates(list2))
  println(removeDuplicates(list3))
  println(removeDuplicates(list4))

  println("\nOnly even indexes:")
  println(onlyEvenIndexes(list1))
  println(onlyEvenIndexes(list2))
  println(onlyEvenIndexes(list3))
  println(onlyEvenIndexes(list4))

  println("\nIs prime:")
  println(isPrime(47))
  println(isPrime(5))
  println(isPrime(4))
  println(isPrime(25))
  println(isPrime(15))
}