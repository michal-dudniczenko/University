
def log(prefix: String)(datetime: String)(text: String): Unit = {
  val red = "\u001b[31m"
  val green = "\u001b[32m"
  val yellow = "\u001b[33m"
  val reset = "\u001b[0m"

  prefix match {
    case "WARN" => println(s"$yellow[$prefix] $datetime \t $text$reset")
    case "INFO" => println(s"$green[$prefix] $datetime \t $text$reset")
    case "CRITICAL" => println(s"$red[$prefix] $datetime \t $text$reset")
    case _ => println(s"[$prefix] $datetime \t $text")
  }
}



def customMap[A, B](f: A => B)(list: List[A]): List[B] = {
  list match {
    case Nil => Nil
    case head :: tail => f(head) :: customMap(f)(tail)
  }
}



def customFilter[A](pred: A => Boolean)(list: List[A]): List[A] = {
  list match {
    case Nil => Nil
    case head :: tail  =>
      if (pred(head)) {
        head :: customFilter(pred)(tail)
      }
      else {
        customFilter(pred)(tail)
      }
  }
}



def customReduce[A, B](op: (A, B) => B)(acc: B)(list: List[A]): B = {
  list match {
    case Nil => acc
    case head :: tail => customReduce(op)(op(head, acc))(tail)
  }
}



def average(list: List[Int]): Double = {
  customReduce((x: Int, y: Int) => x + y)(0)(list) / list.length.toDouble
}



def acronym(text: String): String = {
  val firstLetters = customMap((s: String) => s(0))(text.split("\\s+").toList)
  customReduce((x: Char, y: String) => y + x.toUpper)("")(firstLetters)
}



def cubeLessThanSum(list: List[Int]): List[Int] = {
  val sum = customReduce((x: Int, y: Int) => x + y)(0)(list)
  val filtered = customFilter((x: Int) => x*x*x <= sum)(list)
  customMap((x: Int) => x * x)(filtered)
}



@main
def main(): Unit = {
  println("\nLog:")
  val warnLog = log("WARN")
  val nightlyWarnLog = warnLog("2022-10-26 01:45")
  val nightlyInfoLog = log("INFO")("2022-10-26 01:45")
  val nightlyCriticalLog = log("CRITICAL")("2022-10-26 01:45")
  nightlyWarnLog("Hello")
  nightlyInfoLog("Hello")
  nightlyCriticalLog("Hello")

  println("\ncustomMap:")
  println(customMap((x:Int) => x * 2)(List(1, 2, 3, 4, 5)))

  println("\ncustomFilter:")
  println(customFilter((x: Int) => x % 2 == 0)(List(1, 2, 3, 4, 5)))

  println("\ncustomReduce:")
  println(customReduce((x: Int, y: Int) => x + y)(0)(List(1, 2, 3, 4, 5)))
  println(customReduce((x: String, y: Int) => y + x.length)(0)(List("apple", "orange", "banana")))
  println(customReduce((x: String, y: String) => y + x)("")(List("first", "second", "third")))

  println("\naverage:")
  println(average(List(7, 10, 13, 4, 5, 98)))

  println("\nacronym:")
  println(acronym("Zakład ubezpieczeń społecznych"))
  println(acronym("test acronym test acronym"))

  println("\ncubeLessThanSum:")
  println(cubeLessThanSum(List(1, 2, 3, 4, 5, 100)))
}