using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ATM
{
    class Program
    {
        public static List<int> ResultBancnotes = new List<int>();
        static void Main(string[] args)
        {
            //Путь к файлу, в котором находится информация о банкнотах в банкомате.
            string filePath = @"C:\Temp\ATM.txt";

            //Считываем число, которое необходимо выдать банкнотами, если не получается - выдаём ошибку.
            var inputString = Console.ReadLine();
            Int32.TryParse(inputString, out var input);
            if (input == 0)
            {
                ErrorMessage();
                return;
            }

            //Пытаемся прочитать банкноты из банкомата. Иначе - выдаём ошибку.
            var containsInATM = ParseTextFile(filePath);
            if (containsInATM == null)
            {
                ErrorMessage();
                return;
            }

            //Парсим прочитанные банкноты в список.
            var bancnotes = ParseToList(containsInATM);

            //Основная функция, которая выдаёт ответ.
            FillResultBancnotes(input, bancnotes);

            //Если получилось "набрать" необходимые банкноты, то выводим данные в необходимом формате.
            //Иначе - выдаём ошибку.
            if (ResultBancnotes.Count != 0)
            {
                foreach (var bancnote in ResultBancnotes.Distinct())
                {
                    System.Console.WriteLine($"{bancnote}:{ResultBancnotes.Where(o => o == bancnote).Count()}");
                }
            }
            else
            {
                ErrorMessage();
                return;
            }
        }
        public static void ErrorMessage()
        {
            System.Console.WriteLine("Невозможно выдать банкноты");
        }
        public static void FillResultBancnotes(int Sum, List<int> bancnotes)
        {
            FindSumByBancnotes(Sum, bancnotes);
        }

        //Функция, которая рекурсивно вызывается.
        //Берёт на вход сумму, которую нужно найти и список доступных банкнот.
        public static List<int> FindSumByBancnotes (int Sum, List<int> bancnotes)
        {
            for (int i = 0; i <= bancnotes.Count - 1; i++)
            {
                var tempSum = bancnotes[i];
                ResultBancnotes.Add(bancnotes[i]);
                if (tempSum < Sum)
                {
                    var answer = FindSumByBancnotes((Sum - tempSum), bancnotes.GetRange(i + 1, bancnotes.Count - i - 1));
                    if (answer != null)
                        return answer;
                    else
                    {
                        ResultBancnotes.Remove(bancnotes[i]);
                        continue;
                    }
                }
                else if (tempSum == Sum)
                    return new List<int> { tempSum };
                else
                {
                    ResultBancnotes.Remove(bancnotes[i]);
                    continue;
                }
            }
            return null;
        }
        public static List<int> ParseToList(Dictionary<int,int> ATM)
        {
            var bancnotes = new List<int>();
            foreach (var rating in ATM.Keys)
            {
                for (int i = 0; i < ATM[rating]; i++)
                {
                    bancnotes.Add(rating);
                }
            }
            return bancnotes;
        }
        public static Dictionary<int, int> ParseTextFile(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    var fullText = sr.ReadToEnd();
                    var splitText = fullText.Split("\r\n");
                    var ATM = new Dictionary<int, int>();

                    foreach (var bill in splitText)
                    {
                        int rating = Int32.Parse(bill.Split(':')[0]);
                        int count = Int32.Parse(bill.Split(':')[1]);

                        if (ATM.ContainsKey(rating) == false)
                            ATM.Add(rating, count);

                    }
                    return ATM.OrderByDescending(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
