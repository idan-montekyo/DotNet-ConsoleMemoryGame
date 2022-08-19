using System;
using System.Collections.Generic;

namespace Ex02
{
    class Pc
    {
        private byte m_Score;
        private Dictionary<char, int[]> m_SingleSlotFoundDictionary;
        private Dictionary<char, int[]> m_MatchedSlotsFoundDictionary;

        public Pc()
        {
            this.m_Score = 0;
            this.m_SingleSlotFoundDictionary = new Dictionary<char, int[]>();
            this.m_MatchedSlotsFoundDictionary = new Dictionary<char, int[]>();
        }

        public string Name
        {
            get
            {
                return "PC";
            }
        }

        public byte Score
        {
            get
            {
                return this.m_Score;
            }
            set
            {
                this.m_Score = value;
            }
        }

        public byte[] GetRandomHiddenRowAndColumn(Board i_Board)
        {
            Random random = new Random();
            byte randomRow = 0;
            byte randomColumn = 0;

            bool validSlot = false;
            while (!validSlot)
            {
                randomRow = (byte)random.Next(i_Board.Rows);
                randomColumn = (byte)random.Next(i_Board.Columns);

                if (i_Board.Matrix[randomRow, randomColumn].IsHidden)
                {
                    validSlot = true;

                    char raffledValue = i_Board.Matrix[randomRow, randomColumn].Value;
                    if (IsContainInSingleSlotsDictionary(raffledValue))
                    {
                        int[] positionsOfExistingSlotInSinglesDict = GetPositionOfSingleSlot(raffledValue);
                        if (randomRow == positionsOfExistingSlotInSinglesDict[0] && randomColumn == positionsOfExistingSlotInSinglesDict[1])
                        {
                            validSlot = false;
                        }
                    }
                }
            }

            byte[] returnArray = { randomRow, randomColumn };

            return returnArray;
        }

        /// <summary>
        /// Saves the slot information in a dictionary as Char-Array<int> couples.
        /// The algorithm in short:
        ///     1) Check if CouplesDictionary already contains the key.
        ///        2.1) If yes: do nothing.
        ///        2.2) If no:
        ///             3) Check if SinglesDictionary already contains the key.
        ///                4.1) If no: add to SinglesDictionary.
        ///                4.2) If yes:
        ///                     5) Check if the positions of the existing slot and the new one are equal.
        ///                        6.1) If yes: do nothing.
        ///                        6.2) If no: remove from SinglesDict and add ALL FOUR positions to a new value in CouplesDict.
        /// </summary>
        public void SaveSlotToMemory(char i_Value, byte i_Row, byte i_Column)
        {
            if (m_MatchedSlotsFoundDictionary.ContainsKey(i_Value))
            {
                ;
            }
            else
            {
                if (m_SingleSlotFoundDictionary.ContainsKey(i_Value))
                {
                    int[] positionsArrayFromSinglesDict = GetPositionOfSingleSlot(i_Value);
                    if (i_Row == positionsArrayFromSinglesDict[0] && i_Column == positionsArrayFromSinglesDict[1])
                    {
                        ;
                    }
                    else
                    {
                        int[] positionsOfBothSlotsContainingRelevantValue = { positionsArrayFromSinglesDict[0],
                                                                              positionsArrayFromSinglesDict[1], i_Row, i_Column };
                        m_SingleSlotFoundDictionary.Remove(i_Value);
                        m_MatchedSlotsFoundDictionary.Add(i_Value, positionsOfBothSlotsContainingRelevantValue);
                    }
                }
                else
                {
                    int[] positions = { i_Row, i_Column };
                    m_SingleSlotFoundDictionary.Add(i_Value, positions);
                }
            }
        }

        public bool IsContainInSingleSlotsDictionary(char i_Value)
        {
            return m_SingleSlotFoundDictionary.ContainsKey(i_Value);
        }

        public int[] GetPositionOfSingleSlot(char i_Value)
        {
            int[] positionsArrayFromDict;

            bool isSucceeded = m_SingleSlotFoundDictionary.TryGetValue(i_Value, out positionsArrayFromDict);
            if (!isSucceeded)
            {
                positionsArrayFromDict = new int[0];
            }
            
            return positionsArrayFromDict;
        }

        /// <summary>
        /// Checks for a couple of known matching slots.
        /// If the PC knows of a couple - return their positions.
        /// Else return an empty array.
        /// </summary>
        public int[] GetPositionOfTwoRandomSlots()
        {
            List<char> listOfKeys = new List<char>(m_MatchedSlotsFoundDictionary.Keys);
            int[] positionsArrayFromDict = new int[0];

            if (listOfKeys.Count > 0)
            {
                char key = listOfKeys[0];
                m_MatchedSlotsFoundDictionary.TryGetValue(key, out positionsArrayFromDict);
                m_MatchedSlotsFoundDictionary.Remove(key);
            }

            return positionsArrayFromDict;
        }

        public void RemoveMatchedSlots(char i_Value)
        {
            m_MatchedSlotsFoundDictionary.Remove(i_Value);
        }
    }
}
