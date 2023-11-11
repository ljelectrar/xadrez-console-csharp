using System;
using tabuleiro;
using xadrez;

namespace XadrezConsoleCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            PosicaoXadrez pos = new PosicaoXadrez('a', 1);
            PosicaoXadrez pos2 = new PosicaoXadrez('c', 7);

            Console.WriteLine(pos);

            Console.WriteLine(pos.toPosicao());
            Console.WriteLine(pos2.toPosicao());


        }
    }
}