namespace ZeroUndubProcess
{
    public static class PssConstants
    {
          public static readonly byte[]  AudioSegment = new byte[4] {0x00, 0x00, 0x01, 0xBD};
          public static readonly byte[]  PackStart = new byte[4] {0x00, 0x00, 0x01, 0xBA};
          public static readonly byte[] EndFile = new byte[4] {0x00, 0x00, 0x01, 0xB9};
          public const int FirstHeaderSize = 0x3F;
          public const int HeaderSize = 0x17;
          
          public const string PssIsoData = @"[
      {
            ""EuSizeOffset"": 541976,
            ""Filename"": ""/MOVIE/DUMMY.PSS;1"",
            ""EuOffset"": 2565527552,
            ""EuSize"": 4538372,
            ""JpOffset"": 2164469760,
            ""JpSize"": 4538372
      },
      {
            ""EuSizeOffset"": 541378,
            ""Filename"": ""/MOVIE/SCN0010.PSS;1"",
            ""EuOffset"": 2361264128,
            ""EuSize"": 78692356,
            ""JpOffset"": 1694879744,
            ""JpSize"": 78659588
      },
      {
            ""EuSizeOffset"": 540898,
            ""Filename"": ""/MOVIE/SCN0031.PSS;1"",
            ""EuOffset"": 2225162240,
            ""EuSize"": 8880132,
            ""JpOffset"": 2075656192,
            ""JpSize"": 8880132
      },
      {
            ""EuSizeOffset"": 541018,
            ""Filename"": ""/MOVIE/SCN1000.PSS;1"",
            ""EuOffset"": 2244990976,
            ""EuSize"": 26411012,
            ""JpOffset"": 2084538368,
            ""JpSize"": 26476548
      },
      {
            ""EuSizeOffset"": 540958,
            ""Filename"": ""/MOVIE/SCN1031.PSS;1"",
            ""EuOffset"": 2234044416,
            ""EuSize"": 10944516,
            ""JpOffset"": 2136907776,
            ""JpSize"": 10944516
      },
      {
            ""EuSizeOffset"": 541138,
            ""Filename"": ""/MOVIE/SCN1101.PSS;1"",
            ""EuOffset"": 2294900736,
            ""EuSize"": 20463620,
            ""JpOffset"": 2052616192,
            ""JpSize"": 20463620
      },
      {
            ""EuSizeOffset"": 541078,
            ""Filename"": ""/MOVIE/SCN1240.PSS;1"",
            ""EuOffset"": 2271404032,
            ""EuSize"": 23494660,
            ""JpOffset"": 2113411072,
            ""JpSize"": 23494660
      },
      {
            ""EuSizeOffset"": 541198,
            ""Filename"": ""/MOVIE/SCN1300.PSS;1"",
            ""EuOffset"": 2315366400,
            ""EuSize"": 5799940,
            ""JpOffset"": 1985667072,
            ""JpSize"": 6914052
      },
      {
            ""EuSizeOffset"": 541258,
            ""Filename"": ""/MOVIE/SCN1331.PSS;1"",
            ""EuOffset"": 2321168384,
            ""EuSize"": 2392068,
            ""JpOffset"": 2111016960,
            ""JpSize"": 2392068
      },
      {
            ""EuSizeOffset"": 541318,
            ""Filename"": ""/MOVIE/SCN1332.PSS;1"",
            ""EuOffset"": 2323562496,
            ""EuSize"": 37699588,
            ""JpOffset"": 1922289664,
            ""JpSize"": 55623684
      },
      {
            ""EuSizeOffset"": 540778,
            ""Filename"": ""/MOVIE/SCN2010.PSS;1"",
            ""EuOffset"": 2191587328,
            ""EuSize"": 29753348,
            ""JpOffset"": 1824159744,
            ""JpSize"": 39976964
      },
      {
            ""EuSizeOffset"": 541438,
            ""Filename"": ""/MOVIE/SCN2050.PSS;1"",
            ""EuOffset"": 2439958528,
            ""EuSize"": 16400388,
            ""JpOffset"": 2147854336,
            ""JpSize"": 16613380
      },
      {
            ""EuSizeOffset"": 541498,
            ""Filename"": ""/MOVIE/SCN2061.PSS;1"",
            ""EuOffset"": 2456360960,
            ""EuSize"": 19333124,
            ""JpOffset"": 1898186752,
            ""JpSize"": 24100868
      },
      {
            ""EuSizeOffset"": 541616,
            ""Filename"": ""/MOVIE/SCN2071.PSS;1"",
            ""EuOffset"": 2478270464,
            ""EuSize"": 7225348,
            ""JpOffset"": 1977915392,
            ""JpSize"": 7749636
      },
      {
            ""EuSizeOffset"": 541676,
            ""Filename"": ""/MOVIE/SCN2091.PSS;1"",
            ""EuOffset"": 2485497856,
            ""EuSize"": 22642692,
            ""JpOffset"": 1798221824,
            ""JpSize"": 25935876
      },
      {
            ""EuSizeOffset"": 541736,
            ""Filename"": ""/MOVIE/SCN2110.PSS;1"",
            ""EuOffset"": 2508142592,
            ""EuSize"": 24051716,
            ""JpOffset"": 1864138752,
            ""JpSize"": 34045956
      },
      {
            ""EuSizeOffset"": 541796,
            ""Filename"": ""/MOVIE/SCN2131.PSS;1"",
            ""EuOffset"": 2532196352,
            ""EuSize"": 11730948,
            ""JpOffset"": 1782702080,
            ""JpSize"": 11698180
      },
      {
            ""EuSizeOffset"": 541856,
            ""Filename"": ""/MOVIE/SCN2142.PSS;1"",
            ""EuOffset"": 2543929344,
            ""EuSize"": 11747332,
            ""JpOffset"": 2169010176,
            ""JpSize"": 13467652
      },
      {
            ""EuSizeOffset"": 540838,
            ""Filename"": ""/MOVIE/SCN2143.PSS;1"",
            ""EuOffset"": 2221342720,
            ""EuSize"": 3817476,
            ""JpOffset"": 1794402304,
            ""JpSize"": 3817476
      },
      {
            ""EuSizeOffset"": 541916,
            ""Filename"": ""/MOVIE/SCN2171.PSS;1"",
            ""EuOffset"": 2555678720,
            ""EuSize"": 9846788,
            ""JpOffset"": 2182479872,
            ""JpSize"": 11452420
      },
      {
            ""EuSizeOffset"": 542034,
            ""Filename"": ""/MOVIE/SCN9000.PSS;1"",
            ""EuOffset"": 2570067968,
            ""EuSize"": 6914052,
            ""JpOffset"": 1773541376,
            ""JpSize"": 9158660
      },
      {
            ""EuSizeOffset"": 536744,
            ""Filename"": ""/MOVIE/SCN9001.PSS;1"",
            ""EuOffset"": 2576984064,
            ""EuSize"": 45285380,
            ""JpOffset"": 1992583168,
            ""JpSize"": 60030980
      },
      {
            ""EuSizeOffset"": 542154,
            ""Filename"": ""/MOVIE/SCN9100.PSS;1"",
            ""EuOffset"": 2622271488,
            ""EuSize"": 54263812,
            ""JpOffset"": 2193934336,
            ""JpSize"": 54214660
      },
      {
            ""EuSizeOffset"": 542214,
            ""Filename"": ""/MOVIE/SCN9200.PSS;1"",
            ""EuOffset"": 2676537344,
            ""EuSize"": 18333700,
            ""JpOffset"": 2248151040,
            ""JpSize"": 18333700
      },
      {
            ""EuSizeOffset"": 538108,
            ""Filename"": ""/MOVIE/TECMO.PSS;1"",
            ""EuOffset"": 2475696128,
            ""EuSize"": 2572292,
            ""JpOffset"": 2073081856,
            ""JpSize"": 2572292
      },
      {
            ""EuSizeOffset"": 542826,
            ""Filename"": ""/MOVIE2/SCN3010.PSS;1"",
            ""EuOffset"": 2694873088,
            ""EuSize"": 20332548,
            ""JpOffset"": 2266486784,
            ""JpSize"": 20283396
      },
      {
            ""EuSizeOffset"": 543726,
            ""Filename"": ""/MOVIE2/SCN3040.PSS;1"",
            ""EuOffset"": 3232528384,
            ""EuSize"": 12402692,
            ""JpOffset"": 2814644224,
            ""JpSize"": 13713412
      },
      {
            ""EuSizeOffset"": 543606,
            ""Filename"": ""/MOVIE2/SCN3080.PSS;1"",
            ""EuOffset"": 3202426880,
            ""EuSize"": 24576004,
            ""JpOffset"": 2784788480,
            ""JpSize"": 24330244
      },
      {
            ""EuSizeOffset"": 543666,
            ""Filename"": ""/MOVIE2/SCN3081.PSS;1"",
            ""EuOffset"": 3227004928,
            ""EuSize"": 5521412,
            ""JpOffset"": 2809120768,
            ""JpSize"": 5521412
      },
      {
            ""EuSizeOffset"": 543486,
            ""Filename"": ""/MOVIE2/SCN3090.PSS;1"",
            ""EuOffset"": 3192920064,
            ""EuSize"": 5636100,
            ""JpOffset"": 2775281664,
            ""JpSize"": 5636100
      },
      {
            ""EuSizeOffset"": 543546,
            ""Filename"": ""/MOVIE2/SCN4010.PSS;1"",
            ""EuOffset"": 3198558208,
            ""EuSize"": 3866628,
            ""JpOffset"": 2780919808,
            ""JpSize"": 3866628
      },
      {
            ""EuSizeOffset"": 543426,
            ""Filename"": ""/MOVIE2/SCN4031.PSS;1"",
            ""EuOffset"": 3189706752,
            ""EuSize"": 3211268,
            ""JpOffset"": 2772068352,
            ""JpSize"": 3211268
      },
      {
            ""EuSizeOffset"": 543306,
            ""Filename"": ""/MOVIE2/SCN4041.PSS;1"",
            ""EuOffset"": 3164684288,
            ""EuSize"": 6389764,
            ""JpOffset"": 2747045888,
            ""JpSize"": 6389764
      },
      {
            ""EuSizeOffset"": 543366,
            ""Filename"": ""/MOVIE2/SCN4060.PSS;1"",
            ""EuOffset"": 3171076096,
            ""EuSize"": 18628612,
            ""JpOffset"": 2753437696,
            ""JpSize"": 18628612
      },
      {
            ""EuSizeOffset"": 543246,
            ""Filename"": ""/MOVIE2/SCN4080.PSS;1"",
            ""EuOffset"": 3153180672,
            ""EuSize"": 11501572,
            ""JpOffset"": 2730741760,
            ""JpSize"": 16302084
      },
      {
            ""EuSizeOffset"": 543186,
            ""Filename"": ""/MOVIE2/SCN4090.PSS;1"",
            ""EuOffset"": 3126947840,
            ""EuSize"": 26230788,
            ""JpOffset"": 2702542848,
            ""JpSize"": 28196868
      },
      {
            ""EuSizeOffset"": 543126,
            ""Filename"": ""/MOVIE2/SCN4100.PSS;1"",
            ""EuOffset"": 3085117440,
            ""EuSize"": 41828356,
            ""JpOffset"": 2660712448,
            ""JpSize"": 41828356
      },
      {
            ""EuSizeOffset"": 543066,
            ""Filename"": ""/MOVIE2/SCN4110.PSS;1"",
            ""EuOffset"": 3050250240,
            ""EuSize"": 34865156,
            ""JpOffset"": 2625845248,
            ""JpSize"": 34865156
      },
      {
            ""EuSizeOffset"": 543006,
            ""Filename"": ""/MOVIE2/SCN4120.PSS;1"",
            ""EuOffset"": 3033831424,
            ""EuSize"": 16416772,
            ""JpOffset"": 2604838912,
            ""JpSize"": 21004292
      },
      {
            ""EuSizeOffset"": 542886,
            ""Filename"": ""/MOVIE2/SCN5010.PSS;1"",
            ""EuOffset"": 2715207680,
            ""EuSize"": 166494212,
            ""JpOffset"": 2437982208,
            ""JpSize"": 166854660
      },
      {
            ""EuSizeOffset"": 542946,
            ""Filename"": ""/MOVIE2/SCN5020.PSS;1"",
            ""EuOffset"": 2881703936,
            ""EuSize"": 152125444,
            ""JpOffset"": 2286772224,
            ""JpSize"": 151207940
      }
    ]";
    }
}