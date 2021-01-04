using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mail_thomaslinder_at.Logic.Nodes;
using LogicModule.Nodes.TestHelper;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;
using NUnit.Framework;

namespace LogicNodesTest
{
  [TestFixture]
  public class NodeTest
  {
    [Test]
    public void YourTest()
    {
        INodeContext context = TestNodeContext.Create();
        var node = new KHueNodeBinary(context);
        node.IpAddress.Value = "10.18.11.160";
        node.LightId.Value = 1;
        node.Username.Value = "anz2TXNSxyuKRKBbNvqDDozyCk6hLK6b-1WrsDi8";

        for (int i = 0; i < 10; i++)
        {
            node.Input.Value = i % 2 == 0;
            node.Execute();
            Assert.IsEmpty(node.ErrorMessage.Value);
            Debug.WriteLine(node.ErrorMessage.Value);
            Thread.Sleep(2000);
        }





        }
  }
}
