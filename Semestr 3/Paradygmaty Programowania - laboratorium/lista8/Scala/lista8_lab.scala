enum Result[X]:
  case Success(value: X)
  case Failure(errorMessage: String)

  infix def >>=[Y](f: Result[X] => Result[Y]): Result[Y] = bind(this, transformHelper(f))



def bind[X, Y](result: Result[X], f: X => Result[Y]): Result[Y] = {
  result match {
    case Result.Success(value) => f(value)
    case Result.Failure(errorMessage) => Result.Failure(errorMessage)
  }
}

def transformHelper[X, Y](originalFunction: Result[X] => Result[Y]): X => Result[Y] =
  (input: X) => originalFunction(Result.Success(input))



def getUserInput(): Result[String] = {
  print("\nEnter number: ")
  val input: String = scala.io.StdIn.readLine()
  input match {
    case "" => Result.Failure("No value provided!")
    case _ => Result.Success(input)
  }
}

def convertStringToDouble(textResult: Result[String]): Result[Double] = {
  textResult match {
    case Result.Success(text) => {
      if (text.toDoubleOption.isDefined) {
        return Result.Success(text.toDouble)
      }
      else {
        return Result.Failure("Incorrect number format!")
      }
    }
    case Result.Failure(errorMessage) => return Result.Failure(errorMessage)
  }
}

def calculateSqrt(numberResult: Result[Double]): Result[Double] = {
  numberResult match {
    case Result.Success(number) => {
      if (number < 0) {
        return Result.Failure("Cannot calculate root of negative number!")
      }
      else {
        return Result.Success(Math.sqrt(number))
      }
    }
    case Result.Failure(errorMessage) => return Result.Failure(errorMessage)
  }
}

@main
def main():Unit = {
  val calcRoot = calculateSqrt(convertStringToDouble(getUserInput()))
  calcRoot match {
    case Result.Failure(errorMessage) => println(errorMessage)
    case Result.Success(value) => println(value)
  }

  val calcRootInfix = getUserInput() >>= convertStringToDouble >>= calculateSqrt
  calcRootInfix match {
    case Result.Failure(errorMessage) => println(errorMessage)
    case Result.Success(value) => println(value)
  }
}
