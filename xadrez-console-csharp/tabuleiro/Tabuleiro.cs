namespace tabuleiro
{
    class Tabuleiro
    {
        public int Linhas { get; init; }
        public int Colunas { get; set; }

        private Peca[,] pecas;

        public Tabuleiro(int linhas, int colunas)
        {
            Linhas = linhas;
            Colunas = colunas;
            pecas = new Peca[Linhas, Colunas];
        }
    }
}
