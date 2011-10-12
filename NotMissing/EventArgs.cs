using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Generic event handler delegate
    /// </summary>
    /// <typeparam name="T">EventArg Type</typeparam>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    public delegate void EventHandler<T>(object sender, T e) where T : EventArgs;
}
