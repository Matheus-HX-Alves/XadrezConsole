using System;
using System.Collections.Generic;
using tabuleiro;
using xadrez;

namespace XadrezConsole
{
    class Tela
    {

        public static void  imprimirPartida(PartidaDeXadrez partida)
        {
            imprimirTabuleiro(partida.tab);
            Console.WriteLine();
            imprimirPecasCapturadas(partida);
            Console.WriteLine();
            Console.WriteLine("Turno: " + partida.turno);
            
            if (!partida.terminada) { 
                Console.WriteLine("Aguardando a jogada: " + partida.jogadorAtual);
                if (partida.xeque)            {
                Console.WriteLine("XEQUE!");
                }
            } else {
                Console.WriteLine("XEQUEMATE!");
                Console.WriteLine("Vencedor: "+partida.jogadorAtual);
            }
        }

        public static void imprimirPecasCapturadas(PartidaDeXadrez partida)
        {
            Console.WriteLine("Peças capturadas: ");
            Console.Write("Brancas: ");
            imprimirConjunto(partida.pecasCapturadas(Cor.Branca));

            Console.Write("Pretas: ");
            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            imprimirConjunto(partida.pecasCapturadas(Cor.Preta));

            Console.ForegroundColor = aux;
        }

        public static void imprimirConjunto(HashSet<Peca> conjunto)
        {
            Console.Write("[");
            foreach (Peca x in conjunto)
            {
                Console.Write(x + " ");
            }
            Console.WriteLine("]");
        }

        public static void imprimirTabuleiro(Tabuleiro tab)
        {
            string posi = "";
            for (int i = 0; i < tab.Linhas; i++)
            {
                Console.Write(" " + (8 - i) + " ");
                for (int j = 0; j < tab.Colunas; j++)
                {
                    if (tab.peca(i, j) is Rei && tab.peca(i, j).Cor == Cor.Preta)
                    {
                        imprimirPeca(tab.peca(i, j));
                        posi = i + "," + j;
                    }
                    else
                    {
                        imprimirPeca(tab.peca(i, j));
                    }
                }
                Console.WriteLine();
            }


            Console.WriteLine("   A B C D E F G H");
            Console.WriteLine(posi);
        }
        public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoesPossiveis)
        {
            ConsoleColor fundoOriginal = Console.BackgroundColor;
            ConsoleColor fundoAlterado = ConsoleColor.DarkGray;

            for (int i = 0; i < tab.Linhas; i++)
            {
                Console.Write(" " + (8 - i) + " ");
                for (int j = 0; j < tab.Colunas; j++)
                {
                    if (posicoesPossiveis[i, j])
                    {
                        Console.BackgroundColor = fundoAlterado;
                    }
                    else {
                        Console.BackgroundColor =  fundoOriginal;
                    }
                    if (tab.peca(i, j) is Rei && tab.peca(i, j).Cor == Cor.Preta)
                    {
                        imprimirPeca(tab.peca(i, j));
                    }
                    else
                    {
                        imprimirPeca(tab.peca(i, j));
                    }
                    Console.BackgroundColor = fundoOriginal;
                }
                Console.WriteLine();

            }
            Console.WriteLine("   A B C D E F G H");
            Console.BackgroundColor = fundoOriginal;
        }
        public static void imprimirPeca(Peca peca)
        {
            if(peca == null)
            {
                Console.Write("- ");
            } else {

                if (peca is Rei && peca.Cor == Cor.Preta)
                {
                    if (peca.Cor == Cor.Branca)
                    {
                        Console.Write(peca);
                    }
                    else
                    {
                        ConsoleColor aux = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(peca);
                        Console.ForegroundColor = aux;
                    }
                    Console.Write(" ");
                }
                else
                {
                    if (peca.Cor == Cor.Branca)
                    {
                        Console.Write(peca);
                    }
                    else
                    {
                        ConsoleColor aux = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(peca);
                        Console.ForegroundColor = aux;
                    }
                    Console.Write(" ");
                }
                
            }
        }

        public static PosicaoXadrez lerPosicaoXadrez()
        {
            string s = Console.ReadLine();
            char coluna = s[0].ToString().ToUpper()[0];
            int linha = int.Parse(s[1] + "");
            return new PosicaoXadrez(coluna, linha);
        }
    }
}
