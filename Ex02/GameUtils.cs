using System;
using System.Threading;
using System.Text;

namespace Ex02
{
    static class GameUtils
    {
        private static string m_Player1Name;
        private static string m_Player2Name;
        private static bool m_SinglePlayerMode;
        private static byte m_NumberOfRows;
        private static byte m_NumberOfColumns;
        private static bool m_Player1TurnToPlay;
        private static Player m_Player1;
        private static Player m_Player2;
        private static Pc m_Pc;
        private static Board m_Board;
        private static bool m_ContinueGame;
        private static byte m_InputOfTurn1Row;
        private static byte m_InputOfTurn1Column;
        private static byte m_InputOfTurn2Row;
        private static byte m_InputOfTurn2Column;
        private static char m_ValueFoundInTurn1;
        private static char m_ValueFoundInTurn2;
        private static bool m_StartAnotherGame;
        private static bool m_HoldPcSecondTurn;

        public static void Play()
        {
            m_StartAnotherGame = true;
            while (m_StartAnotherGame)
            {
                ConsoleUtils.Screen.Clear();
                insertName();
                choosePlayingMode();
                insertSecondNameIfNeeded();
                createPlayers();
                determineBoardSize();
                createBoard();
                m_Player1TurnToPlay = true;
                m_ContinueGame = true;
                while (m_ContinueGame)
                {
                    printBoardToConsole();
                    playTurn(1, ref m_InputOfTurn1Row, ref m_InputOfTurn1Column, ref m_ValueFoundInTurn1);
                    printBoardToConsole();
                    bool isEqual = false;

                    if (m_ContinueGame)
                    {
                        printBoardToConsole();
                        playTurn(2, ref m_InputOfTurn2Row, ref m_InputOfTurn2Column, ref m_ValueFoundInTurn2);
                        printBoardToConsole();
                        isEqual = m_Board.CheckForSlotValueMatch(m_InputOfTurn1Row, m_InputOfTurn1Column,
                                                                      m_InputOfTurn2Row, m_InputOfTurn2Column);
                        if (isEqual)
                        {
                            m_Board.UpdateCouplesFoundOnBoard();
                            if (m_Player1TurnToPlay)
                            {
                                m_Player1.Score++;
                            }
                            else
                            {
                                if (m_SinglePlayerMode)
                                {
                                    m_Pc.Score++;
                                }
                                else
                                {
                                    m_Player2.Score++;
                                }
                            }
                            m_ContinueGame = !m_Board.IsBoardFullyUnveiled;
                        }
                        else
                        {
                            bool visible = true;
                            m_Board.SetSlotVisibility(!visible, m_InputOfTurn1Row, m_InputOfTurn1Column);
                            m_Board.SetSlotVisibility(!visible, m_InputOfTurn2Row, m_InputOfTurn2Column);
                            Thread.Sleep(2000);
                            m_Player1TurnToPlay = !m_Player1TurnToPlay;
                        }
                        Thread.Sleep(2000);
                    }

                    if (m_SinglePlayerMode)
                    {
                        m_Pc.SaveSlotToMemory(m_ValueFoundInTurn1, m_InputOfTurn1Row, m_InputOfTurn1Column);
                        m_Pc.SaveSlotToMemory(m_ValueFoundInTurn2, m_InputOfTurn2Row, m_InputOfTurn2Column);
                        if (isEqual)
                        {
                            m_Pc.RemoveMatchedSlots(m_ValueFoundInTurn1);
                        }
                    }
                }

                printScores();
                askForAnotherGame();
            }
        }

        private static void insertName()
        {
            Console.Write("Welcome to the Memory Game!\nPlease insert your name: ");
            m_Player1Name = Console.ReadLine();
        }

        private static void choosePlayingMode()
        {
            Console.Write("\nFor two-player mode insert 'Y' then press ENTER. Otherwise press ENTER for single-player mode: ");
            if (Console.ReadLine() == "Y")
            {
                m_SinglePlayerMode = false;
            }
            else
            {
                m_SinglePlayerMode = true;
            }
        }

        private static void insertSecondNameIfNeeded()
        {
            if (false == m_SinglePlayerMode)
            {
                Console.Write("Please insert second player's name: ");
                m_Player2Name = Console.ReadLine();
            }
        }

        private static void determineBoardSize()
        {
            Console.WriteLine("\nChoose your desired size of game board:\n" +
                "The rows and columns can be in range of 4 to 6 except for the combination 5x5");
            Console.Write("Number of rows: ");
            string numberOfRowsStr = Console.ReadLine();
            Console.Write("Number of columns: ");
            string numberOfColumnsStr = Console.ReadLine();
            bool validBoardSizeInput = isValidBoardSizeInput(numberOfRowsStr, numberOfColumnsStr);
            while (false == validBoardSizeInput)
            {
                Console.WriteLine("\nInvalid input, please follow the instructions above.");
                Console.Write("Number of rows: ");
                numberOfRowsStr = Console.ReadLine();
                Console.Write("Number of columns: ");
                numberOfColumnsStr = Console.ReadLine();
                validBoardSizeInput = isValidBoardSizeInput(numberOfRowsStr, numberOfColumnsStr);
            }

            m_NumberOfRows = byte.Parse(numberOfRowsStr);
            m_NumberOfColumns = byte.Parse(numberOfColumnsStr);
        }

        private static bool isValidBoardSizeInput(string i_NumberOfRows, string i_NumberOfColumns)
        {
            bool returnValue = true;
            if ((i_NumberOfRows != "4" && i_NumberOfRows != "5" && i_NumberOfRows != "6") ||
                (i_NumberOfColumns != "4" && i_NumberOfColumns != "5" && i_NumberOfColumns != "6") ||
                (i_NumberOfRows == "5" && i_NumberOfColumns == "5"))
            {
                returnValue = false;
            }

            return returnValue;
        }

        private static void createPlayers()
        {
            m_Player1 = new Player(m_Player1Name);
            if (m_SinglePlayerMode)
            {
                m_Pc = new Pc();
            }
            else
            {
                m_Player2 = new Player(m_Player2Name);
            }
        }

        private static void createBoard()
        {
            m_Board = new Board(m_NumberOfRows, m_NumberOfColumns);
        }

        private static void printBoardToConsole()
        {
            ConsoleUtils.Screen.Clear();

            StringBuilder boardSb = new StringBuilder();
            boardSb.Append("    A   B   C   D");
            if (5 == m_Board.Columns)
            {
                boardSb.Append("   E");
            }
            if (6 == m_Board.Columns)
            {
                boardSb.Append("   E   F");
            }

            boardSb.Append("\n  ");
            boardSb.Append('=', (m_Board.Columns * 4) + 1);

            for (int row = 0; row < m_Board.Rows; row++)
            {
                boardSb.Append("\n" + (row + 1) + " ");
                for (int column = 0; column < m_Board.Columns; column++)
                {
                    boardSb.Append("| " + m_Board.GetValueOfSlotIfVisible(row, column) + " ");
                }

                boardSb.Append("|\n  ");
                boardSb.Append('=', (m_Board.Columns * 4) + 1);
            }

            Console.WriteLine(boardSb.ToString());
        }

        private static void playTurn(byte i_TurnNumber, ref byte io_RowOfInput, ref byte io_ColumnOfInput, ref char io_ValueFound)
        {
            string nameOfCurrentPlayer;
            if (m_Player1TurnToPlay)
            {
                nameOfCurrentPlayer = m_Player1.Name;
            }
            else
            {
                nameOfCurrentPlayer = m_SinglePlayerMode ? m_Pc.Name : m_Player2.Name;
            }

            Console.WriteLine("\nPlease select two hidden slots on the board." +
                "\nThe input needs to be 2 characters in the range of [A-{0}][1-{1}]. For example A3 or C1." +
                "\nTo exit insert 'Q' then ENTER.", (char)('A' + m_NumberOfColumns - 1), m_NumberOfRows);
            Console.Write("\n{0}'s turn:\nSlot #{1}: ", nameOfCurrentPlayer, i_TurnNumber);

            if (!m_SinglePlayerMode || m_Player1TurnToPlay)
            {
                turnOfPlayer(ref io_ValueFound, ref io_RowOfInput, ref io_ColumnOfInput);
            }
            else
            {
                turnOfPc(i_TurnNumber, ref io_ValueFound, ref io_RowOfInput, ref io_ColumnOfInput);
            }
        }

        private static void turnOfPlayer(ref char io_ValueFound, ref byte io_RowOfInput, ref byte io_ColumnOfInput)
        {
            string slotInput = "";
            bool flipSucceeded = false;
            while (!flipSucceeded && !slotInput.Equals("Q"))
            {
                slotInput = getValidSlotInput();
                if (!slotInput.Equals("Q"))
                {
                    convertSlotPositionToBoardIndex(slotInput, ref io_RowOfInput, ref io_ColumnOfInput);
                    flipSucceeded = m_Board.TryFlipCard(io_RowOfInput, io_ColumnOfInput, ref io_ValueFound);
                    if (!flipSucceeded)
                    {
                        Console.WriteLine("Invalid input - the slot you chose is already visible. Insert again:");
                    }
                }
            }
        }

        private static void turnOfPc(byte i_TurnNumber, ref char io_ValueFound, ref byte io_RowOfInput, ref byte io_ColumnOfInput)
        {
            if (1 == i_TurnNumber)
            {
                m_HoldPcSecondTurn = false;
                int[] arrayOfTwoSlots = m_Pc.GetPositionOfTwoRandomSlots();
                int rowsAndColumnsOfTwoSlots = 4;
                if (rowsAndColumnsOfTwoSlots == arrayOfTwoSlots.Length)
                {
                    io_RowOfInput = (byte)arrayOfTwoSlots[0];
                    io_ColumnOfInput = (byte)arrayOfTwoSlots[1];
                    m_InputOfTurn2Row = (byte)arrayOfTwoSlots[2];
                    m_InputOfTurn2Column = (byte)arrayOfTwoSlots[3];
                    m_HoldPcSecondTurn = true;
                }
                else
                {
                    byte[] hiddenSlot = m_Pc.GetRandomHiddenRowAndColumn(m_Board);
                    io_RowOfInput = hiddenSlot[0];
                    io_ColumnOfInput = hiddenSlot[1];
                }
            }
            else
            {
                if (m_HoldPcSecondTurn)
                {
                    io_RowOfInput = m_InputOfTurn2Row;
                    io_ColumnOfInput = m_InputOfTurn2Column;
                }
                else
                {
                    int[] SlotFromSinglesDict = m_Pc.GetPositionOfSingleSlot(m_ValueFoundInTurn1);
                    if (SlotFromSinglesDict.Length > 0)
                    {
                        io_RowOfInput = (byte)SlotFromSinglesDict[0];
                        io_ColumnOfInput = (byte)SlotFromSinglesDict[1];
                    }
                    else
                    {
                        byte[] hiddenSlot = m_Pc.GetRandomHiddenRowAndColumn(m_Board);
                        io_RowOfInput = hiddenSlot[0];
                        io_ColumnOfInput = hiddenSlot[1];
                    }
                }
            }

            m_Board.TryFlipCard(io_RowOfInput, io_ColumnOfInput, ref io_ValueFound);
        }

        private static string getValidSlotInput()
        {
            string selectionInput = Console.ReadLine();
            bool validInput = false;
            while (!validInput)
            {
                if (selectionInput.Length != 2)
                {
                    if (selectionInput.Equals("Q"))
                    {
                        validInput = true;
                        m_ContinueGame = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input! Insert a character in range A-{0} then a number in range 1-{1}: ",
                            (char)('A' + m_NumberOfColumns - 1), m_NumberOfRows);
                        selectionInput = Console.ReadLine();
                    }
                }
                else
                {
                    if ((0 <= selectionInput[0] - 'A') && (selectionInput[0] - 'A' < m_NumberOfColumns)
                        && (0 < selectionInput[1] - '0' && selectionInput[1] - '0' <= m_NumberOfRows))
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input! There is no such slot as {0} on the board. Insert again: ", selectionInput);
                        selectionInput = Console.ReadLine();
                    }
                }
            }

            return selectionInput;
        }

        private static void convertSlotPositionToBoardIndex(string i_InputToCast, ref byte o_Row, ref byte o_Column)
        {
            int rowAsInt = int.Parse(i_InputToCast[1].ToString());
            o_Row = (byte)(rowAsInt - 1);
            o_Column = (byte)(i_InputToCast[0] - 'A');
        }

        private static void printScores()
        {
            string nameOfWinner;
            ConsoleUtils.Screen.Clear();
            if (m_Player1.Score == (m_SinglePlayerMode ? m_Pc.Score : m_Player2.Score))
            {
                Console.WriteLine("It's a tie !");
            }
            else
            {
                if (m_SinglePlayerMode)
                {
                    nameOfWinner = m_Player1.Score > m_Pc.Score ? m_Player1.Name : m_Pc.Name;
                }
                else
                {
                    nameOfWinner = m_Player1.Score > m_Player2.Score ? m_Player1.Name : m_Player2.Name;
                }
                Console.WriteLine("The winner is {0} !!", nameOfWinner);
            }
            Console.WriteLine("[Player1] {0}'s score: {1}", m_Player1.Name, m_Player1.Score);
            Console.WriteLine("[Player2] {0}'s score: {1}", m_SinglePlayerMode ? m_Pc.Name : m_Player2.Name,
                                                            m_SinglePlayerMode ? m_Pc.Score : m_Player2.Score);
        }

        private static void askForAnotherGame()
        {
            Console.Write("\nTo start a new game insert 'Y' then press ENTER. Otherwise press ENTER to exit: ");
            if (Console.ReadLine() == "Y")
            {
                m_StartAnotherGame = true;
            }
            else
            {
                m_StartAnotherGame = false;
            }
        }
    }
}
