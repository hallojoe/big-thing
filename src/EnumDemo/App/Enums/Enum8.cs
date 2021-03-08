using System;

namespace App.Enums 
{
  [Flags]
  enum Enum8: byte {
    None  = 0, 
    One   = 1, 
    Two   = 2, 
    Three = 4, 
    Four  = 8, 
    Five  = 16, 
    Six   = 32, 
    Seven = 64, 
    Eight = 128,  
    All   = 255
  }
}
