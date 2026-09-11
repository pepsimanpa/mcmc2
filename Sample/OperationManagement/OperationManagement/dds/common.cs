
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from common.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using Rti.Types;
using System.Linq;
using Omg.Types;

public class DateTime :  IEquatable<DateTime>
{
    public short year { get; set; }
    public short month { get; set; }
    public short day { get; set; }
    public short hour { get; set; }

    public DateTime()
    {
    }

    public DateTime(short  year, short  month, short  day, short  hour)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        this.hour = hour;
    }

    public DateTime(DateTime other)
    {
        if (other == null)
        {
            return;
        }

        this.year = other.year;
        this.month = other.month;
        this.day = other.day;
        this.hour = other.hour;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.year);
        hash.Add(this.month);
        hash.Add(this.day);
        hash.Add(this.hour);

        return hash.ToHashCode();
    }

    public bool Equals(DateTime other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.year.Equals(other.year) && 
        this.month.Equals(other.month) && 
        this.day.Equals(other.day) && 
        this.hour.Equals(other.hour);
    }

    public override bool Equals(object obj) => this.Equals(obj as DateTime);

    public override string ToString() => DateTimeSupport.Instance.ToString(this);
}

