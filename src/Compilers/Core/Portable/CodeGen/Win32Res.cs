// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using Cci = Microsoft.Cci;
using DWORD = System.UInt32;

namespace Microsoft.CodeAnalysis.CodeGen
{
    internal class Win32Resource : Cci.IWin32Resource
    {
        private readonly byte[] _data;
        private readonly DWORD _codePage;
        private readonly DWORD _languageId;
        private readonly int _id;
        private readonly string _name;
        private readonly int _typeId;
        private readonly string _typeName;

        internal Win32Resource(
            byte[] data,
            DWORD codePage,
            DWORD languageId,
            int id,
            string name,
            int typeId,
            string typeName)
        {
            _data = data;
            _codePage = codePage;
            _languageId = languageId;
            _id = id;
            _name = name;
            _typeId = typeId;
            _typeName = typeName;
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public int TypeId
        {
            get { return _typeId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Id
        {
            get { return _id; }
        }

        public DWORD LanguageId
        {
            get { return _languageId; }
        }

        public DWORD CodePage
        {
            get { return _codePage; }
        }

        public IEnumerable<byte> Data
        {
            get { return _data; }
        }
    }
}
