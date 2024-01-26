
namespace LeapWoF
{
    class Program
    {
        static void Main(string[] args)
        {
            var gm = new GameManager();
            gm.StartGame();
            // JOSH: Added this to prevent the console from closing immediately after the game ends
            Interfaces.IOutputProvider outputProvider = new ConsoleOutputProvider();
            Interfaces.IInputProvider inputProvider = new ConsoleInputProvider();
            outputProvider.WriteLine("Press any key to exit...");
            inputProvider.Read();
        }
    }
}
