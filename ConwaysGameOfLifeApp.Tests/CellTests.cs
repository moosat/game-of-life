//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ConwaysGameOfLifeApp.Tests
//{
//    [TestClass]
//    public class CellTests
//    {
//        [TestMethod]
//        public void Get_NumOf_Neighobur()
//        {
//            var mat = new bool[,]
//            {
//                {false, false, false },
//                {false, false, false },
//                {false, false, false },
//            };
//          var board = new Board(mat);
//          int x = board.GetNumOfLivingNeigbours(0, 0);
//          Assert.IsTrue(x == 0);
//        }

//        [TestMethod]
//        public void Get_NumOf_Neighobur1()
//        {
//            var mat = new bool[,]
//            {
//                {false, true, false },
//                {false, false, false },
//                {false, false, false },
//            };
//            var board = new Board(mat);
//            int x = board.GetNumOfLivingNeigbours(0, 0);
//            Assert.IsTrue(x == 1);
//        }

//        [TestMethod]
//        public void Get_NumOf_Neighobur2()
//        {
//            var mat = new bool[,]
//            {
//                {true, true, true },
//                {false, false, false },
//                {false, false, false },
//            };
//            var board = new Board(mat);
//            int x = board.GetNumOfLivingNeigbours(0, 0);
//            Assert.IsTrue(x == 1);
//        }

//        [TestMethod]
//        public void Get_NumOf_Neighobur3()
//        {
//            var mat = new bool[,]
//            {
//                {true, true, true },
//                {false, false, false },
//                {false, false, false },
//            };
//            var board = new Board(mat);
//            int x = board.GetNumOfLivingNeigbours(1, 1);
//            Assert.IsTrue(x == 3);
//        }
//        [TestMethod]
//        public void CheckNextensssg_Shounle_be_Live()
//        {
          
//        }
//    }
//}
