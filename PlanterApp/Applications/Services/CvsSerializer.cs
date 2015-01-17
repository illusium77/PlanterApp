using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanterApp.Applications.Services
{

    /// <summary>
    /// Credit: http://www.codeproject.com/Articles/566656/CSV-Serializer-for-NET
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CsvSerializer<T> where T : class, new()
    {
        private bool _ignoreEmptyLines = true;
        private bool _ignoreReferenceTypesExceptString = true;
        private string _newlineReplacement = ((char)0x254).ToString();
        private List<PropertyInfo> _properties;
        private string _replacement = ((char)0x255).ToString();
        private string _rowNumberColumnTitle = "RowNumber";
        private char _separator = ',';
        private bool _useEofLiteral = false;
        private bool _useLineNumbers = true;
        private bool _useTextQualifier = false;
 
        public bool IgnoreEmptyLines
        {
            get { return _ignoreEmptyLines; }
            set { _ignoreEmptyLines = value; }
        }
        
        public bool IgnoreReferenceTypesExceptString
        {
            get { return _ignoreReferenceTypesExceptString; }
            set { _ignoreReferenceTypesExceptString = value; }
        }
        
        public string NewlineReplacement
        {
            get { return _newlineReplacement; }
            set { _newlineReplacement = value; }
        }
        
        public string Replacement
        {
            get { return _replacement; }
            set { _replacement = value; }
        }
        
        public string RowNumberColumnTitle
        {
            get { return _rowNumberColumnTitle; }
            set { _rowNumberColumnTitle = value; }
        }
        
        public char Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }
        
        public bool UseEofLiteral
        {
            get { return _useEofLiteral; }
            set { _useEofLiteral = value; }
        }
        
        public bool UseLineNumbers
        {
            get { return _useLineNumbers; }
            set { _useLineNumbers = value; }
        }
        
        public bool UseTextQualifier
        {
            get { return _useTextQualifier; }
            set { _useTextQualifier = value; }
        }
        
        /// <summary>
        /// Csv Serializer
        /// Initialize by selected properties from the type to be de/serialized
        /// </summary>
        public CsvSerializer()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                | BindingFlags.GetProperty | BindingFlags.SetProperty);
            
            var q = properties.AsQueryable();
            
            if (IgnoreReferenceTypesExceptString)
            {
                q = q.Where(a => a.PropertyType.IsValueType
                    || a.PropertyType.Name == "String"
                    || a.GetCustomAttribute<CsvCustomPropertyHeadersAttribute>() != null
                    || a.GetCustomAttribute<CsvCustomPropertyValuesAttribute>() != null);
            }
            
            var r = from a in q
                    where a.GetCustomAttribute<CsvIgnoreAttribute>() == null
                    //orderby a.Name
                    select a;
            
            _properties = r.ToList();
        }
        
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public IList<T> Deserialize(Stream stream, ref List<string> omittedColumns)
        {
            string[] columns;
            string[] rows;
            
            try
            {
                using (var sr = new StreamReader(stream))
                {
                    columns = sr.ReadLine().Split(Separator);
                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }
            
            var data = new List<T>();
            
            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];
                
                if (IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (!IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    throw new InvalidOperationException(string.Format(@"CSV Error: Empty line at line number: {0}", row));
                }
                
                var parts = line.Split(Separator);
                var firstColumnIndex = UseLineNumbers ? 2 : 1;
                
                if (parts.Length == firstColumnIndex && parts[firstColumnIndex - 1] != null && parts[firstColumnIndex - 1] == "EOF")
                {
                    break;
                }
                
                var datum = new T();
                var start = UseLineNumbers ? 1 : 0;
                
                for (int i = start; i < parts.Length; i++)
                {
                    var value = parts[i];
                    var column = columns[i];
                    
                    // continue of deviant RowNumber column condition
                    // this allows for the deserializer to implicitly ignore the RowNumber column
                    if (column.Equals(RowNumberColumnTitle) && !_properties.Any(a => a.Name.Equals(RowNumberColumnTitle)))
                    {
                        continue;
                    }
                    
                    value = value
                        .Replace(Replacement, Separator.ToString())
                        .Replace(NewlineReplacement, Environment.NewLine).Trim();
                    var p = _properties.FirstOrDefault(a => a.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase));
                    
                    /// ignore property csv column, Property not found on targing type
                    if (p == null)
                    {
                        omittedColumns.Add(column);
                        continue;
                    }
                    
                    if (UseTextQualifier)
                    {
                        if (value.IndexOf("\"") == 0)
                            value = value.Substring(1);
                        
                        if (value[value.Length - 1].ToString() == "\"")
                            value = value.Substring(0, value.Length - 1);
                    }
                    
                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);
                    
                    p.SetValue(datum, convertedvalue);
                }
                data.Add(datum);
            }

            return data;
        }
        
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="data">data</param>        
        public void Serialize(Stream stream, IEnumerable<T> data)
        {
            var sb = new StringBuilder();
            var values = new List<string>();

            sb.AppendLine(GetHeader());
            
            var row = 1;
            foreach (var item in data)
            {
                values.Clear();
                
                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }
                
                foreach (var p in _properties)
                {
                    if (p.GetCustomAttribute<CsvCustomPropertyHeadersAttribute>() != null)
                    {
                        continue;
                    }

                    var raw = p.GetValue(item);
                    if (p.GetCustomAttribute<CsvCustomPropertyValuesAttribute>() != null && p.PropertyType.Name == "String[]")
                    {
                        var customValues = raw as string[];
                        if (customValues != null)
                        {
                            foreach (var value in customValues)
                            {
                                values.Add(PolishValue(value));
                            }
                        }
                    }
                    else
                    {
                        values.Add(PolishValue(raw));
                    }

                    //if (p.GetCustomAttribute<CsvCustomPropertyValuesAttribute>() == null)
                    //{
                    //    var raw = p.GetValue(item);
                    //    var value = raw == null ? "" : raw.ToString().Replace(Separator.ToString(), Replacement).Replace(Environment.NewLine, NewlineReplacement);

                    //    if (UseTextQualifier)
                    //    {
                    //        value = string.Format("\"{0}\"", value);
                    //    }

                    //    values.Add(value);
                    //}
                    //else if (p.PropertyType.Name == "String[]")
                    //{
                    //    var customValues = p.GetValue(item) as string[];
                    //    if (customValues != null)
                    //    {
                    //        values.AddRange(customValues);
                    //    }
                    //}
                }
                
                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }
            
            if (UseEofLiteral)
            {
                values.Clear();
                
                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }
                
                values.Add("EOF");
                
                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }
            
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(sb.ToString().Trim());
            }
        }

        private string PolishValue(object rawValue)
        {
            var value = rawValue == null ? "" : rawValue.ToString().Replace(Separator.ToString(), Replacement).Replace(Environment.NewLine, NewlineReplacement);

            if (UseTextQualifier)
            {
                value = string.Format("\"{0}\"", value);
            }

            return value;
        }

        /// <summary>
        /// Get Header
        /// </summary>
        /// <returns></returns>
        private string GetHeader()
        {
            var headers = new List<string>();

            if (UseLineNumbers)
            {
                headers.Add(RowNumberColumnTitle);
            }

            foreach (var p in _properties)
            {
                if (p.GetCustomAttribute<CsvCustomPropertyHeadersAttribute>() != null && p.PropertyType.Name == "String[]")
                {
                    var customHeaders = p.GetValue(typeof(T)) as string[];
                    if (customHeaders != null)
                    {
                        headers.AddRange(customHeaders);
                    }
                }
                else if (p.GetCustomAttribute<CsvCustomPropertyValuesAttribute>() == null)
                {
                    headers.Add(p.Name);
                }
            }

            return string.Join(Separator.ToString(), headers.ToArray());
        }
    }

    public class CsvIgnoreAttribute : Attribute { }
    public class CsvCustomPropertyHeadersAttribute : Attribute { }
    public class CsvCustomPropertyValuesAttribute : Attribute { }
    }
