using System;

namespace Ex02
{
    class Slot
    {
        char m_Value;
        bool m_Hidden;

        public Slot(char i_Value)
        {
            this.m_Value = i_Value;
            this.m_Hidden = true;
        }

        public char Value
        {
            get
            {
                return this.m_Value;
            }
        }

        public bool IsHidden
        {
            get
            {
                return this.m_Hidden;
            }
            set
            {
                this.m_Hidden = value;
            }
        }
    }
}
