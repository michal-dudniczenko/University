using System;

namespace lista06 {
    class Program {
        static void Main() {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("Zadanie 1:");
            var tuple = ("Jan", "Kowalski", 25, 5321.00);

            DisplayPerson(tuple);

            Console.WriteLine("Sposób pierwszy:");
            Console.WriteLine(tuple.Item1 + " " + tuple.Item2 + " " + tuple.Item3 + " " + tuple.Item4);
        
            Console.WriteLine("Sposób drugi:");
            var (firstName, lastName, age, salary) = tuple;
            Console.WriteLine(firstName + " " + lastName + " " + age + " " + salary);

            Console.WriteLine("Sposób trzeci:");
            var namedTuple = (firstName: "Jan", lastName: "Kowalski", age: 25, salary: 5321.00);
            Console.WriteLine(namedTuple.firstName + " " + namedTuple.lastName + " " + namedTuple.age + " " + namedTuple.salary);



            Console.WriteLine("\nZadanie 2:");
            string @class = "ala ma kota";
            Console.WriteLine("zmienna class = " + @class);

            Console.WriteLine("\nZadanie 3:");
            int[] array = {10, 1, 38, 39, 56, 120, 7, 2, 1, 10};
            Console.WriteLine("Początkowa tablica: [" + string.Join(", ", array) + "]");
            Array.Sort(array);
            Console.WriteLine("Posortowana: [" + string.Join(", ", array) + "]");
            Array.Reverse(array);
            Console.WriteLine("Reversed: [" + string.Join(", ", array) + "]");
            Console.WriteLine("Indeks elementu = 7: " + Array.IndexOf(array, 7));
            Console.WriteLine("TrueForAll parzyste: " + Array.TrueForAll(array, value => value % 2 == 0));
            Console.WriteLine("TrueForAll dodatnie: " + Array.TrueForAll(array, value => value > 0));
            int[] newArray = new int[array.Length];
            Console.WriteLine("Nowa tablica przed kopiowaniem: ["  + string.Join(", ", newArray) + "]");
            Array.Copy(array, newArray, array.Length);
            Console.WriteLine("Nowa tablica po kopiowaniu: ["  + string.Join(", ", newArray) + "]");
            Array.Fill(newArray, 42);
            Console.WriteLine("Nowa tablica po wypełnieniu 42: [" + string.Join(", ", newArray) + "]");

            Console.WriteLine("\nZadanie 4:");
            Console.WriteLine("Żeby przekazać do funkcji to trzeba użyć dynamic albo funkcja generyczna i castować na dynamic");
            var person = new {firstName = "Jan", lastName = "Kowalski", age = 25, salary = 5321.00};
            DisplayPerson2(person);
            Console.WriteLine($"Dane osoby: {person.firstName} {person.lastName} {person.age} {person.salary}");

            Console.WriteLine("\nZadanie 5:");
            DrawCard("Ryszard");
            Console.WriteLine();
            DrawCard("Jan", minWidth: 50);

            Console.WriteLine("\nZadanie 6:");
            var result = CountMyTypes(2, 3, 1, 4, 4.5, "hello", "C#", 10, -2.3, 3.14, "world", true, 8);
            Console.WriteLine($"Even ints: {result.evenInts}");
            Console.WriteLine($"Posivite doubles: {result.positiveDoubles}");
            Console.WriteLine($"Strings of length >= 5: {result.stringsOfLengthAtLeastFive}");
            Console.WriteLine($"Others: {result.others}");
        }

        static void DisplayPerson((String firstName, String lastName, int age, double salary) person) {
            Console.WriteLine($"Dane osoby:\n\tImię: {person.firstName}\n\t" +
            $"Nazwisko: {person.lastName}\n\tWiek: {person.age}\n\tPłaca: {person.salary} zł");
        }

        static void DisplayPerson2(dynamic person) {
            Console.WriteLine($"Dane osoby:\n\tImię: {person.firstName}\n\t" +
            $"Nazwisko: {person.lastName}\n\tWiek: {person.age}\n\tPłaca: {person.salary} zł");
        }

        static void DrawCard(string firstLine, string secondLine = "Rys", string frameChar = "X", int frameWidth = 2, int minWidth = 20) {
            int width = Math.Max(firstLine.Length + 1 + frameWidth, Math.Max(secondLine.Length + 1 + frameWidth, minWidth));
            // jeżeli linie mają długości jedna parzysta a druga nieparzysta to centruję pierwszą linię
            if (firstLine.Length % 2 == 1) {
                if (width % 2 == 0) {
                    width++;
                }
            }
            int height = 4 + frameWidth * 2;
            for (int i = 0; i < height; i++) {
                if (i < frameWidth || i >= height - frameWidth) {
                    Console.WriteLine(string.Concat(Enumerable.Repeat(frameChar, width)));
                } else {
                    Console.Write(string.Concat(Enumerable.Repeat(frameChar, frameWidth)));

                    if (i == frameWidth + 1) {
                        Console.Write(string.Concat(Enumerable.Repeat(" ", (width - 2 * frameWidth - firstLine.Length) / 2)));
                        Console.Write(firstLine);
                        Console.Write(string.Concat(Enumerable.Repeat(" ", (width - 2 * frameWidth - firstLine.Length) / 2)));                        
                    } else if (i == frameWidth + 2) {
                        Console.Write(string.Concat(Enumerable.Repeat(" ", (width - 2 * frameWidth - secondLine.Length) / 2)));
                        // jeżeli linie mają długości jedna parzysta a druga nieparzysta to do drugiej muszę dodać spację
                        Console.Write(secondLine + (secondLine.Length % 2 != firstLine.Length % 2 ? " " : ""));
                        Console.Write(string.Concat(Enumerable.Repeat(" ", (width - 2 * frameWidth - secondLine.Length) / 2)));                         
                    } else {
                        Console.Write(string.Concat(Enumerable.Repeat(" ", width - 2 * frameWidth)));
                    }

                    Console.Write(string.Concat(Enumerable.Repeat(frameChar, frameWidth)));  
                    Console.Write("\n");                  
                }           
            }
        }

        static (int evenInts, int positiveDoubles, int stringsOfLengthAtLeastFive, int others) CountMyTypes(params object[] items) {
            int evenInts = 0;
            int positiveDoubles = 0;
            int stringsOfLengthAtLeastFive = 0;
            int others = 0;

            foreach (var item in items) {
                switch (item) {
                    case int i when i % 2 == 0:
                        evenInts++;
                        break;
                    case double d when d > 0:
                        positiveDoubles++;
                        break;
                    case string s when s.Length >= 5:
                        stringsOfLengthAtLeastFive++;
                        break;
                    default:
                        others++;
                        break;
                }
            }

            return (evenInts, positiveDoubles, stringsOfLengthAtLeastFive, others);
        }
    }
}