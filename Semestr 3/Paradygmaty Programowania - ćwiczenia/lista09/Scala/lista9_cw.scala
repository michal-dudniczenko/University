class Time1(private var _hour: Int):
  if (_hour < 0) _hour = 0

  def getHour: Int = _hour

  def setHour(newHour: Int): Unit = {
    if (newHour < 0) {
      _hour = 0
    } else {
      _hour = newHour
    }
  }
end Time1



class Time2a(private var _hour: Int, private var _minute: Int):
  if (!(_hour >= 0 && _hour < 24 && _minute >= 0 && _minute < 60)) {
    throw new IllegalArgumentException("Invalid time parameters")
  }

  def getHour: Int = _hour

  def setHour(newHour: Int): Unit = {
    if (newHour >= 0 && newHour < 24) {
      _hour = newHour
    } else {
      throw new IllegalArgumentException("Invalid time parameters")
    }
  }

  def getMinute: Int = _minute

  def setMinute(newMinute: Int): Unit = {
    if (newMinute >= 0 && newMinute < 60) {
      _minute = newMinute
    } else {
      throw new IllegalArgumentException("Invalid time parameters")
    }
  }

  def before(other: Time2a): Boolean = {
    if (_hour < other.getHour) {
      true
    } else if (_hour == other.getHour && _minute < other.getMinute) {
      true
    } else {
      false
    }
  }
end Time2a



class Time2b:
  private var _minFromMidnight = 0

  def this(hour: Int, minute: Int) = {
    this()
    if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60) {
      _minFromMidnight = hour * 60 + minute
    } else {
      throw new IllegalArgumentException("Invalid time parameters")
    }
  }

  def getHour: Int = _minFromMidnight / 60

  def setHour(newHour: Int): Unit = {
    if (newHour >= 0 && newHour < 24) {
      _minFromMidnight %= 60
      _minFromMidnight += newHour * 60
    } else {
      throw new IllegalArgumentException("Invalid time parameters")
    }
  }

  def getMinute: Int = _minFromMidnight % 60

  def setMinute(newMinute: Int): Unit = {
    if (newMinute >= 0 && newMinute < 60) {
      _minFromMidnight -= (_minFromMidnight % 60)
      _minFromMidnight += newMinute
    } else {
      throw new IllegalArgumentException("Invalid time parameters")
    }
  }

  def before(other: Time2b): Boolean = {
    _minFromMidnight < (other.getHour * 60 + other.getMinute)
  }
end Time2b



class Pojazd(
  private val _producent: String,
  private val _model: String,
  private val _rokProdukcji: Int = -1,
  private var _numerRejestracyjny: String = ""
):

  def this(producent: String, model: String, numerRejestracyjny: String) = {
    this(producent, model)
    _numerRejestracyjny = numerRejestracyjny
  }


  /*
    def this(producent: String, model: String, rokProdukcji: Int) = {
      this(producent, model)
      _rokProdukcji = rokProdukcji
    }

  def this(producent: String, model: String, rokProdukcji: Int, numerRejestracyjny: String) = {
    this(producent, model)
    _rokProdukcji = rokProdukcji
    _numerRejestracyjny = numerRejestracyjny
  }

   */

  def getProducent: String = _producent
  def getModel: String = _model
  def getRokProdukcji: Int = _rokProdukcji
  def getNumerRejestracyjny: String = _numerRejestracyjny

  def setNumerRejestracyjny(numerRejestracyjny: String): Unit = {
    _numerRejestracyjny = numerRejestracyjny
  }

  override def toString: String = {
    _producent + " " + _model + " Rocznik: " + _rokProdukcji +" Rejestracja: " + _numerRejestracyjny
  }
end Pojazd



object UzycieWyjatkow {
  def main(args: Array[String]): Unit = {
    try
      metoda1()
    catch
      case e: Exception =>
        println(e.getMessage + "\n")
        e.printStackTrace()
  }

  @throws[Exception]
  def metoda1(): Unit = {
    metoda2()
  }

  @throws[Exception]
  def metoda2(): Unit = {
    metoda3()
  }

  @throws[Exception]
  def metoda3(): Unit = {
    throw new Exception("Wyjatek zgloszony w metoda3")
  }
}



object Test {
  def main(args: Array[String]): Unit = {

    println("\nZadanie 1:\n")

    val time1 = new Time1(5)
    println(time1.getHour)

    val time2 = new Time1(-3)
    println(time2.getHour)

    time2.setHour(8)
    println(time2.getHour)

    time2.setHour(-2)
    println(time2.getHour)

    println("\nZadanie 2a:\n")
    try {
      val time1 = new Time2a(12, 30)
      println(time1.before(new Time2a(15, 45)))

      val time2 = new Time2a(8, 0)
      time2.setHour(10)
      time2.setMinute(15)
      println(time2.before(new Time2a(8, 30)))
    } catch {
      case e: IllegalArgumentException => println(s"Exception: ${e.getMessage}")
    }

    println("\nZadanie 2b:\n")
    try {
      val time1 = new Time2b(12, 30)
      println(time1.before(new Time2b(15, 45)))

      val time2 = new Time2b(8, 0)
      time2.setHour(10)
      time2.setMinute(15)
      println(time2.before(new Time2b(8, 30)))
    } catch {
      case e: IllegalArgumentException => println(s"Exception: ${e.getMessage}")
    }

    println("\nZadanie 3:\n")

    val pojazd1 = new Pojazd("Toyota", "Yaris")
    val pojazd2 = new Pojazd("Toyota", "Yaris", 2010)
    val pojazd3 = new Pojazd("Toyota", "Yaris", "SK986124")
    val pojazd4 = new Pojazd("Toyota", "Yaris", 2010, "SK986124")

    println(pojazd1)
    println(pojazd2)
    println(pojazd3)
    println(pojazd4)
  }
}