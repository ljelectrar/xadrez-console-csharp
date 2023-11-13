using System;
using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }


        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.Branca;

            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();

            colocarPecas();
            Terminada = false;

            vulneravelEnPassant = null;

            xeque = false;
        }

        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.retirarPeca(origem);
            Tab.retirarPeca(destino);
            Peca pecaCapturada = Tab.retirarPeca(destino);
            Tab.colocarPeca(p, destino);
            p.incrementarQtdMovimentos();

            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemDaTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoDaTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.retirarPeca(origemDaTorre);
                T.incrementarQtdMovimentos();
                Tab.colocarPeca(T, destinoDaTorre);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemDaTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoDaTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.retirarPeca(origemDaTorre);
                T.incrementarQtdMovimentos();
                Tab.colocarPeca(T, destinoDaTorre);
            }

            // #jogadaespecial en passant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if (p.Cor == Cor.Branca)
                    {
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = Tab.retirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void validarPosicaoDeOrgigem(Posicao pos)
        {
            if (Tab.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição escolhida!");
            }
            if (JogadorAtual != Tab.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!Tab.Peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existem movimentos possíveis para a peça selecionada!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!Tab.Peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posicão de destino inválida!");
            }
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.retirarPeca(destino);
            p.decrementarQtdMovimentos();
            if (pecaCapturada != null)
            {
                Tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }

            Tab.colocarPeca(p, origem);


            // #jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemDaTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoDaTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.retirarPeca(destinoDaTorre);
                T.decrementarQtdMovimentos();
                Tab.colocarPeca(T, origemDaTorre);
            }

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemDaTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoDaTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.retirarPeca(destinoDaTorre);
                T.decrementarQtdMovimentos();
                Tab.colocarPeca(T, origemDaTorre);
            }

            // #jogadaespecial en passant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peao = Tab.retirarPeca(destino);
                    Posicao posP;
                    if (p.Cor == Cor.Branca)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }
                    Tab.colocarPeca(peao, posP);
                }
            }

        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCaptura = executaMovimento(origem, destino);

            if (estaEmXeque(JogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCaptura);
                throw new TabuleiroException("Você não pode se colocar em xeque.");
            }

            Peca peca = Tab.Peca(destino);

            // #joagadaespecial promoção
            if (peca is Peao)
            {
                if ((peca.Cor == Cor.Branca && destino.Linha == 0)
                    || (peca.Cor == Cor.Preta && destino.Linha == 7))
                {
                    peca = Tab.retirarPeca(destino);
                    pecas.Remove(peca);
                    Peca dama = new Dama(Tab, peca.Cor);
                    Tab.colocarPeca(dama, destino);
                    pecas.Add(dama);
                }
            }
           
            if (estaEmXeque(adversaria(JogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }
            if (testeXequeMate(adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                mudaJogador();
            }

            // #jogadaespecial en passant
            if (peca is Peao && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
            {
                vulneravelEnPassant = peca;
            }
            else
            {
                vulneravelEnPassant = null;
            }

        }

        private void mudaJogador()
        {
            if (JogadorAtual == Cor.Branca)
            {
                JogadorAtual = Cor.Preta;
            }
            else
            {
                JogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in capturadas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in pecas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }

            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }
        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach (Peca peca in pecasEmJogo(cor))
            {
                if (peca is Rei)
                {
                    return peca;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);

            if (R == null)
            {
                throw new TabuleiroException("Não tem Rei da cor " + cor + " no tabuleiro!");
            }

            foreach (Peca peca in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = peca.movimentosPossiveis();
                if (mat[R.Posicao.Linha, R.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequeMate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }

            foreach (Peca peca in pecasEmJogo(cor))
            {
                bool[,] mat = peca.movimentosPossiveis();
                for (int i = 0; i < Tab.Linhas; i++)
                {
                    for (int j = 0; j < Tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = peca.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void colocarPecas()
        {

            //Brancas
            colocarNovaPeca('a', 1, new Torre(Tab, Cor.Branca));
            colocarNovaPeca('h', 1, new Torre(Tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Cavalo(Tab, Cor.Branca));
            colocarNovaPeca('g', 1, new Cavalo(Tab, Cor.Branca));
            colocarNovaPeca('c', 1, new Bispo(Tab, Cor.Branca));
            colocarNovaPeca('f', 1, new Bispo(Tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Rei(Tab, Cor.Branca, this));
            colocarNovaPeca('e', 1, new Dama(Tab, Cor.Branca));
            colocarNovaPeca('a', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('b', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('c', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('d', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('e', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('f', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('g', 2, new Peao(Tab, Cor.Branca, this));
            colocarNovaPeca('h', 2, new Peao(Tab, Cor.Branca, this));

            // Pretas
            colocarNovaPeca('a', 8, new Torre(Tab, Cor.Preta));
            colocarNovaPeca('h', 8, new Torre(Tab, Cor.Preta));
            colocarNovaPeca('b', 8, new Cavalo(Tab, Cor.Preta));
            colocarNovaPeca('g', 8, new Cavalo(Tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Bispo(Tab, Cor.Preta));
            colocarNovaPeca('f', 8, new Bispo(Tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Rei(Tab, Cor.Preta, this));
            colocarNovaPeca('e', 8, new Dama(Tab, Cor.Preta));
            colocarNovaPeca('a', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('b', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('c', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('d', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('e', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('f', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('g', 7, new Peao(Tab, Cor.Preta, this));
            colocarNovaPeca('h', 7, new Peao(Tab, Cor.Preta, this));

        }

    }
}
