def zadanie1(): Unit = {
  println("\nZadanie 1:")
  type Point2D = (Float, Float)

  def distance(p1: Point2D, p2: Point2D): Float = {
    val distance_x = p1._1 - p2._1
    val distance_y = p1._2 - p2._2
    Math.sqrt(distance_x * distance_x + distance_y * distance_y).toFloat
  }

  val point1: Point2D = (0, 0)
  val point2: Point2D = (3, 3)

  println("Distance: " + distance(point1, point2))

  type Point3D = (Float, Float, Float)
  def distance3D(p1: Point3D, p2: Point3D): Float = {
    val distance_x = p1._1 - p2._1
    val distance_y = p1._2 - p2._2
    val distance_z = p1._3 - p2._3
    Math.sqrt(distance_x * distance_x + distance_y * distance_y + distance_z * distance_z).toFloat
  }

  type PointND = List[Float]
  def distanceND(p1: PointND, p2: PointND): Float = {
    val squaredDistances = p1.zip(p2).map {case (x1, x2) => Math.pow(x1-x2, 2)}
    Math.sqrt(squaredDistances.sum).toFloat
  }
}



def zadanie2Tuple(): Unit = {
  println("\nZadanie 2 Tuple:")
  type Person = (String, String, Int, String, Int)
  type Partnership = (Person, Person)

  def getYoungerPerson(partnership: Partnership): Person = {
    if (partnership._1._3 < partnership._2._3) partnership._1
    else partnership._2
  }

  val person1: Person = ("Jan", "Kowalski", 22, "male", 43)
  val person2: Person = ("Anna", "Nowak", 19, "female", 36)
  val partnership: Partnership = (person1, person2)

  println("The younger person is: " + getYoungerPerson(partnership))
  //person1._1 = "Kamil"
  //partnership._2 = person2
}

def zadanie2CC(): Unit = {
  println("\nZadanie 2 Case Class:")
  case class Person(name: String, surname: String, age: Int, gender: String, shoeSize: Int)
  case class Partnership(person1: Person, person2: Person)

  def getYoungerPerson(partnership: Partnership): Person = {
    if (partnership.person1.age < partnership.person2.age) partnership.person1
    else partnership.person2
  }

  val person1CC = Person("Jan", "Kowalski", 22, "male", 43)
  val person2CC = Person("Anna", "Nowak", 19, "female", 36)
  val partnership = Partnership(person1CC, person2CC)

  println("The younger person is: " + getYoungerPerson(partnership))
  //person1CC.name = "Kamil"
  //partnership.person1 = person2CC
}



def zadanie3(): Unit = {
  println("\nZadanie 3:")
  enum WeekDay:
    case Poniedzialek, Wtorek, Sroda, Czwartek, Piatek, Sobota, Niedziela

  def weekDayToString(day: WeekDay): String = {
    day match {
      case WeekDay.Poniedzialek => "Poniedzialek"
      case WeekDay.Wtorek => "Wtorek"
      case WeekDay.Sroda => "Sroda"
      case WeekDay.Czwartek => "Czwartek"
      case WeekDay.Piatek => "Piatek"
      case WeekDay.Sobota => "Sobota"
      case WeekDay.Niedziela => "Niedziela"
    }
  }

  def nextDay(day: WeekDay): WeekDay = {
    day match {
      case WeekDay.Poniedzialek => WeekDay.Wtorek
      case WeekDay.Wtorek => WeekDay.Sroda
      case WeekDay.Sroda => WeekDay.Czwartek
      case WeekDay.Czwartek => WeekDay.Piatek
      case WeekDay.Piatek => WeekDay.Sobota
      case WeekDay.Sobota => WeekDay.Niedziela
      case WeekDay.Niedziela => WeekDay.Poniedzialek
    }
  }

  val wednesday: WeekDay = WeekDay.Sroda
  println(weekDayToString(wednesday))
  println(nextDay(wednesday))
}



def zadanie4(): Unit = {
  println("\nZadanie 4:")
  enum Maybe[+A]:
    case Just(value: A)
    case Nothing

  def safeHead[A](list: List[A]): Maybe[A] = {
    list match {
      case Nil => Maybe.Nothing
      case head :: _ => Maybe.Just(head)
    }
  }

  println(safeHead(List()))
  println(safeHead(List(1, 2, 3)))
}



def zadanie5(): Unit = {
  println("\nZadanie 5:")
  case class Cuboid(length: Double, width: Double, height: Double)
  case class Cone(radius: Double, height: Double)
  case class Sphere(radius: Double)
  case class Cylinder(radius: Double, height: Double)

  enum SolidFigure:
    case CuboidFigure(cuboid: Cuboid)
    case ConeFigure(cone: Cone)
    case SphereFigure(sphere: Sphere)
    case CylinderFigure(cylinder: Cylinder)

  def volume(solidFigure: SolidFigure): Double = {
    solidFigure match {
      case SolidFigure.CuboidFigure(cuboid) => cuboid.length * cuboid.width * cuboid.height
      case SolidFigure.ConeFigure(cone) => Math.PI / 3.0 * cone.radius * cone.radius * cone.height
      case SolidFigure.SphereFigure(sphere) => 4.0 / 3.0 * Math.PI * sphere.radius * sphere.radius * sphere.radius
      case SolidFigure.CylinderFigure(cylinder) => Math.PI * cylinder.radius * cylinder.radius
    }
  }

  val cuboid: Cuboid = Cuboid(3.0, 4.0, 5.0)
  val cone: Cone = Cone(6.0, 7.0)
  val sphere: Sphere = Sphere(8.0)
  val cylinder: Cylinder = Cylinder(9.0, 10.0)

  val cuboidFigure: SolidFigure = SolidFigure.CuboidFigure(cuboid)
  val coneFigure: SolidFigure = SolidFigure.ConeFigure(cone)
  val sphereFigure: SolidFigure = SolidFigure.SphereFigure(sphere)
  val cylinderFigure: SolidFigure = SolidFigure.CylinderFigure(cylinder)

  println("Cuboid volume = " + volume(cuboidFigure))
  println("Cone volume = " + volume(coneFigure))
  println("Sphere volume = " + volume(sphereFigure))
  println("Cylinder volume = " + volume(cylinderFigure))
}



@main
def main(): Unit = {
  zadanie1()
  zadanie2Tuple()
  zadanie2CC()
  zadanie3()
  zadanie4()
  zadanie5()
}