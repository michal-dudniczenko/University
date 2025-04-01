namespace ConsoleApp {
    internal class Program {
        static void Main() {
            string? a, b, c;
            while (true) {
                Console.WriteLine("-----------------------------------");
                Console.Write("Podaj wartość współczynnika a: ");
                a = Console.ReadLine();
                Console.Write("Podaj wartość współczynnika b: ");
                b = Console.ReadLine();
                Console.Write("Podaj wartość współczynnika c: ");
                c = Console.ReadLine();

                double a2, b2, c2;

                if (double.TryParse(a, out a2) && double.TryParse(b, out b2) && double.TryParse(c, out c2)) {
                    double[]? result = CalculateRoot(a2, b2, c2);

                    if (result == null) {
                        Console.WriteLine("\nNieskończenie wiele rozwiązań");
                    } else if (result.Length == 0) {
                        Console.WriteLine("\nBrak rozwiązań");
                    } else if (result.Length == 1) {
                        Console.WriteLine(string.Format("\nRozwiązanie podwójne równania x0 = {0}", result[0]));
                    } else {
                        Console.WriteLine($"\nRozwiązania równania kwadratowego:\nx1 = {result[0]}\nx2 = {result[1]} ");
                    }
                } else {
                    Console.WriteLine("\nWprowadzono nieprawidłowe dane.");
                }
            }
        }

        private static double[]? CalculateRoot(double a, double b, double c) {
            if (a == 0) {
                if (b == 0) {
                    if (c == 0) {
                        return null;
                    } else {
                        return [];
                    }
                } else {
                    return [(-1 * c) / b];
                }
            } else {
                double delta = b * b - 4 * a * c;
                if (delta < 0) {
                    return [];
                } else if (delta == 0) {
                    return [(-1 * b) / (2 * a)];
                } else {
                    return [(-1 * b) / (2 * a) - Math.Sqrt(delta), (-1 * b) / (2 * a) - Math.Sqrt(delta)];
                }
            }
        }
    }
}
