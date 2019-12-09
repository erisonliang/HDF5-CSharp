﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hdf5DotnetWrapper
{
    public static partial class Hdf5
    {
        public static Settings Hdf5Settings { get; set; }


        static Hdf5()
        {
            Hdf5Settings = new Settings { DateTimeType = DateTimeType.Ticks };

        }
    }

    public class Settings
    {
        public DateTimeType DateTimeType { get; set; }
    }

    public enum DateTimeType
    {
        Ticks,
        UnixTimeSeconds,
        UnixTimeMilliseconds,
    }
}
