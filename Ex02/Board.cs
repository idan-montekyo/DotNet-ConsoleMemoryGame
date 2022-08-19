using System;
using System.Linq;

namespace Ex02
{
    class Board
    {
        private Slot[,] m_SlotsMatrix;
        private static byte m_NumberOfSlotsUnveiled;

        public int Rows
        {
            get
            {
                return this.m_SlotsMatrix.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return this.m_SlotsMatrix.GetLength(1);
            }
        }

        public Slot[,] Matrix
        {
            get
            {
                return m_SlotsMatrix;
            }
        }

        public bool IsBoardFullyUnveiled
        {
            get
            {
                return m_NumberOfSlotsUnveiled == (Rows * Columns);
            }
        }

        public Board(byte i_NumberOfRows, byte i_NumberOfColumns)
        {
            m_SlotsMatrix = new Slot[i_NumberOfRows, i_NumberOfColumns];
            m_NumberOfSlotsUnveiled = 0;

            char[] shuffledArray = getShuffledArray(i_NumberOfRows * i_NumberOfColumns);
            for (int i = 0; i < i_NumberOfRows; i++)
            {
                for (int j = 0; j < i_NumberOfColumns; j++)
                {
                    Slot tempSlot = new Slot(shuffledArray[i * i_NumberOfColumns + j]);
                    m_SlotsMatrix[i, j] = tempSlot;
                }
            }
        }

        private static char[] getShuffledArray(int i_Size)
        {
            char[] fullCharArrayOfSize36 = {'A', 'A', 'B', 'B', 'C', 'C', 'D', 'D', 'E', 'E', 'F', 'F', 
                                            'G', 'G', 'H', 'H', 'I', 'I', 'J', 'J', 'K', 'K', 'L', 'L', 
                                            'M', 'M', 'N', 'N', 'O', 'O', 'P', 'P', 'Q', 'Q', 'R', 'R'};

            char[] desiredSizeCharArray = fullCharArrayOfSize36.Take(i_Size).ToArray();

            Random random = new Random();
            char[] shuffledCharArray = desiredSizeCharArray.OrderBy(x => random.Next()).ToArray();
            return shuffledCharArray;
        }

        public char GetValueOfSlotIfVisible(int i_Row, int i_Column)
        {
            Slot slot = this.m_SlotsMatrix[i_Row, i_Column];
            char returnValue = ' ';
            if (!slot.IsHidden)
            {
                returnValue = slot.Value;
            }

            return returnValue;
        }

        public bool TryFlipCard(in byte i_Row, in byte i_Column, ref char o_CharRetreived)
        {
            bool IsSucceeded = false;
            Slot slotChosen = m_SlotsMatrix[i_Row, i_Column];

            if (slotChosen.IsHidden)
            {
                slotChosen.IsHidden = false;
                o_CharRetreived = slotChosen.Value;
                IsSucceeded = true;
            }

            return IsSucceeded;
        }

        public void SetSlotVisibility(bool i_IsVisible, byte i_Row, byte i_Column)
        {
            m_SlotsMatrix[i_Row, i_Column].IsHidden = !i_IsVisible;
        }

        public bool CheckForSlotValueMatch(byte i_Row1, byte i_Column1, byte i_Row2, byte i_Column2)
        {
            return m_SlotsMatrix[i_Row1, i_Column1].Value == m_SlotsMatrix[i_Row2, i_Column2].Value;
        }

        public void UpdateCouplesFoundOnBoard()
        {
            m_NumberOfSlotsUnveiled += 2;
        }
    }
}
