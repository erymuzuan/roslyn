// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.Reflection.Metadata;

namespace Microsoft.Cci
{
    /// <summary>
    /// A region representing an exception handler clause. The region exposes the type (catch or
    /// finally) and the bounds of the try block and catch or finally block as needed by 
    /// </summary>
    internal abstract class ExceptionHandlerRegion
    {
        private readonly uint _tryStartOffset;
        private readonly uint _tryEndOffset;
        private readonly uint _handlerStartOffset;
        private readonly uint _handlerEndOffset;

        public ExceptionHandlerRegion(
            uint tryStartOffset,
            uint tryEndOffset,
            uint handlerStartOffset,
            uint handlerEndOffset)
        {
            Debug.Assert(tryStartOffset < tryEndOffset);
            Debug.Assert(tryEndOffset <= handlerStartOffset);
            Debug.Assert(handlerStartOffset < handlerEndOffset);

            _tryStartOffset = tryStartOffset;
            _tryEndOffset = tryEndOffset;
            _handlerStartOffset = handlerStartOffset;
            _handlerEndOffset = handlerEndOffset;
        }

        /// <summary>
        /// Handler kind for this SEH info
        /// </summary>
        public abstract ExceptionRegionKind HandlerKind { get; }

        /// <summary>
        /// If HandlerKind == HandlerKind.Catch, this is the type of expection to catch. If HandlerKind == HandlerKind.Filter, this is System.Object.
        /// Otherwise this is a Dummy.TypeReference.
        /// </summary>
        public virtual ITypeReference ExceptionType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Label instruction corresponding to the start of filter decision block
        /// </summary>
        public virtual uint FilterDecisionStartOffset
        {
            get { return 0; }
        }

        /// <summary>
        /// Label instruction corresponding to the start of try block
        /// </summary>
        public uint TryStartOffset
        {
            get { return _tryStartOffset; }
        }

        /// <summary>
        /// Label instruction corresponding to the end of try block
        /// </summary>
        public uint TryEndOffset
        {
            get { return _tryEndOffset; }
        }

        /// <summary>
        /// Label instruction corresponding to the start of handler block
        /// </summary>
        public uint HandlerStartOffset
        {
            get { return _handlerStartOffset; }
        }

        /// <summary>
        /// Label instruction corresponding to the end of handler block
        /// </summary>
        public uint HandlerEndOffset
        {
            get { return _handlerEndOffset; }
        }
    }

    internal sealed class ExceptionHandlerRegionFinally : ExceptionHandlerRegion
    {
        public ExceptionHandlerRegionFinally(
            uint tryStartOffset,
            uint tryEndOffset,
            uint handlerStartOffset,
            uint handlerEndOffset)
            : base(tryStartOffset, tryEndOffset, handlerStartOffset, handlerEndOffset)
        {
        }

        public override ExceptionRegionKind HandlerKind
        {
            get { return ExceptionRegionKind.Finally; }
        }
    }

    internal sealed class ExceptionHandlerRegionFault : ExceptionHandlerRegion
    {
        public ExceptionHandlerRegionFault(
            uint tryStartOffset,
            uint tryEndOffset,
            uint handlerStartOffset,
            uint handlerEndOffset)
            : base(tryStartOffset, tryEndOffset, handlerStartOffset, handlerEndOffset)
        {
        }

        public override ExceptionRegionKind HandlerKind
        {
            get { return ExceptionRegionKind.Fault; }
        }
    }

    internal sealed class ExceptionHandlerRegionCatch : ExceptionHandlerRegion
    {
        private readonly ITypeReference _exceptionType;

        public ExceptionHandlerRegionCatch(
            uint tryStartOffset,
            uint tryEndOffset,
            uint handlerStartOffset,
            uint handlerEndOffset,
            ITypeReference exceptionType)
            : base(tryStartOffset, tryEndOffset, handlerStartOffset, handlerEndOffset)
        {
            _exceptionType = exceptionType;
        }

        public override ExceptionRegionKind HandlerKind
        {
            get { return ExceptionRegionKind.Catch; }
        }

        public override ITypeReference ExceptionType
        {
            get { return _exceptionType; }
        }
    }

    internal sealed class ExceptionHandlerRegionFilter : ExceptionHandlerRegion
    {
        private readonly uint _filterDecisionStartOffset;

        public ExceptionHandlerRegionFilter(
            uint tryStartOffset,
            uint tryEndOffset,
            uint handlerStartOffset,
            uint handlerEndOffset,
            uint filterDecisionStartOffset)
            : base(tryStartOffset, tryEndOffset, handlerStartOffset, handlerEndOffset)
        {
            _filterDecisionStartOffset = filterDecisionStartOffset;
        }

        public override ExceptionRegionKind HandlerKind
        {
            get { return ExceptionRegionKind.Filter; }
        }

        public override uint FilterDecisionStartOffset
        {
            get { return _filterDecisionStartOffset; }
        }
    }
}
