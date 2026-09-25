using ExamSystem.Domain;

namespace ExamSystem.App;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== Examination System ===");
        Console.WriteLine("1) Final Exam");
        Console.WriteLine("2) Practical Exam");

        ExamType examType = ReadExamType();

        // Declare a subject object and let it create ONE type of exam.
        Subject subject = new(1, "C# OOP");
        Exam exam = subject.CreateExam(examType, timeMinutes: 30, questions: BuildQuestions(examType));

        Console.WriteLine();
        Console.WriteLine(subject);
        Console.WriteLine(exam);
        Console.WriteLine();

        exam.ShowExam();

        // Quick demo of ICloneable / IComparable
        Exam copy = (Exam)exam.Clone();
        Console.WriteLine();
        Console.WriteLine($"Clone is a different object: {!ReferenceEquals(exam, copy)}");
        Console.WriteLine($"exam.CompareTo(copy) = {exam.CompareTo(copy)}");
    }

    private static ExamType ReadExamType()
    {
        while (true)
        {
            Console.Write("Choose exam type (1/2): ");
            string? input = Console.ReadLine()?.Trim();
            if (input == "1") return ExamType.Final;
            if (input == "2") return ExamType.Practical;
            Console.WriteLine("Please enter 1 or 2.");
        }
    }

    private static List<Question> BuildQuestions(ExamType examType)
    {
        var questions = new List<Question>();

        if (examType == ExamType.Final)
        {
            questions.Add(QuestionFactory.CreateTrueOrFalse(examType,
                "OOP", "A class can inherit from more than one class in C#.", 1, correctAnswerIsTrue: false));
            questions.Add(QuestionFactory.CreateTrueOrFalse(examType,
                "OOP", "An abstract class can contain implemented methods.", 1, correctAnswerIsTrue: true));
        }

        questions.Add(QuestionFactory.CreateMcq(examType,
            "OOP", "Which keyword is used to inherit from a class?", 2,
            new[] { "extends", ":", "inherits", "implements" }, rightAnswerId: 2));
        questions.Add(QuestionFactory.CreateMcq(examType,
            "OOP", "Which interface is used to make an object copyable?", 2,
            new[] { "IComparable", "IDisposable", "ICloneable", "IEnumerable" }, rightAnswerId: 3));
        questions.Add(QuestionFactory.CreateMcq(examType,
            "OOP", "Which keyword allows a derived class to change a base method?", 2,
            new[] { "sealed", "override", "static", "const" }, rightAnswerId: 2));

        return questions;
    }
}
