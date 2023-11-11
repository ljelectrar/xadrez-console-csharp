using System;
using tabuleiro;
using xadrez_console_csharp;

namespace XadrezConsoleCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Tabuleiro tab = new Tabuleiro(10, 10);

            Tela.imprimirTabuleiro(tab);
        }
    }
}