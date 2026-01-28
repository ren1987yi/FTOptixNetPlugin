using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    internal class CycleTimeTask
    {
        CancellationTokenSource cancleSouce;
        CancellationToken cancleToken;

        bool _running;
        Action _doAction;
        Task _task;
        int _millsecond;




        public int Millisecond
        {
            get => _millsecond;
            set => _millsecond = value;
        }

        public Action DoAction
        {
            get => _doAction;
            set => _doAction = value;
        }




        public CycleTimeTask(Action action,int millisecond)
        {
            _doAction = action;

            _millsecond = millisecond;
           
        }


        public void Start()
        {

            if (_running)
            {

            }
            else
            {
                _running = true;
                _task = createAndStartTask();
            }

        }


        public void Stop()
        {
            if (_running)
            {
                cancleSouce?.Cancel();
                _running = false;


                _task.Dispose();

            }
        }


        private Task createAndStartTask()
        {

            cancleSouce = new CancellationTokenSource();
            cancleToken = cancleSouce.Token;

            return Task.Factory.StartNew(() => {

                try
                {
                    while (_running && !cancleToken.IsCancellationRequested)
                    {
                        _doAction.Invoke();
                        Task.Delay(_millsecond, cancleToken).Wait(cancleToken);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"err:{ex.Message}");
                }


                _running = false;

            }


           , cancleToken);
        }


    }
}
