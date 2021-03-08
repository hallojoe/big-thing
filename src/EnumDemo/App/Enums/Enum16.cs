using System;

namespace App.Enums 
{
    [Flags]
    public enum Enum16 : ushort
    {
        None        =          0, 
        One         =          1, //                                1
        Two         =          2, //                               10   1U <<  1
        Three       =          4, //                              100   1U <<  2
        Four        =          8, //                             1000   1U <<  3
        Five        =         16, //                            10000   1U <<  4
        Six         =         32, //                           100000   1U <<  5
        Seven       =         64, //                          1000000   1U <<  6
        Eight       =        128, //                         10000000   1U <<  7
        Nine        =        256, //                        100000000   1U <<  8
        Ten         =        512, //                       1000000000   1U <<  9
        Eleven      =       1024, //                      10000000000   1U << 10
        Twelve      =       2048, //                     100000000000   1U << 11
        Thirteen    =       4096, //                    1000000000000   1U << 12
        Fourteen    =       8192, //                   10000000000000   1U << 13
        Fifteen     =      16384, //                  100000000000000   1U << 14
        Sixteen     =      32768, //                 1000000000000000   1U << 15
        All         =      65535, //                 1111111111111111   UInt16/ushort MaxValue.
                                  //                                    Note:
                                  //                                    No Bitwise complement operator use,
                                  //                                    because of operator signature int ~(int a, int b)
                                  //                                    number is converted to the closest containing integral type
    }
}