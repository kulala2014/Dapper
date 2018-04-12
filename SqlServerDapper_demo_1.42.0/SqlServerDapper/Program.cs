using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlServerDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new AdventureWorksTests().RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("------------------------------------------------------------");
                while (ex != null)
                {
                    Console.WriteLine(string.Format("{0}\n{1}\n------------------------------------------------------------", ex.GetType().ToString(), ex.Message));
                    ex = ex.InnerException;
                }
            }
            finally
            {
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
