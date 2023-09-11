using System.Net.Sockets;
using System.Text;

namespace future_chat_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string host;
            int porta;

            if (args.Length == 2)
            {
                host = args[0];
                porta = int.Parse(args[1]);
            }
            else
            {
                Console.Write("Informe o host: ");
                host = Console.ReadLine();

                Console.Write("Informe a porta: ");
                porta = int.Parse(Console.ReadLine());
            }

            ProcessaConexao(host, porta);
        }
        private static async Task ProcessaConexao(string host, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient(host, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
                {
                    Console.WriteLine("[i] Bem-vindo ao FutureChat");

                    //Task pra printar na tela mensagem chegando
                    Task leitor = LeitorServidor(reader);

                    while (true)
                    {
                        Console.Write("Entre com a mensage ou '/sair' para sair: ");
                        string message = Console.ReadLine();

                        if (message.ToLower() == "/sair")
                            break;

                        //Posta na stream
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                    }
                    await leitor;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        private static async Task LeitorServidor(StreamReader reader)
        {
            try
            {
                while (true)
                {
                    string chegou = await reader.ReadLineAsync();
                    if (chegou == null)
                    {
                        Console.WriteLine("[i] Finalizada conexão");
                        break;
                    }

                    Console.WriteLine($"[] -> {chegou}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] -> {ex.Message}");
            }
        }
    }
}