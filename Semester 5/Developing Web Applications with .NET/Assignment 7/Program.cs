namespace lista07;

using System.Reflection;

class Program {
    static List<StudentWithTopics> students = Generator.GenerateStudentsWithTopicsEasy();
    static void Main() {
        Console.WriteLine("----------------------------------------------------------------------");

        zadanie1();
        zadanie2();
        zadanie3();      
        zadanie4();
    }

    static void zadanie1() {
        Console.WriteLine("\nZadanie 1:\n");
        var grouped = GroupIntoNSizedGroups(students, 3);

        foreach (var group in grouped) {
            Console.WriteLine($"Group {group.Key + 1}:");
            foreach (var student in group) {
                Console.WriteLine($"\t{student}");
            }
        }
    }

    static void zadanie2() {
        Console.WriteLine("\nZadanie 2:\n");

        Console.WriteLine("a)\n");

        foreach (var topic in SortTopicsByFrequency(students)) {
            Console.WriteLine($"\t{topic}");
        }

        Console.WriteLine("\nb)\n");

        var sortedWithGender = SortTopicsByFrequencyWithGender2(students);

        foreach (var genderGroup in sortedWithGender) {
            Console.WriteLine($"{genderGroup.Key}:");
            foreach (var topic in genderGroup.Value) {
                Console.WriteLine($"\t{topic}");
            }
        }
    }

    static void zadanie3() {
        Console.WriteLine("\nZadanie 3:\n");
        var studentsWithTopicsIds1 = ConvertToStudents1(students);
        var studentsWithTopicsIds2 = ConvertToStudents2(students);

        foreach (var student in studentsWithTopicsIds1) {
            Console.WriteLine($"\t{student}");
        }

        Console.WriteLine("\n-----------------------------\n");

        foreach (var student in studentsWithTopicsIds2) {
            Console.WriteLine($"\t{student}");
        }

        var (studentsRel, topicsRel, studentsToTopics) = ConvertToRelational(students);

        Console.WriteLine("\n-----------------------------\n");

        foreach (var student in studentsRel) {
            Console.WriteLine(student);
        }

        Console.WriteLine();

        foreach (var topic in topicsRel) {
            Console.WriteLine(topic);
        }

        Console.WriteLine();

        foreach (var pair in studentsToTopics) {
            Console.WriteLine(pair);
        }
    }

    static void zadanie4() {
        Console.WriteLine("\nZadanie 4:\n");

        Type? dogType = Type.GetType("lista07.Dog");

        if (dogType != null) {
            object? dog = Activator.CreateInstance(dogType);

            MethodInfo? woofMethod = dog?.GetType().GetMethod("woof", new Type[] { typeof(bool), typeof(int) });
            object? result = woofMethod?.Invoke(dog, new object[] {true, 5});
            Console.WriteLine(result);
        } else {
            Console.WriteLine("null");
        }
    }


    static IEnumerable<IGrouping<int, StudentWithTopics>> GroupIntoNSizedGroups(List<StudentWithTopics> students, int groupSize) {
        var sortedStudents = students
            .OrderBy(s => s.Name)
            .ThenBy(s => s.Index);

        var grouped = sortedStudents
            .Select((student, position) => new { Student = student, GroupNumber = position / groupSize })
            .GroupBy(x => x.GroupNumber, x => x.Student);

        return grouped;
    }

    static IEnumerable<string> SortTopicsByFrequency(List<StudentWithTopics> students) {
        return students
            .SelectMany(s => s.Topics)
            .GroupBy(topic => topic)
            .OrderByDescending(group => group.Count()) 
            .ThenBy(topic => topic.Key)
            .Select(group => group.Key);
    }

    static Dictionary<Gender, List<string>> SortTopicsByFrequencyWithGender(List<StudentWithTopics> students) {
        var studentsByGender = students.GroupBy(s => s.Gender);

        var result = new Dictionary<Gender, List<string>>();

        foreach (var group in studentsByGender) {
            var sortedTopics = group
                .SelectMany(s => s.Topics)
                .GroupBy(topic => topic)
                .OrderByDescending(group => group.Count()) 
                .ThenBy(topic => topic.Key)
                .Select(group => group.Key)           
                .ToList();                      

            result[group.Key] = sortedTopics;
        }

        return result;
    }

    static Dictionary<Gender, List<string>> SortTopicsByFrequencyWithGender2(List<StudentWithTopics> students) {
        return students
                .GroupBy(s => s.Gender)
                .ToDictionary(
                    group => group.Key,
                    group => group
                                .SelectMany(s => s.Topics)
                                .GroupBy(topic => topic)
                                .OrderByDescending(group => group.Count()) 
                                .ThenBy(topic => topic.Key)
                                .Select(group => group.Key)           
                                .ToList()
                );               
    }

    static List<Student> ConvertToStudents1(List<StudentWithTopics> students) {
        var topics = new List<Topic> {
            new Topic(1, "C#"),
            new Topic(2, "PHP"),
            new Topic(3, "algorithms"),
            new Topic(4, "C++"),
            new Topic(5, "fuzzy logic"),
            new Topic(6, "Basic"),
            new Topic(7, "Java"),
            new Topic(8, "JavaScript"),
            new Topic(9, "neural networks"),
            new Topic(10, "web programming")
        };

        return students.Select(s => new Student(
            id: s.Id,
            index: s.Index,
            name: s.Name,
            gender: s.Gender,
            active: s.Active,
            departmentId: s.DepartmentId,
            topicIds: s.Topics
                .Select(t => topics.First(topic => topic.Name == t).Id)
                .ToList()
        )).ToList();
    }

    static List<Student> ConvertToStudents2(List<StudentWithTopics> students) {
        var topics = students
            .SelectMany(s => s.Topics)
            .Distinct()
            .Select((name, index) => new Topic(id: index + 1, name: name))
            .ToList();

        return students.Select(s => new Student(
            id: s.Id,
            index: s.Index,
            name: s.Name,
            gender: s.Gender,
            active: s.Active,
            departmentId: s.DepartmentId,
            topicIds: s.Topics
                .Select(t => topics.First(topic => topic.Name == t).Id)
                .ToList()
        )).ToList();
    }

    static (List<StudentRel>, List<Topic>, List<StudentToTopic>) ConvertToRelational(List<StudentWithTopics> studentsWithTopics) {
        var topics = studentsWithTopics
            .SelectMany(s => s.Topics)
            .Distinct()
            .Select((name, index) => new Topic(id: index + 1, name: name))
            .ToList();

        var students = studentsWithTopics.Select(s => new StudentRel(
            id: s.Id,
            index: s.Index,
            name: s.Name,
            gender: s.Gender,
            active: s.Active,
            departmentId: s.DepartmentId
        )).ToList();

        var studentTopicPairs = studentsWithTopics
            .SelectMany(student => student.Topics, (student, topicName) => new {
                StudentId = student.Id,
                TopicName = topicName
            });

        var studentToTopics = studentTopicPairs
            .Select(pair => new StudentToTopic(
                studentId: pair.StudentId,
                topicId: topics.First(topic => topic.Name == pair.TopicName).Id
            ))
            .ToList();

        return (students, topics, studentToTopics);
    }
}


class Dog {
    public string woof(bool isLoud, int times) {
        string result = string.Concat(Enumerable.Repeat("woof! ", times));
        return isLoud ? result.ToUpper() : result;
    }
}


public class Department(int id, string name) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;

    public override string ToString() {
        return $"{Id,2}), {Name,11}";
    }
}


public enum Gender {
    Female,
    Male
}

public class Topic(int id, string name) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;

    public override string ToString() {
        return $"{Id,2}), {Name,11}";
    }
}


public class Student(int id, int index, string name, Gender gender, bool active, int departmentId, List<int> topicIds) {
    public int Id { get; set; } = id;
    public int Index { get; set; } = index;
    public string Name { get; set; } = name;
    public Gender Gender { get; set; } = gender;
    public bool Active { get; set; } = active;
    public int DepartmentId { get; set; } = departmentId;
    public List<int> TopicIds { get; set; } = topicIds;

    public override string ToString() {
        var topics = string.Join(", ", TopicIds);
        return $"{Id,2}) {Index,5}, {Name,11}, {Gender,6},{(Active ? "active" : "no active"),9},{DepartmentId,2}, topic IDs: {topics}";
    }
}

public class StudentRel(int id, int index, string name, Gender gender, bool active, int departmentId) {
    public int Id { get; set; } = id;
    public int Index { get; set; } = index;
    public string Name { get; set; } = name;
    public Gender Gender { get; set; } = gender;
    public bool Active { get; set; } = active;
    public int DepartmentId { get; set; } = departmentId;

    public override string ToString() {
        return $"{Id,2}) {Index,5}, {Name,11}, {Gender,6},{(Active ? "active" : "no active"),9},{DepartmentId,2}";
    }
}

public class StudentToTopic(int studentId, int topicId) {
    public int StudentId { get; set; } = studentId;
    public int TopicId { get; set; } = topicId;

    public override string ToString() {
        return $"StudentId: {StudentId}, TopicId: {TopicId}";
    }
}


public class StudentWithTopics(int id, int index, string name, Gender gender, bool active,
    int departmentId, List<string> topics) {
    public int Id { get; set; } = id;
    public int Index { get; set; } = index;
    public string Name { get; set; } = name;
    public Gender Gender { get; set; } = gender;
    public bool Active { get; set; } = active;
    public int DepartmentId { get; set; } = departmentId;

    public List<string> Topics { get; set; } = topics;

    public override string ToString() {
        var result = $"{Id,2}) {Index,5}, {Name,11}, {Gender,6},{(Active ? "active" : "no active"),9},{DepartmentId,2}, topics: ";
        foreach (var str in Topics)
            result += str + ", ";
        return result;
    }
}


public static class Generator {
    public static int[] GenerateIntsEasy() {
        return [5, 3, 9, 7, 1, 2, 6, 7, 8];
    }

    public static int[] GenerateIntsMany() {
        var result = new int[10000];
        Random random = new ();
        for (int i = 0; i < result.Length; i++)
            result[i] = random.Next(1000);
        return result;
    }

    public static List<string> GenerateNamesEasy() {
        return [
            "Nowak",
            "Kowalski",
            "Schmidt",
            "Newman",
            "Bandingo",
            "Miniwiliger",
            "Showner",
            "Neumann",
            "Rocky",
            "Bruno"
        ];
    }
    public static List<StudentWithTopics> GenerateStudentsWithTopicsEasy() {
        return [
        new StudentWithTopics(1,12345,"Nowak", Gender.Female,true,1,
                ["C#","PHP","algorithms"]),
        new StudentWithTopics(2, 13235, "Kowalski", Gender.Male, true,1,
                ["C#","C++","fuzzy logic"]),
        new StudentWithTopics(3, 13444, "Schmidt", Gender.Male, false,2,
                ["Basic","Java"]),
        new StudentWithTopics(4, 14000, "Newman", Gender.Female, false,3,
                ["JavaScript","neural networks"]),
        new StudentWithTopics(5, 14001, "Bandingo", Gender.Male, true,3,
                ["Java","C#"]),
        new StudentWithTopics(6, 14100, "Miniwiliger", Gender.Male, true,2,
                ["algorithms","web programming"]),
        new StudentWithTopics(11,22345,"Nowak", Gender.Female,true,2,
                ["C#","PHP","web programming"]),
        new StudentWithTopics(12, 23235, "Nowak", Gender.Male, true,1,
                ["C#","C++","fuzzy logic"]),
        new StudentWithTopics(13, 23444, "Showner", Gender.Male, false,2,
                ["Basic","C#"]),
        new StudentWithTopics(14, 24000, "Neumann", Gender.Female, false,3,
                ["JavaScript","neural networks"]),
        new StudentWithTopics(15, 24001, "Rocky", Gender.Male, true,2,
                ["fuzzy logic","C#"]),
        new StudentWithTopics(16, 24100, "Bruno", Gender.Female, false,2,
                ["algorithms","web programming"]),
        ];
    }

    public static List<Department> GenerateDepartmentsEasy() {
        return [
        new Department(1,"Computer Science"),
        new Department(2,"Electronics"),
        new Department(3,"Mathematics"),
        new Department(4,"Mechanics")
        ];
    }
}