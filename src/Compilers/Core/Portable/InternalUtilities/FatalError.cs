// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;

#if COMPILERCORE
namespace Microsoft.CodeAnalysis
#else
namespace Microsoft.CodeAnalysis.ErrorReporting
#endif
{
    internal static class FatalError
    {
        private static Action<Exception> s_handler;
        private static Exception s_reportedException;
        private static string s_reportedExceptionMessage;

        // Set by the host to a fail fast trigger, 
        // if the host desires to crash the process on a fatal exception.
        // May also just report a non-fatal Watson and continue.
        public static Action<Exception> Handler
        {
            get
            {
                return s_handler;
            }

            set
            {
                if (s_handler != value)
                {
                    Debug.Assert(s_handler == null, "Handler already set");
                    s_handler = value;
                }
            }
        }

        // Same as setting the Handler property except that it avoids the assert.  This is useful in 
        // test code which needs to verify the handler is called in specific cases and will continually
        // overwrite this value.
        public static void OverwriteHandler(Action<Exception> value)
        {
            s_handler = value;
        }

        /// <summary>
        /// Use in an exception filter to report a fatal error. 
        /// Unless the exception is <see cref="OperationCanceledException"/> 
        /// it calls <see cref="Handler"/>. The exception is passed thru (the method returns false).
        /// </summary>
        /// <returns>False to avoid catching the exception.</returns>
        [DebuggerHidden]
        public static bool ReportUnlessCanceled(Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                return false;
            }

            return Report(exception);
        }

        /// <summary>
        /// Use in an exception filter to report a fatal error. 
        /// Unless the exception is <see cref="NotImplementedException"/> 
        /// it calls <see cref="Handler"/>. The exception is passed thru (the method returns false).
        /// </summary>
        /// <returns>False to avoid catching the exception.</returns>
        [DebuggerHidden]
        public static bool ReportUnlessNotImplemented(Exception exception)
        {
            if (exception is NotImplementedException)
            {
                return false;
            }

            return Report(exception);
        }

        /// <summary>
        /// Use in an exception filter to report a fatal error.
        /// Calls <see cref="Handler"/> and passes the exception thru (the method returns false).
        /// </summary>
        /// <returns>False to avoid catching the exception.</returns>
        public static bool Report(Exception exception)
        {
            // hold onto last exception to make investigation easier
            s_reportedException = exception;
            s_reportedExceptionMessage = exception.ToString();

            var localHandler = s_handler;
            if (localHandler != null)
            {
                localHandler(exception);
            }

            return false;
        }
    }
}
