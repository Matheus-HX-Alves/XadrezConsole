﻿using tabuleiro;
using System.Collections.Generic;
using System.IO;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }
 
        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8,8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            xeque = false;
            terminada = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public void mudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca) {
                return Cor.Preta;
            } else {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach (Peca x in pecasEmJogo(cor)) {
                if (x is Rei)
                { 
                    return x; 
                }            
            }
            return null;
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

        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimento();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);

            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimento();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimento();
                tab.colocarPeca(T, destinoT);
            }
            // #jogadaespecial EnPassant
            if(p is Peao && origem.Coluna != destino.Coluna && pecaCapturada == null) {
                Posicao posP;
                if(p.Cor == Cor.Branca)
                {
                    posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    
                } else {
                    posP = new Posicao(destino.Linha - 1, destino.Coluna);
                }
                pecaCapturada = tab.retirarPeca(posP);
                capturadas.Add(pecaCapturada);
            }

            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimento();

            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);

            //#JogadaEspecial RoquePequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);

                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimento();
                tab.colocarPeca(T, origemT);
            }

            //#JogadaEspecial RoqueGrande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);

                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimento();
                tab.colocarPeca(T, origemT);
            }

            //#JogadaEspecial EnPassant 
            if(p is Peao)
            {
                if(origem.Coluna != destino.Coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posP;

                    if(peao.Cor == Cor.Branca)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    } else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }
                    tab.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino);
            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual))) {
                xeque = true;
            } else {
                xeque = false;
            } if (TesteXequeMate(adversaria(jogadorAtual))) {
                terminada = true;
            } else {
                turno++;
                mudaJogador();
            }
            Peca p = tab.peca(destino);
            //#JogadaEspecialEnPassant
            if (p is Peao && (destino.Linha == (origem.Linha -2) || destino.Linha == (origem.Linha + 2))) {
                vulneravelEnPassant = p;
            }
            else  {
                vulneravelEnPassant = null;
            }
        }


        public void validarPosicaoDeOrigem(Posicao pos)
        {
            if (tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");

            }
            if(jogadorAtual != tab.peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existem movimentos possíveis para a peça escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posicação de destino inválida!");
            }
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        public bool estaEmXeque(Cor cor) {
            Peca R = rei(cor);
            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach (Peca x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.Posicao.Linha, R.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool TesteXequeMate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i = 0; i < tab.Linhas; i++)
                {
                    for (int j = 0; j < tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecasCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecasCapturada);
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
            colocarNovaPeca('A', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('B', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('C', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('D', 1, new Dama(tab, Cor.Branca));
            colocarNovaPeca('E', 1, new Rei(tab, Cor.Branca, this));
            colocarNovaPeca('F', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('G', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('H', 1, new Torre(tab, Cor.Branca));

            colocarNovaPeca('A', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('B', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('C', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('D', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('E', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('F', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('G', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('H', 2, new Peao(tab, Cor.Branca, this));

            colocarNovaPeca('A', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('B', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('C', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('D', 8, new Dama(tab, Cor.Preta));
            colocarNovaPeca('E', 8, new Rei(tab, Cor.Preta, this));
            colocarNovaPeca('F', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('G', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('H', 8, new Torre(tab, Cor.Preta));

            colocarNovaPeca('A', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('B', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('C', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('D', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('E', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('F', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('G', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('H', 7, new Peao(tab, Cor.Preta, this));
        }
    }
}
