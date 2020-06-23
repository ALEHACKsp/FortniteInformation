namespace FortniteInformation
{
    class Program
    {

        static void Main(string[] args)
            => new FortniteInformation().StartAsync().GetAwaiter().GetResult();

    }
}
