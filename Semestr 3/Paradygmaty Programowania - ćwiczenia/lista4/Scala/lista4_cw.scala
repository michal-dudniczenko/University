import scala.annotation.tailrec

//zadanie 3

enum BinaryTree[+A] {
  case Node(value: A, left: BinaryTree[A], right: BinaryTree[A])
  case Empty
}

def breadthBT[A](tree: BinaryTree[A]): List[A] = {
  @tailrec
  def bfs(queue: List[BinaryTree[A]], acc: List[A]): List[A] = queue match {
    case Nil => acc.reverse
    case BinaryTree.Empty :: rest => bfs(rest, acc)
    case BinaryTree.Node(value, left, right) :: rest =>
      bfs(rest ++ List(left, right), value :: acc)
  }

  bfs(List(tree), Nil)
}

val tt: BinaryTree[Int] =
  BinaryTree.Node(1,
    BinaryTree.Node(2,
      BinaryTree.Node(4,
        BinaryTree.Empty,
        BinaryTree.Empty
      ),
      BinaryTree.Empty
    ),
    BinaryTree.Node(3,
      BinaryTree.Node(5,
        BinaryTree.Empty,
        BinaryTree.Node(6,
          BinaryTree.Empty,
          BinaryTree.Empty
        )
      ),
      BinaryTree.Empty
    )
  )


// Zadanie 5
class Graph[A](val adjacencyList: A => List[A])

def depthSearch[A](graph: Graph[A], start: A): List[A] = {
  @tailrec
  def dfs(visited: List[A], stack: List[A]): List[A] = stack match {
    case Nil => visited.reverse
    case node :: rest =>
      if (visited.contains(node))
        dfs(visited, rest)
      else {
        val neighbors = graph.adjacencyList(node)
        dfs(node :: visited, neighbors ++ rest)
      }
  }

  dfs(Nil, List(start))
}

val g = new Graph[Int](node =>
  node match {
    case 0 => List(4, 3, 1)
    case 1 => List(0)
    case 2 => List(3)
    case 3 => List(0, 2)
    case 4 => List(0)
    case _ => Nil
  }
)

@main
def main(): Unit = {
  println("\nZadanie 3:")
  val zadanie3 = breadthBT(tt)
  println(s"[${zadanie3.mkString("; ")}]")

  println("\nZadanie 5:")
  val zadanie5 = depthSearch(g, 4)
  println(s"[${zadanie5.mkString("; ")}]")
}