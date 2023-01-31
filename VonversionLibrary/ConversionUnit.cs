using System.Collections.Generic;
using System.Data;

namespace ConversionLibrary
{
    public static class ConversionUnit
    {
        //all units to check
        static readonly Dictionary<string, string> units = new();

        //all conversions
        static readonly Dictionary<string, string> conversions = new();

        static double valueSiWillMultToResult = 1;
        static ConversionUnit()
        {
            // we can add new unit and formulas here

            /*
             * I have an idea here that if we will have more units in future
             * we can use a logic that changing every unit one main unit and 
             * then convert it to the desirable unit 
             * so we will not need to convert every unit to every other unit
             * just will use transit main unit.
             */

            units.Add("length", "meter,foot,inch");
            conversions.Add("meter,foot", "3.28084*x");
            conversions.Add("meter,inch", "39.3701*x");
            conversions.Add("foot,meter", "0.3048*x");
            conversions.Add("foot,inch", "12*x");
            conversions.Add("inch,meter", "0.0254*x");
            conversions.Add("inch,foot", "0.0833333*x");

            units.Add("data", "bit,byte");
            conversions.Add("bit,byte", "0.125*x");
            conversions.Add("byte,bit", "8*x");

            units.Add("temperature", "celsius,fahrenheit");
            conversions.Add("celsius,fahrenheit", "x*9/5+32");
            conversions.Add("fahrenheit,celsius", "(x-32)*5/9");

        }
        public static ConversionResult Convertion(double value, string unitFrom, string unitTo)
        {
            // in result, first is convert value, second is description about conversion
            ConversionResult conversionResult = new();

            // chech if there is any SI prefix change valume accordingly and make unit a simple unit
            SiPrefix siPrefix = GetSiPrefix(unitFrom);
            if (!siPrefix.value.Equals(1)) // if it is not simple unit
            {
                value *= Convert.ToDouble(siPrefix.value);
                unitFrom = unitFrom.Replace(siPrefix.prefix, ""); // delete Si prefix
            }

            // chech if there is any SI prefix accordingly make unit a simple unit
            siPrefix = GetSiPrefix(unitTo);
            if (!siPrefix.value.Equals(1)) // if it is not simple unit
            {
                valueSiWillMultToResult = Convert.ToDouble(siPrefix.value);
                unitTo = unitTo.Replace(siPrefix.prefix, ""); // delete Si prefix
            }

            // if not convertable return -1
            if (!CheckConvertbale(unitFrom, unitTo)) return conversionResult;

            // if not have formula return -1
            string keyOfUnits = unitFrom + "," + unitTo;
            if (!conversions.ContainsKey(keyOfUnits)) { conversionResult.detail = "Do not have formula."; return conversionResult; }

            double resultOfConvertion = Evaluate(conversions[keyOfUnits], value);

            //if unitTo has Si prefix we change result accordingly
            resultOfConvertion *= valueSiWillMultToResult;

            conversionResult.value = resultOfConvertion;
            conversionResult.detail = "Converted, Status code 200.";
            return conversionResult;
        }
        public class ConversionResult
        {
            public double value;
            public string detail;
            public ConversionResult()
            {
                value = -1;
                detail = "Cannot convert";
            }
        }
        static bool CheckConvertbale(string unitFrom, string unitTo)
        {
            foreach (KeyValuePair<string, string> entry in units)
            {
                string[] values = entry.Value.Split(',');
                if ((values.Contains(unitTo) || values.Contains(unitFrom)))
                    return values.Contains(unitTo) && values.Contains(unitFrom);
            }
            return false; // did not find the pair
        }
        static double Evaluate(string expression, double value)
        {
            expression = expression.Replace("x", value.ToString());
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (double)(loDataTable.Rows[0]["Eval"]);
        }
        static SiPrefix GetSiPrefix(string unit)
        {
            Dictionary<string, double> prefix = new Dictionary<string, double>();
            prefix.Add("deca", 10);
            prefix.Add("hecto", 100);
            prefix.Add("kilo", 1000);
            prefix.Add("mega", 1000000);
            prefix.Add("giga", Math.Pow(10, 9));
            prefix.Add("deci", 0.1);
            prefix.Add("centi", 0.01);
            prefix.Add("milli", 0.001);
            prefix.Add("micro", 0.000001);
            prefix.Add("nano", Math.Pow(10, -9));

            SiPrefix siPrefix = new();
            foreach (string key in prefix.Keys)
            {
                if (unit.Contains(key))
                {
                    siPrefix.prefix = key;
                    siPrefix.value = prefix[key];
                    return siPrefix;
                }
            }
            return siPrefix;
        }
        class SiPrefix
        {
            public string prefix { get; set; }
            public double value { get; set; }
            public SiPrefix()
            {
                prefix = "";
                value = 1;
            }
        }
    }
}