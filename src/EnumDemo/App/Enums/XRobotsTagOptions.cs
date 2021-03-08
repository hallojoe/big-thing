using System;
using System.Collections.Generic;

namespace App.Enums 
{
  [Flags]
  public enum XRobotsTagOptions 
  {
    Default    =  0, // Good enum practice says we should name this(0) None, but for this example we use None elsewhere
    NoIndex    =  1, 
    NoFollow   =  2,
    None       =  3, // combine
    NoArchive  =  4,
    NoSnippet  =  8,
    All        = 15, // combine
  }

  public class Page 
  {
    public string Title { get; set; }
    public XRobotsTagOptions Meta { get; set; }
  }

  public static class Data 
  {
    public static IList<Page> Pages { get; set; } = new List<Page>()
    {
      new Page { Title = "Public Page" },
      new Page { Title = "Private Page", Meta = XRobotsTagOptions.NoIndex | XRobotsTagOptions.NoFollow},
      new Page { Title = "Extra Private Page", Meta = XRobotsTagOptions.All },
      new Page { Title = "No Cache Page", Meta = XRobotsTagOptions.NoArchive },
      new Page { Title = "Narcissist Page", Meta = XRobotsTagOptions.NoFollow },
      new Page { Title = "Introvert Narcissist Page", Meta = XRobotsTagOptions.None },
      new Page { Title = "No Cache", Meta = XRobotsTagOptions.NoArchive | XRobotsTagOptions.NoSnippet }
    };
  }

}