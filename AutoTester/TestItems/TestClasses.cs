using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConverter.Attributs;


namespace AutoTester.TestItems
{
    class StandardClass
    {
        public int IntProp { get; set; }
        public byte ByteProp { get; private set; }
        public short ShortProp { get; }

        public int IntField;

        public StandardChildClass Child { get; set; }

        public StandardClass()
        {
        }

    }
    class StandardChildClass
    {

        public int X { get; set; }

    }

    class ConstructerTestClass1
    {
        public int X { get; set; }
        public int Y { get;}

        public byte Z { get; private set; }

        public bool CtorUsed { get; }



        public ConstructerTestClass1()
        {

        }

        [OptionalConstructor("YVal", Priority = 1)]
        public ConstructerTestClass1(int y)
        {
            Y = y;
            CtorUsed = true;
        }


        [OptionalConstructor("ZVal", Priority = 1)]
        public ConstructerTestClass1(byte z)
        {
            Z = z;
            CtorUsed = true;
        }
    }



    class ConstructerTestClass2
    {
        public int X { get; set; }
        public int Y { get; }

        public byte Z { get; private set; }

        public bool CtorUsed { get; }



        [OptionalConstructor("YVal", Priority = 1)]
        public ConstructerTestClass2(int y)
        {
            Y = y;
            CtorUsed = true;
        }


        [OptionalConstructor("ZVal", Priority = 1)]
        public ConstructerTestClass2(byte z)
        {
            Z = z;
            CtorUsed = true;
        }
    }

}
