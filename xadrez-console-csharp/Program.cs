using System;
using tabuleiro;
using xadrez;

namespace XadrezConsoleCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PartidaDeXadrez partida = new PartidaDeXadrez();


                while (!partida.Terminada)
                {
                    try
                    {
                        Console.Clear();
                        Tela.imprimirTabuleiro(partida.Tab);

                        Console.WriteLine("\nTurno: " + partida.Turno
                            + "\nAguardando jogada: " + partida.JogadorAtual);

                        Console.Write("\nOrigem: ");
                        Posicao origem = Tela.lerPosicaoXadrez().toPosicao();
                        partida.validarPosicaoDeOrgigem(origem);

                        bool[,] posicoesPossives = partida.Tab.Peca(origem).movimentosPossiveis();

                        Console.Clear();
                        Tela.imprimirTabuleiro(partida.Tab, posicoesPossives);

                        Console.Write("Destino: ");
                        Posicao destino = Tela.lerPosicaoXadrez().toPosicao();
                        partida.validarPosicaoDeDestino(origem, destino);

                        partida.realizaJogada(origem, destino);
                    }
                    catch (TabuleiroException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                }

           


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}