﻿using System;
using System.Collections.Generic;
using System.Linq;
using HDF5CSharp.Interfaces;

namespace HDF5CSharp
{
   public class Hdf5ReaderWriter
    {
        IHdf5ReaderWriter rw;
        public Hdf5ReaderWriter(IHdf5ReaderWriter _rw)
        {
            rw = _rw;
        }

        public (int success, long CreatedgroupId) WriteArray(long groupId, string name, Array collection, string datasetName, Dictionary<string, List<string>> attributes)
        {

            Type type = collection.GetType();
            Type elementType = type.GetElementType();
            TypeCode typeCode = Type.GetTypeCode(elementType);
            //Boolean isStruct = type.IsValueType && !type.IsEnum;
            (int success, long CreatedgroupId) result;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    var bls = collection.ConvertArray<bool, ushort>(Convert.ToUInt16);
                    result = rw.WriteFromArray<ushort>(groupId, name, bls, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "Boolean", name);
                    break;
                case TypeCode.Byte:
                    result = rw.WriteFromArray<byte>(groupId, name, collection, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "Byte", name);
                    break;
                case TypeCode.Char:
                    var chrs = collection.ConvertArray<char, ushort>(Convert.ToUInt16);
                    result = rw.WriteFromArray<ushort>(groupId, name, chrs, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "Char", name);
                    break;

                case TypeCode.DateTime:
                    var dts = collection.ConvertArray<DateTime, long>(dt => Hdf5Conversions.FromDatetime(dt, Hdf5.Hdf5Settings.DateTimeType));
                    result = rw.WriteFromArray<long>(groupId, name, dts, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "DateTime", name);
                    break;
                case TypeCode.Decimal:
                    var decs = collection.ConvertArray<decimal, double>(Convert.ToDouble);
                    result = rw.WriteFromArray<double>(groupId, name, decs, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "Decimal", name);
                    break;

                case TypeCode.Double:
                    result = rw.WriteFromArray<double>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.Int16:
                    result = rw.WriteFromArray<short>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.Int32:
                    result = rw.WriteFromArray<int>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.Int64:
                    result = rw.WriteFromArray<long>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.SByte:
                    result = rw.WriteFromArray<sbyte>(groupId, name, collection, datasetName);
                    Hdf5.WriteStringAttribute(groupId, name, "SByte", name);
                    break;

                case TypeCode.Single:
                    result = rw.WriteFromArray<float>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.UInt16:
                    result = rw.WriteFromArray<ushort>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.UInt32:
                    result = rw.WriteFromArray<uint>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.UInt64:
                    result = rw.WriteFromArray<ulong>(groupId, name, collection, datasetName);
                    break;

                case TypeCode.String:
                    if (collection.Rank > 1 && collection.GetLength(1) > 1)
                        throw new Exception("Only 1 dimensional string arrays allowed: " + name);
                    result = rw.WriteStrings(groupId, name, (string[])collection, datasetName);
                    break;

                default:
                    if (elementType == typeof(TimeSpan))
                    {
                        var tss = collection.ConvertArray<TimeSpan, long>(dt => dt.Ticks);
                        result = rw.WriteFromArray<long>(groupId, name, tss, datasetName);
                        Hdf5.WriteStringAttribute(groupId, name, "TimeSpan", name);

                    }
                    //else if (isStruct) {
                    //    rw.WriteStucts(groupId, name, collection);
                    //}
                    else
                    {
                        string str = "type is not supported: ";
                        throw new NotSupportedException(str + elementType.FullName);
                    }
                    break;
            }
            //append attributes
            foreach (KeyValuePair<string, List<string>> entry in attributes)
            {
                Hdf5.WriteStringAttribute(groupId, entry.Key, string.Join("',", entry.Value), name);
            }

            return result;
        }


        public (bool success, Array result) ReadArray<T>(long groupId, string name, string alternativeName)
        {
            return ReadArray(typeof(T), groupId, name, alternativeName);
        }

        public (bool success, Array result) ReadArray(Type elementType, long groupId, string name, string alternativeName)
        {
            TypeCode ty = Type.GetTypeCode(elementType);
            bool success;
            Array result;
            switch (ty)
            {
                case TypeCode.Boolean:
                    (success, result) = rw.ReadToArray<ushort>(groupId, name, alternativeName);
                    return(success, result.ConvertArray<ushort, bool>(Convert.ToBoolean));

                case TypeCode.Byte:
                    return rw.ReadToArray<byte>(groupId, name, alternativeName);

                case TypeCode.Char:
                    (success, result) = rw.ReadToArray<ushort>(groupId, name, alternativeName);
                    return (success, result.ConvertArray<ushort, char>(Convert.ToChar));

                case TypeCode.DateTime:
                    (success, result) = rw.ReadToArray<long>(groupId, name, alternativeName);
                    return (success, result.ConvertArray<long, DateTime>(tc => Hdf5Conversions.ToDateTime(tc, Hdf5.Hdf5Settings.DateTimeType)));

                case TypeCode.Decimal:
                    (success, result) = rw.ReadToArray<double>(groupId, name, alternativeName);
                    return (success,result.ConvertArray<double, decimal>(Convert.ToDecimal));

                case TypeCode.Double:
                    return rw.ReadToArray<double>(groupId, name, alternativeName);

                case TypeCode.Int16:
                    return rw.ReadToArray<short>(groupId, name, alternativeName);

                case TypeCode.Int32:
                    return rw.ReadToArray<int>(groupId, name, alternativeName);

                case TypeCode.Int64:
                    return rw.ReadToArray<long>(groupId, name, alternativeName);

                case TypeCode.SByte:
                    return rw.ReadToArray<sbyte>(groupId, name, alternativeName);

                case TypeCode.Single:
                    return rw.ReadToArray<float>(groupId, name, alternativeName);

                case TypeCode.UInt16:
                    return rw.ReadToArray<ushort>(groupId, name, alternativeName);

                case TypeCode.UInt32:
                    return rw.ReadToArray<uint>(groupId, name, alternativeName);

                case TypeCode.UInt64:
                    return rw.ReadToArray<ulong>(groupId, name, alternativeName);

                case TypeCode.String:
                    var (valid, strings) = rw.ReadStrings(groupId, name, alternativeName);
                    return (valid, strings.ToArray());

                default:
                    if (elementType == typeof(TimeSpan))
                    {
                        (success, result) =  rw.ReadToArray<long>(groupId, name, alternativeName);
                        return (success, result.ConvertArray<long, TimeSpan>(tcks => new TimeSpan(tcks)));
                    }
                    string str = "type is not supported: ";
                    Hdf5Utils.LogError?.Invoke($"Error: {str}");
                    throw new NotSupportedException(str + elementType.FullName);

            }
        }

    }
}