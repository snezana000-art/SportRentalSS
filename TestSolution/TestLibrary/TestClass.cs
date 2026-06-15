namespace TestLibrary
{
    public class Class1
    {
        // a. Числовые поля x и y
        public int x;
        public int y;

        // b. Метод для вывода информации об объекте
        public void PrintInfo()
        {
            Console.WriteLine($"x = {x}, y = {y}");
        }

        // c. Метод, возвращающий значение x^y
        public double Power()
        {
            return Math.Pow(x, y);
        }
    }
}
