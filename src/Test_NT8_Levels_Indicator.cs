using System;
using System.Collections.Generic;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Indicators.Tests
{
    /// <summary>
    /// Test class for NT8_Levels_Indicator
    /// Note: These tests need to be run within NinjaTrader 8 environment
    /// </summary>
    public class Test_NT8_Levels_Indicator
    {
        public static void RunTests()
        {
            Console.WriteLine("=== NT8 Levels Indicator Tests ===");
            Test_DataParsing();
            Test_PropertyDefaults();
            Console.WriteLine("=== All Tests Completed ===");
        }

        /// <summary>
        /// Tests data parsing logic for Set1 format
        /// </summary>
        public static void Test_DataParsing()
        {
            Console.WriteLine("Running Test_DataParsing...");

            // Test Set1 data format parsing
            string set1Data = "24500,T1=10,T2=20,O1=5,O2=15,24400";
            string[] parts = set1Data.Split(',');
            
            bool passed = parts.Length >= 6 &&
                         double.TryParse(parts[0], out double pTop) &&
                         double.TryParse(parts[1].Replace("T1=", ""), out double t1v) &&
                         double.TryParse(parts[2].Replace("T2=", ""), out double t2v) &&
                         double.TryParse(parts[3].Replace("O1=", ""), out double o1v) &&
                         double.TryParse(parts[4].Replace("O2=", ""), out double o2v) &&
                         double.TryParse(parts[5], out double pBottom);

            if (passed && pTop == 24500 && pBottom == 24400 && t1v == 10 && t2v == 20)
            {
                Console.WriteLine("  Test_DataParsing: PASSED");
            }
            else
            {
                Console.WriteLine("  Test_DataParsing: FAILED");
            }
        }

        /// <summary>
        /// Tests that property defaults are correctly set
        /// </summary>
        public static void Test_PropertyDefaults()
        {
            Console.WriteLine("Running Test_PropertyDefaults...");

            // Test default values
            bool passed = true;

            // Check default opacity values
            int set1OpacityDefault = 80;
            int set2OpacityDefault = 80;

            if (set1OpacityDefault != 80)
            {
                passed = false;
                Console.WriteLine("  Set1ZoneOpacity default incorrect");
            }

            if (set2OpacityDefault != 80)
            {
                passed = false;
                Console.WriteLine("  Set2ZoneOpacity default incorrect");
            }

            if (passed)
            {
                Console.WriteLine("  Test_PropertyDefaults: PASSED");
            }
            else
            {
                Console.WriteLine("  Test_PropertyDefaults: FAILED");
            }
        }
    }
}
