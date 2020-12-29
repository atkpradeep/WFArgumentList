namespace WorkflowConsoleApplication
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Remoting.Contexts;
    using System.Threading;
    using System.Xml.Schema;

    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvokerExample();

            //Hold output window until user press any key
            Console.Read();
        }

        static void WorkflowInvokerExample()
        {
            //Initialize Workflow Invoker
            WorkflowInvoker invoker = new WorkflowInvoker(new CustomeActivity());

            string userState = "Custom user state";

            //BeginInvoke call for CustomeActivity execution
            IAsyncResult result = invoker.BeginInvoke(new AsyncCallback(WorkflowCompletedCallback), userState);

            // You can inspect the result from the host to determine if the workflow
            // is complete.
            Console.WriteLine($"Execution is completed: {result.IsCompleted}");

            Console.WriteLine("Waiting for the workflow to complete.");

            //Final result: It will return a collection of Arguments
            IDictionary<string, object> outputs = invoker.EndInvoke(result);

            Console.WriteLine($"Out put Argument value for Start {outputs["Start"]} and End {outputs["End"]}");
        }

        //This method called after execution got completed
        static void WorkflowCompletedCallback(IAsyncResult result)
        {
            Console.WriteLine("Workflow complete.");
        }
    }

    internal class CustomeActivity : Activity
    {
        //Out Arguments
        public OutArgument<double> Start { get; set; }
        public OutArgument<int> End { get; set; }

        public CustomeActivity()
        {
            this.Implementation = () => new Sequence
            {
                Activities =
                {
                    new WriteLine
                    {
                        Text = $"CustomeActivity execution started on {DateTime.Now.TimeOfDay}"
                    },
                    //Call another activity 
                    new AnotherCustomeActivity
                    {
                        OutParam1 = new OutArgument<double>(opt => this.Start.Get(opt)),
                        OutParam2 = new OutArgument<int>(opt => this.End.Get(opt))
                    },
                    new WriteLine
                    {
                        Text = $"CustomeActivity execution ended on {DateTime.Now.TimeOfDay}"
                    }
                }
            };
        }

    }

    internal class AnotherCustomeActivity : Activity
    {
        public OutArgument<double> OutParam1 { get; set; }
        public OutArgument<int> OutParam2 { get; set; }

        public AnotherCustomeActivity()
        {
            this.Implementation = () => new Sequence
            {
                Activities =
                {
                    new WriteLine
                    {
                        Text = $"AnotherCustomeActivity execution started on {DateTime.Now.TimeOfDay}"
                    },
                    new Delay
                    {
                        //Delay for 10 second
                        Duration=TimeSpan.FromSeconds(10)
                    },
                    new WriteLine
                    {
                        Text = $"AnotherCustomeActivity execution ended on {DateTime.Now.TimeOfDay}"
                    }
                }
            };
        }
    }
}
