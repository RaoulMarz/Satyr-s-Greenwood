using System;

namespace Satyrs_Greenwood
{
    struct FlagDateTime
    {
        public DateTime timestamp;
        public bool flag;

        public FlagDateTime(DateTime time1, bool flag1)
        {
            timestamp = time1;
            flag = flag1;
        }
    }
}