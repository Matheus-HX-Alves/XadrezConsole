using System;
using tabuleiro;

namespace xadrez
{
    class Peao : Peca
    {
        private PartidaDeXadrez Partida;
        public Peao(Tabuleiro tab, Cor cor, PartidaDeXadrez Partida) : base(tab, cor)
        {
            this.Partida = Partida;
        }
        public override string ToString()
        {
            return "P";
        }

        public bool existeInimigo(Posicao pos)
        {
            Peca p = Tab.peca(pos);
            return p != null && p.Cor != Cor;
        }

        public Boolean Livre (Posicao pos)
        {
            return Tab.peca(pos) == null;
        }

        public override bool[,] movimentosPossiveis()
        {
            bool[,] mat = new bool[Tab.Linhas, Tab.Colunas];

            Posicao pos = new Posicao(0, 0);

            if (Cor == Cor.Branca)
            {
                pos.definirValores(Posicao.Linha - 1, Posicao.Coluna);
                if (Tab.posicaoValida(pos) && Livre(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha - 2, Posicao.Coluna);
                if (Tab.posicaoValida(pos) && Livre(pos) && QtdeMovimentos == 0)
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha - 1, Posicao.Coluna - 1);
                if (Tab.posicaoValida(pos) && existeInimigo(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha - 1, Posicao.Coluna + 1);
                if (Tab.posicaoValida(pos) && existeInimigo(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }
                //#JogadaEspecial EnPassant
                if(Posicao.Linha == 3)
                {
                    Posicao esquerda = new Posicao(Posicao.Linha, Posicao.Coluna - 1);
                        if (Tab.posicaoValida(esquerda) && existeInimigo(esquerda) && Tab.peca(esquerda)  == Partida.vulneravelEnPassant){
                        mat[esquerda.Linha, esquerda.Coluna] = true;
                    }
                    Posicao direita = new Posicao(Posicao.Linha, Posicao.Coluna + 1);
                    if (Tab.posicaoValida(direita) && existeInimigo(direita) && Tab.peca(direita) == Partida.vulneravelEnPassant)
                    {
                        mat[direita.Linha, direita.Coluna] = true;
                    }
                }

            }
            else
            {
                pos.definirValores(Posicao.Linha + 1, Posicao.Coluna);
                if (Tab.posicaoValida(pos) && Livre(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha + 2, Posicao.Coluna);
                if (Tab.posicaoValida(pos) && Livre(pos) && QtdeMovimentos == 0)
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha + 1, Posicao.Coluna - 1);
                if (Tab.posicaoValida(pos) && existeInimigo(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                pos.definirValores(Posicao.Linha + 1, Posicao.Coluna + 1);
                if (Tab.posicaoValida(pos) && existeInimigo(pos))
                {
                    mat[pos.Linha, pos.Coluna] = true;
                }

                //#JogadaEspecial EnPassant
                if (Posicao.Linha == 4)
                {
                    Posicao esquerda = new Posicao(Posicao.Linha, Posicao.Coluna - 1);
                    if (Tab.posicaoValida(esquerda) && existeInimigo(esquerda) && Tab.peca(esquerda) == Partida.vulneravelEnPassant)
                    {
                        mat[esquerda.Linha, esquerda.Coluna] = true;
                    }
                    Posicao direita = new Posicao(Posicao.Linha, Posicao.Coluna + 1);
                    if (Tab.posicaoValida(direita) && existeInimigo(direita) && Tab.peca(direita) == Partida.vulneravelEnPassant)
                    {
                        mat[direita.Linha, direita.Coluna] = true;
                    }
                }
            }
            return mat;
        }
    }
}
