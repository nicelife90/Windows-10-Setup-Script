using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Windows10SetupScript.Classes
{
    class ErrorsHelper
    {
        public bool HasErrors { get; private set; } = false;
        
        public Action FixErrors { get; private set;}

        private void WatchDog(Test result)
        {
            if (HasErrors != result.Result)
            {
                HasErrors = result.Result;

                if (result.Description == CONST.Error_OsVersionNotSupported)
                {
                    FixErrors = () => FixOsVersion();
                }
            };
        }

        private void FixOsVersion()
        {
            throw new NotImplementedException();
        }

        public ErrorsHelper()
        {
            
            Queue<Func<Test>> funcQueue = new Queue<Func<Test>>();
            funcQueue.Enqueue(() => TestOsVersion());



            for (int i = 0; i < funcQueue.Count; i++)
            {
                WatchDog(funcQueue.Dequeue().Invoke());

                if (HasErrors)
                    break;
            }
        }        

        private Test TestOsVersion()
        {
            return new Test()
            {
                Result = Convert.ToInt32(Environment.OSVersion.Version.Build) >= Convert.ToInt32(CONST.Win10_Build)
                             && Convert.ToInt32(Environment.OSVersion.Version.Major) == Convert.ToInt32(CONST.Win10_Major)
                             ? false : true,

                Description = CONST.Error_OsVersionNotSupported
            };            
        }        

        
    }
}


