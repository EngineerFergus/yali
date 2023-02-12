using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAst.Tests
{
    [TestClass]
    public  class FieldContainerTests
    {
        [DataTestMethod]
        [DataRow("int i > I", "int", "i", "I")]
        [DataRow("  int i  > I  ", "int", "i", "I")]
        public void ParsesCorrectly(string source, string fieldType, string paramName, string memberName)
        {
            var parsed = FieldContainer.Parse(source);
            Assert.AreEqual(paramName, parsed.ParamName, "ParamName");
            Assert.AreEqual(memberName, parsed.MemberName, "MemberName");
            Assert.AreEqual(fieldType, parsed.FieldType, "FieldType");
        }
    }
}
