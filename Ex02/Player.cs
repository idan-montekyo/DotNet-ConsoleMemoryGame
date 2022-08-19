using System;

namespace Ex02
{
    struct Player
    {
        private string m_Name;
        private byte m_Score;

        public Player(string i_Name)
        {
            this.m_Score = 0;
            this.m_Name = i_Name;
        }

        public string Name
        {
            get
            { 
                return this.m_Name; 
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
    }
}
