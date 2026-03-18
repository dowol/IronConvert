using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IronConvert;

internal class Configuration() : IDisposable
{
    private readonly RegistryKey reg = Registry.CurrentUser.CreateSubKey(@"Software\IronConvert");
    
    internal string GetString(string key, string defaultValue = "")
    {
        return reg.GetValue(key, defaultValue).ToString()!;
    }

    internal int GetDWord(string key, int defaultValue = default)
    {
        return (int)reg.GetValue(key, defaultValue);
    }

    internal long GetQWord(string key, long defaultValue = default)
    {
        return (long)reg.GetValue(key, defaultValue);
    }
    internal bool GetBoolean(string key, bool defaultValue = default)
    {
        return GetDWord(key, defaultValue ? 1 : 0) != 0;
    }

    internal byte[] GetBinary(string key, byte[]? defaultValue = default)
    {
        return (byte[])reg.GetValue(key, defaultValue ?? []);
    }

    internal void Set(string key, string value, RegistryValueKind kind = RegistryValueKind.String)
    {
        reg.SetValue(key, value, kind);
        reg.Flush();
    }

    internal void Set(string key, int value)
    {
        reg.SetValue(key, value, RegistryValueKind.DWord);
        reg.Flush();
    }

    internal void Set(string key, long value)
    {
        reg.SetValue(key, value, RegistryValueKind.QWord);
        reg.Flush();
    }

    internal void Set(string key, bool value)
    {
        reg.SetValue(key, value ? 1 : 0, RegistryValueKind.DWord);
        reg.Flush();
    }

    internal void Set(string key, byte[] value)
    {
        reg.SetValue(key, value, RegistryValueKind.Binary);
        reg.Flush();
    }

    public void Dispose()
    {
        reg.Close();
        reg.Dispose();
        GC.SuppressFinalize(this);
    }
}