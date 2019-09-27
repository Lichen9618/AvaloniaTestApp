using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.CrossChain
{
    public class VMString: ByteArray
    {

        private string _textValue;
        public VMString(byte[] value):base(value)
        {
            _textValue = Encoding.UTF8.GetString(value);
        }


        public VMString(string text) : base(Encoding.UTF8.GetBytes(text))
        {
            _textValue = text;
        }

    }
}
